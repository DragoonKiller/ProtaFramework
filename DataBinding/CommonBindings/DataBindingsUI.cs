using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Prota.Data
{

    public class DataBindingsUI : DataBlock
    {
        public Image image => this.GetComponent<Image>();
        public Sprite sprite
        {
            get => image.sprite;
            set => image.sprite = value;
        }
        public Text text => this.GetComponent<Text>();
        public string textContent
        {
            get => text.text;
            set => text.text = value;
        }
        public RawImage rawImage => this.GetComponent<RawImage>();
        public Texture2D texture
        {
            get => (image ? image.mainTexture : rawImage.texture) as Texture2D;
            set => rawImage.texture = texture;
        }
        
        
        
    }
    
}