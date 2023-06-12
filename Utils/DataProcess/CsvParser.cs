
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Prota
{
    public enum CsvGridType
    {
        String = 0,
        Float = 1,
        Int = 2,
    }
    
    
    // specific CSV format:
    // use " to quote a field.
    // use "" to represent a quote in field.
    // other characters are kept.
    public class CsvParser
    {
        const char specialChar = '\xFF';
        
        public class HeaderInfo
        {
            readonly Dictionary<string, int> index = new Dictionary<string, int>();
            public readonly int headerLine = -1;
            public int this[string name] => index[name];
            public readonly IReadOnlyList<string> properties;
            public HeaderInfo(CsvParser parser, int headerLine)
            {
                var props = new List<string>();
                this.headerLine = headerLine;
                for(int i = 0; i < parser.columnN; i++)
                {
                    var name = parser.GetString(0, i);
                    if(name.NullOrEmpty()) continue;
                    index[name] = i;
                    props.Add(name);
                }
                properties = props;
            }
        }
        
        public readonly string name;
        
        public readonly string originalContent;
        
        public readonly string content;
        
        public readonly HeaderInfo headerInfo;
        
        public readonly int rowN;
        
        public readonly int columnN;
        
        public int dataRowN => rowN - dataRowBeginFrom;
        
        readonly List<List<int>> index;
        
        public int dataRowBeginFrom => headerInfo != null ? headerInfo.headerLine : 0;
        
        public readonly IReadOnlyList<CsvGridType> gridType;
        
        public CsvParser(string originalContent, int headerLine) : this("Unknown", originalContent, headerLine)
        {
            
        }
        
        public CsvParser(string name, string originalContent, int headerLine)
        {
            this.name = name;
            
            this.originalContent = originalContent = originalContent.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd();
            var sb = new StringBuilder();
            
            index = new List<List<int>>();
            
            int currentLine = 0;
            int currentColumn = 0;
            int maxColumn = 0;
            bool isInQuote = false;
            bool lastIsQuote = false;
            for(int cur = 0; cur < originalContent.Length; cur++)
            {
                var curChar = originalContent[cur];
                if(isInQuote)
                {
                    if(curChar == '\"')
                    {
                        isInQuote = false;
                        lastIsQuote = true;
                    }
                    else
                    {
                        sb.Append(curChar);
                        lastIsQuote = false;
                    }
                }
                else
                {
                    if(curChar == '\n')
                    {
                        RecordGrid();
                        index[currentLine].Add(sb.Length);
                        currentLine++;
                        currentColumn = 0;
                        lastIsQuote = false;
                    }
                    else if(curChar == ',')
                    {
                        RecordGrid();
                        currentColumn++;
                        maxColumn = Math.Max(maxColumn, currentColumn + 1);
                        lastIsQuote = false;
                    }
                    else if(curChar == '\"')
                    {
                        isInQuote = true;
                        if(lastIsQuote)
                        {
                            sb.Append('\"');
                            lastIsQuote = false;
                        }
                        lastIsQuote = true;
                    }
                    else
                    {
                        sb.Append(curChar);
                        lastIsQuote = false;
                    }
                }
            }
            
            RecordGrid();
            index[currentLine].Add(sb.Length);
            
            content = sb.ToString();
            
            void RecordGrid()        // tag finish section of a grid.
            {
                while(currentLine >= index.Count) index.Add(new List<int>());
                while(currentColumn >= index[currentLine].Count) index[currentLine].Add(-1);
                for(int j = sb.Length - 1; j >= -1; j--)
                {
                    if(j == -1 || sb[j] == specialChar)
                    {
                        index[currentLine][currentColumn] = j + 1;
                        break;
                    }
                }
                sb.Append(specialChar);
                // sb.ToString(index[currentLine][currentColumn], sb.Length - index[currentLine][currentColumn]).LogError();
            }
            
            rowN = currentLine + 1;
            columnN = maxColumn;
            
            if(headerLine != 0) headerInfo = new HeaderInfo(this, headerLine);
            
            // 检查行数是否一致.
            for(int i = 0; i < index.Count; i++)
            {
                if(index[i].Count != columnN + 1)
                    throw new Exception($"CSV [{name}] has inconsistent column number at line {i}.");
            }
            
            // 检查第 0 行是否有空字符串. 只有在启用 header 时生效.
            if(headerInfo != null)
            {
                for(int i = 0; i < columnN; i++)
                {
                    if(GetString(-dataRowBeginFrom, i).NullOrEmpty())
                        throw new Exception($"CSV [{name}] has empty string at row 0 column {i}.");
                }
            }
            
            // 获取所有行的类型. 跳过 header.
            var gtype = new CsvGridType[columnN];
            for(int col = 0; col < columnN; col++)
            {
                bool supportsInt = true;
                bool supportsFloat = true;
                for(int row = 0; row < dataRowN; row++)
                {
                    var str = GetString(row, col);
                    if(str.NullOrEmpty()) continue;
                    if(!int.TryParse(str, out var intResult)) supportsInt = false;
                    if(!float.TryParse(str, out var floatResult)) supportsFloat = false;
                    if(!supportsInt && !supportsFloat) break;
                }
                
                if(supportsInt) gtype[col] = CsvGridType.Int;
                else if(supportsFloat) gtype[col] = CsvGridType.Float;
                else gtype[col] = CsvGridType.String;
                this.gridType = gtype;
            }
        }
        
        void GetDataIndex(int i, int j, out int from, out int to)
        {
            from = to = -1;
            if(this.index.Count <= i) throw new Exception($"CSV [{name}] has no row [{i}].");
            if(this.index[i].Count - 1 <= j) throw new Exception($"CSV [{name}] has no column [{j}] at row [{i}].");
            from = this.index[i][j];
            to = this.index[i][j + 1] - 1;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        public string GetString(int row, int col)
        {
            GetDataIndex(row + dataRowBeginFrom, col, out var from, out var to);
            return content.Substring(from, to - from);
        }
        
        public int GetInt(int row, int col)
        {
            GetDataIndex(row + dataRowBeginFrom, col, out var from, out var to);
            var str = content.AsSpan(from, to - from);
            if(str.Length == 0) return 0;       // 默认值.
            if(!int.TryParse(str, out var v))
                throw new Exception($"CSV [{name}] has invalid int at row [{row}] column [{col}] value [{ new string(content.AsSpan(from, to - from)) }].");
            return v;
        }
        
        public float GetFloat(int row, int col)
        {
            GetDataIndex(row + dataRowBeginFrom, col, out var from, out var to);
            var str = content.AsSpan(from, to - from);
            if(str.Length == 0) return 0;       // 默认值.
            if(!float.TryParse(str, out var v))
                throw new Exception($"CSV [{name}] has invalid float at row [{row}] column [{col}] value [{ new string(content.AsSpan(from, to - from)) }].");
            return v;
        }
        
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public IEnumerable<(int r, int c, string grid)> grids
        {
            get
            {
                for(int i = 0; i < index.Count; i++) for(int j = 0, n = index[i].Count - 1; j < n; j++)
                {
                    GetDataIndex(i, j, out var from, out var to);
                    var s = new string(content.AsSpan(from, to - from));
                    yield return (i, j, s);
                }
            }
        }
        
        public override string ToString()
        {
            return grids.Select(x => $"({x.r},{x.c})[{x.grid}]").ToStringJoined("\n");
        }
        
        
        public static void UnitTest()
        {
            
        }
    }
}
