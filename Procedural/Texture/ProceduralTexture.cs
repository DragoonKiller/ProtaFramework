using UnityEngine;
using UnityEditor;

namespace Prota.Procedural
{
    [ExecuteAlways]
    public partial class ProceduralTexture : MonoBehaviour
    {
        public enum TextureType
        {
            None,
            Grid,
        }
        
        public TextureType type;
        
        [SerializeField] TextureType recordType;
        
        public Texture texture;
        
        public Sprite sprite;
        
        void Start()
        {
            
        }
        
        void Update()
        {
            if(recordType != type)
            {
                recordType = type;
                if(texture)
                {
                    DestroyImmediate(texture);
                    DestroyImmediate(sprite);
                }
                if(type == TextureType.None) { }
                else if(type == TextureType.Grid) texture = Grid(512, 512);
                else Debug.LogError("Wrong texture type.");
                
                if(texture)
                {
                    texture.name = "Generated";
                }
                if(texture is Texture2D t)
                {
                    sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f), 128, 0, SpriteMeshType.FullRect);
                    sprite.name = "Generated";
                }
            }
            
            if(UnityEngine.Application.isPlaying)
            {
                if(TryGetComponent<SpriteRenderer>(out var cc))
                {
                    cc.sprite = sprite;
                }
            }
        }
    }
}
