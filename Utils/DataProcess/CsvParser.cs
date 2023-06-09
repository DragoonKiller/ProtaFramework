
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Prota
{
    
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
            public HeaderInfo(CsvParser parser, int headerLine)
            {
                this.headerLine = headerLine;
                for(int i = 0; i < parser.columnN; i++)
                {
                    bool success = parser.Get(headerLine, i, out string name);
                    if(name.NullOrEmpty()) continue;
                    index[name] = i;
                }
            }
        }
        
        public readonly string name;
        
        public readonly string originalContent;
        
        public readonly string content;
        
        public HeaderInfo headerInfo { get; private set; }
        
        public readonly int rowN;
        
        public readonly int columnN;
        
        readonly List<List<int>> index;
        
        public CsvParser(string originalContent) : this("Unknown", originalContent)
        {
            
        }
        
        public CsvParser(string name, string originalContent)
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
        }
        
        public void SetHeader(int headerLine)
        {
            headerInfo = new HeaderInfo(this, headerLine);
        }
        
        bool GetDataIndex(int i, int j, out int from, out int to)
        {
            from = to = -1;
            if(this.index.Count <= i) return false;
            if(this.index[i].Count - 1 <= j) return false;
            from = this.index[i][j];
            to = this.index[i][j + 1] - 1;
            return true;
        }
                
        // ====================================================================================================
        // ====================================================================================================
        
        public bool Get(int row, int col, out string v)
        {
            if(!GetDataIndex(row, col, out var from, out var to))
            {
                v = null;
                return false;
            }
            v = content.Substring(from, to - from);
            return true;
        }
        
        public bool Get(int row, int col, out int v)
        {
            if(!GetDataIndex(row, col, out var from, out var to))
            {
                v = 0;
                return false;
            }
            return int.TryParse(content.AsSpan(from, to - from), out v);
        }
        
        public bool Get(int row, int col, out float v)
        {
            if(!GetDataIndex(row, col, out var from, out var to))
            {
                v = 0;
                return false;
            }
            return float.TryParse(content.AsSpan(from, to - from), NumberStyles.Float, CultureInfo.InvariantCulture, out v);
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
