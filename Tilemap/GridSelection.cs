using System.Collections.Generic;
using Prota.Unity;
using UnityEngine;


public class GridSelection : MonoBehaviour
{
    public RectTransform selection;
    
    public Vector2Int selectionPosMin
    {
        get
        {
            var pos = selection.transform.TransformPoint(selection.rect.min);
            var posInt = pos.CeilToInt();
            return new Vector2Int(posInt.x, posInt.y);
        }
    }
    
    public Vector2Int selectionPosMax
    {
        get
        {
            var pos = selection.transform.TransformPoint(selection.rect.max);
            var posInt = pos.FloorToInt();
            return new Vector2Int(posInt.x, posInt.y);
        }
    }
    
    public OverworldTilemap overworldTilemap;
    
    public readonly Dictionary<Vector2Int, GameObject> coordToChunk = new();
    
    public void RebindObjects()
    {
        coordToChunk.Clear();
        var d = new Dictionary<string, GameObject>();
        foreach(Transform tr in this.transform)
            d.Add(tr.name, tr.gameObject);
        foreach(var chunk in overworldTilemap.chunks)
        {
            if(d.TryGetValue(chunk.chunkObjectName, out var obj))
            {
                coordToChunk[chunk.chunkCoord] = obj;    
            }
            else
            {
                var g = new GameObject(chunk.chunkObjectName);
                g.transform.SetParent(this.transform);
                coordToChunk[chunk.chunkCoord] = g;
            }
        }
    }
    
    
    
    // ====================================================================================================
    // ====================================================================================================
    
    void OnDrawGizmos()
    {
        
    }
}
