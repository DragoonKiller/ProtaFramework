// using System;
// using System.Linq;
// using Prota;
// using Prota.Unity;
// using UnityEditor.SceneTemplate;
// using UnityEngine;
// using UnityEngine.U2D;
// using UnityEngine.UIElements;
// 
// [ExecuteAlways]
// [RequireComponent(typeof(SpriteRenderer))]
// [DisallowMultipleComponent]
// public class SpriteShearing : MonoBehaviour
// {
//     SpriteRenderer rd => this.GetComponent<SpriteRenderer>();
//     
//     public float shearAngle = 0f;
//     
//     // 需要绘制的原始sprite.
//     public Sprite sprite;
//     
//     // 当前 instanceSprite 对应的是哪一个 sprite.
//     [NonSerialized] Sprite recordSprite;
//     
//     // 当前绘制的改变过mesh的sprite.
//     [NonSerialized] Sprite instanceSprite;
//     
//     [NonSerialized] float recordShearAngle = 0f;
//     
//     [NonSerialized] Vector2[] originalVertices;
//     
//     [NonSerialized] Vector2[] vertices;
//     
//     [NonSerialized] ushort[] indices;
//     
//     void Update()
//     {
//         UpdateSprite();
//     }
//     
//     void OnDestroy()
//     {
//         if(instanceSprite != null) DestroyImmediate(instanceSprite);
//     }
//     
//     void UpdateSprite()
//     {
//         if(!sprite && recordSprite)
//         {
//             if(instanceSprite) DestroyImmediate(instanceSprite);
//             originalVertices = vertices = null;
//             indices = null;
//             recordShearAngle = 0f;
//             recordSprite = sprite = null;
//             rd.sprite = null;
//             return;
//         }
//         
//         bool needUpdateSprite = recordSprite != sprite;
//         bool needUpdateVertices = needUpdateSprite || shearAngle != recordShearAngle;
//         
//         if(needUpdateSprite)
//         {
//             var templateSprite = recordSprite = sprite;
//             
//             if(instanceSprite) DestroyImmediate(instanceSprite);
//             instanceSprite = Sprite.Create(
//                 templateSprite.texture,
//                 templateSprite.rect,
//                 templateSprite.rect.ToLocalPosition(templateSprite.pivot),
//                 templateSprite.pixelsPerUnit,
//                 128,
//                 SpriteMeshType.FullRect);
//             instanceSprite.name = "CloneSprite:" + templateSprite.name;
//             
//             // void DD(Sprite s, string a)
//             // {
//             //     Debug.LogError($"{a}:{ s.name }\n{ s.textureRect }\n{ s.rect }\n{ s.pivot }\n{ s.border }\n{ s.vertices.ToStringJoined("|") }");
//             // } 
//             
//             // DD(templateSprite, "templateSprite");
//             // DD(instanceSprite, "instanceSprite");
//             // 
//             rd.sprite = instanceSprite;
//         
//             originalVertices = templateSprite.vertices;
//             vertices = templateSprite.vertices;
//             Debug.Assert(vertices.Length == 4);
//             indices = templateSprite.triangles;
//             
//         }
//         
//         if(needUpdateVertices)
//         {
//             recordShearAngle = shearAngle;
//             
//             var lt = originalVertices[0];
//             var rt = originalVertices[1];
//             var rb = originalVertices[2];
//             var lb = originalVertices[3];
//             
//             var dy = rt.y - rb.y;
//             var xOffset = dy * shearAngle.ToRadian().Sin();
//             var yOffset = dy * (shearAngle.ToRadian().Cos() - 1);
//             
//             vertices[0] = lt + new Vector2(xOffset, yOffset);
//             vertices[1] = rt + new Vector2(xOffset, yOffset);
//             vertices[2] = rb;
//             vertices[3] = lb;
//             
//             // Debug.LogError(originalVertices.ToStringJoined("&"));
//             // Debug.LogError(vertices.ToStringJoined("|"));
//             
//             // https://docs.unity3d.com/ScriptReference/Sprite-vertices.html
//             var spriteRect = sprite.rect;
//             var bound = sprite.bounds;
//             for(int i = 0; i < vertices.Length; i++)
//             {
//                 float x = (
//                         (vertices[i].x - bound.center.x - (sprite.textureRectOffset.x / sprite.texture.width) + bound.extents.x) /
//                         (2.0f * bound.extents.x) * spriteRect.width
//                     ).Clamp(0.0f, spriteRect.width);
// 
//                 float y = (
//                         (vertices[i].y - bound.center.y - (sprite.textureRectOffset.y / sprite.texture.height) + bound.extents.y) /
//                         (2.0f * bound.extents.y) * spriteRect.height
//                     ).Clamp(0.0f, spriteRect.height);
//                     
//                 vertices[i].x = x;
//                 vertices[i].y = y;
//             }
//             
//             // Debug.LogError(vertices.ToStringJoined("?"));
//             
//             instanceSprite.OverrideGeometry(vertices, indices);
//         }
//         
//     }
// }
