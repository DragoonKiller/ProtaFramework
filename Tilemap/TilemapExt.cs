using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Prota;
using System.Collections.Concurrent;

namespace Prota.Unity
{
    public static class TilemapExt
    {
        public static void GetAllFilledPositions(this Tilemap tilemap, ICollection<Vector2Int> positions)
        {
            foreach(var p in tilemap.cellBounds.allPositionsWithin)
                if(tilemap.HasTile(p))
                    positions.Add(p.ToVec2Int());
        }
        
        // 恰好在边缘的tile距离为0.
        public static void DistanceToEdge(
            this Tilemap tilemap,
            Dictionary<Vector2Int, int> distance,
            Func<Vector2Int, Vector2Int, float> distanceMeasure = null
        )
        {
            if(distanceMeasure == null) distanceMeasure = (a, b) => a.Distance(b);
            var allPositions = new HashSet<Vector2Int>();
            tilemap.GetAllFilledPositions(allPositions);
            
            // BFS
            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();
            var adjacent = new Vector2Int[4];
            
            foreach(var p in allPositions)
            {
                if(visited.Contains(p)) continue;
                
                visited.Add(p);
                queue.Enqueue(p);
                for(int _ = 0; _ < 1e7 && queue.Count != 0; _++)
                {
                    adjacent[0] = p + Vector2Int.up;
                    adjacent[1] = p + Vector2Int.down;
                    adjacent[2] = p + Vector2Int.left;
                    adjacent[3] = p + Vector2Int.right;
                    foreach(var a in adjacent)
                        if(!visited.Contains(a))
                        {
                            distance[a] = distance[p] + 1;
                            visited.Add(a);
                            queue.Enqueue(a);
                        }
                }
            }
            
            
        }
        
    }
}
