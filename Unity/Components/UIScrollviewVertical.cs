using UnityEngine;
using UnityEngine.UI;
using Prota.Unity;
using System.Collections.Generic;
using System.Linq;
using Prota;
using System.Collections;
using System;

namespace Prota.Unity
{
    public abstract class UIScrollviewVertical : MonoBehaviour
    {
        [field: SerializeField] public GameObject cellTemplate { get; protected set; }
        
        [field: SerializeField] public ScrollRect scroll { get; protected set; }
        
        [Readonly] public List<GameObject> cells = new List<GameObject>();
        
        Func<int, GameObject> internalNewCell;
        
        Action<int, GameObject> internalUpdateCell;
        
        Action<int, GameObject> internalDisableCell;
        
        protected Action<int, GameObject> updateCell;
        
        protected Action<int, GameObject> disableCell;
        
        public abstract int n { get; }
        
        public int maxCellPerLine
            => (this.RectTransform().rect.size.x / cellTemplate.RectTransform().rect.size.x).FloorToInt();
        
        protected virtual void OnValidate()
        {
            scroll ??= this.GetComponentInChildren<ScrollRect>();
            scroll.AssertNotNull();
            
            if(cellTemplate != null)
            {
                var p = cellTemplate.RectTransform().pivot;
                (p.x == 0.5f).Assert("格子 Pivot 必须在正中间");
                (p.y == 0.5f).Assert("格子 Pivot 必须在正中间");
            }
        }
        
        protected virtual void Awake()
        {
            internalNewCell = i => cellTemplate.Clone(scroll.content);
            
            internalUpdateCell = (i, cell) => {
                updateCell?.Invoke(i, cell);
                cell.RectTransform().anchoredPosition = PositionOfCell(i);
                cell.SetActive(true);
                
            };
            
            internalDisableCell = (i, cell) => {
                cell.SetActive(false);
                disableCell?.Invoke(i, cell);
            };
            
            scroll.AssertNotNull();
            cellTemplate.AssertNotNull();
            var cellPivot = cellTemplate.RectTransform().pivot;
            (cellPivot.x == 0.5f).Assert();
            (cellPivot.y == 0.5f).Assert();
            
            cellTemplate.SetActive(false);
        }
        
        public virtual void Update()
        {
            cells.SyncData(n, internalNewCell, internalUpdateCell, internalDisableCell);
            scroll.content.RectTransform().sizeDelta = ContentSize();
        }
        
        public virtual (int line, int column) GridOfCell(int i)
        {
            var maxCellPerLine = this.maxCellPerLine;
            var line = i / maxCellPerLine;
            var column = i % maxCellPerLine;
            return (line, column);
        }
        
        public virtual Vector2 PositionOfCell(int i)
        {
            var coord = GridOfCell(i);
            var cellHalfSize = cellTemplate.RectTransform().rect.size * 0.5f;
            var x = (2 * coord.column + 1) * cellHalfSize.x;
            var y = -(2 * coord.line + 1) * cellHalfSize.y;
            return new Vector2(x, y);
        }
        
        protected virtual Vector2 ContentSize()
        {
            int cellsN = cells.Count;
            var coord = GridOfCell(cellsN);
            var lineCount = coord.line + (cellsN == 0 ? 0 : 1);
            var columnCount = maxCellPerLine.Min(cellsN);
            var x = columnCount * cellTemplate.RectTransform().rect.size.x;
            var y = lineCount * cellTemplate.RectTransform().rect.size.y;
            return new Vector2(x, y);
        }
    }
}
