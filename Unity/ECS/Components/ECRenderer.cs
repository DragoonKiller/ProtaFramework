using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prota;
using Prota.Unity;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ECRenderer : EComponent
{
    [Serializable]
    abstract class RendererWrapper
    {
        abstract public void Init(GameObject g);
        abstract public Color color { get; set; }
        abstract public Material mat { get; set; }
        abstract public Texture2D texture { get; set; }
        abstract public Sprite sprite { get; set; }
    }
    
    [Serializable]
    sealed class SpriteRendererWrapper : RendererWrapper
    {
        [SerializeField] SpriteRenderer target;
        public override void Init(GameObject g) => target = g.GetComponent<SpriteRenderer>();
        public override Color color { get => target.color; set => target.color = value; }
        public override Material mat { get => target.GetMaterialInstance(); set => target.sharedMaterial = value; }
        public override Texture2D texture { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public override Sprite sprite { get => target.sprite; set => target.sprite = value; }
    }
    
    [Serializable]
    sealed class MeshRendererWrapper : RendererWrapper
    {
        [SerializeField] MeshRenderer target;
        public override void Init(GameObject g) => target = g.GetComponent<MeshRenderer>();
        public override Color color { get => target.material.color; set => target.material.color = value; }
        public override Material mat { get => target.GetMaterialInstance(); set => target.sharedMaterial = value; }
        public override Texture2D texture { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public override Sprite sprite { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
    }
    
    [Serializable]
    sealed class SkinnedMeshRendererWrapper : RendererWrapper
    {
        [SerializeField] SkinnedMeshRenderer target;
        public override void Init(GameObject g) => target = g.GetComponent<SkinnedMeshRenderer>();
        public override Color color { get => target.material.color; set => target.material.color = value; }
        public override Material mat { get => target.GetMaterialInstance(); set => target.sharedMaterial = value; }
        public override Texture2D texture { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public override Sprite sprite { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
    }
    
    [Serializable]
    sealed class LineRendererWrapper : RendererWrapper
    {
        [SerializeField] LineRenderer target;
        public override void Init(GameObject g) => target = g.GetComponent<LineRenderer>();
        public override Color color { get => target.material.color; set => target.material.color = value; }
        public override Material mat { get => target.GetMaterialInstance(); set => target.sharedMaterial = value; }
        public override Texture2D texture { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public override Sprite sprite { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
    }
    
    [Serializable]
    sealed class TrailRendererWrapper : RendererWrapper
    {
        [SerializeField] TrailRenderer target;
        public override void Init(GameObject g) => target = g.GetComponent<TrailRenderer>();
        public override Color color { get => target.material.color; set => target.material.color = value; }
        public override Material mat { get => target.GetMaterialInstance(); set => target.sharedMaterial = value; }
        public override Texture2D texture { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public override Sprite sprite { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
    }
    
    [Serializable]
    sealed class ImageWrapper : RendererWrapper
    {
        [SerializeField] Image target;
        public override void Init(GameObject g) => target = g.GetComponent<Image>();
        public override Color color { get => target.color; set => target.color = value; }
        public override Material mat { get => target.material; set => target.material = value; }
        public override Texture2D texture { get => (Texture2D)target.mainTexture; set => target.sprite = Sprite.Create(value, new Rect(0, 0, value.width, value.height), Vector2.zero); }
        public override Sprite sprite { get => target.sprite; set => target.sprite = value; }
    }
    
    [Serializable]
    sealed class RawImageWrapper : RendererWrapper
    {
        [SerializeField] RawImage target;
        public override void Init(GameObject g) => target = g.GetComponent<RawImage>();
        public override Color color { get => target.color; set => target.color = value; }
        public override Material mat { get => target.material; set => target.material = value; }
        public override Texture2D texture { get => (Texture2D)target.texture; set => target.texture = value; }
        public override Sprite sprite { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
    }
    
    [Serializable]
    sealed class TextWrapper : RendererWrapper
    {
        [SerializeField] Text target;
        public override void Init(GameObject g) => target = g.GetComponent<Text>();
        public override Color color { get => target.color; set => target.color = value; }
        public override Material mat { get => target.material; set => target.material = value; }
        public override Texture2D texture { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public override Sprite sprite { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
    }
    
    [Serializable]
    sealed class CanvasGroupWrapper : RendererWrapper
    {
        [SerializeField] CanvasGroup target;
        public override void Init(GameObject g) => target = g.GetComponent<CanvasGroup>();
        public override Color color { get => new Color(0, 0, 0, target.alpha); set => target.alpha = value.a; }
        public override Material mat { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public override Texture2D texture { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public override Sprite sprite { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
    }
    
    [Serializable]
    sealed class TextMeshProWrapper : RendererWrapper
    {
        [SerializeField] TextMeshPro target;
        public override void Init(GameObject g) => target = g.GetComponent<TextMeshPro>();
        public override Color color { get => target.color; set => target.color = value; }
        public override Material mat { get => target.material; set => target.material = value; }
        public override Texture2D texture { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public override Sprite sprite { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
    }
    
    [Serializable]
    sealed class TextMeshProUGUIWrapper : RendererWrapper
    {
        [SerializeField] TextMeshProUGUI target;
        public override void Init(GameObject g) => target = g.GetComponent<TextMeshProUGUI>();
        public override Color color { get => target.color; set => target.color = value; }
        public override Material mat { get => target.material; set => target.material = value; }
        public override Texture2D texture { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public override Sprite sprite { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
    }
    
    [SerializeReference, Readonly] RendererWrapper rendererWrapper;
    
    public Color color
    {
        get => rendererWrapper.color;
        set => rendererWrapper.color = value;
    }
    
    public Material mat
    {
        get => rendererWrapper.mat;
        set => rendererWrapper.mat = value;
    }
    
    public Texture2D texture
    {
        get => rendererWrapper.texture;
        set => rendererWrapper.texture = value;
    }
    
    public Sprite sprite
    {
        get => rendererWrapper.sprite;
        set => rendererWrapper.sprite = value;
    }
    
    public float transparency
    {
        get => rendererWrapper.color.a;
        set => rendererWrapper.color = rendererWrapper.color.WithA(value);
    }
    
    protected void OnValidate()
    {
        RendererWrapper newWrapper = null;
        if(TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            newWrapper = new SpriteRendererWrapper();
        }
        else if(TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            newWrapper = new MeshRendererWrapper();
        }
        else if(TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer))
        {
            newWrapper = new SkinnedMeshRendererWrapper();
        }
        else if(TryGetComponent<LineRenderer>(out var lineRenderer))
        {
            newWrapper = new LineRendererWrapper();
        }
        else if(TryGetComponent<TrailRenderer>(out var trailRenderer))
        {
            newWrapper = new TrailRendererWrapper();
        }
        else if(TryGetComponent<Image>(out var image))
        {
            newWrapper = new ImageWrapper();
        }
        else if(TryGetComponent<RawImage>(out var rawImage))
        {
            newWrapper = new RawImageWrapper();
        }
        else if(TryGetComponent<Text>(out var text))
        {
            newWrapper = new TextWrapper();
        }
        else if(TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            newWrapper = new CanvasGroupWrapper();
        }
        else if(TryGetComponent<TextMeshPro>(out var textMeshPro))
        {
            newWrapper = new TextMeshProWrapper();
        }
        else if(TryGetComponent<TextMeshProUGUI>(out var textMeshProUGUI))
        {
            newWrapper = new TextMeshProUGUIWrapper();
        }
        
        newWrapper.Init(gameObject);
        
        if(rendererWrapper != null && rendererWrapper.GetType() == newWrapper.GetType())
        {
            var t = new ProtaReflectionType(rendererWrapper.GetType());
            bool same = true;
            foreach(var field in t.allFields)
            {
                var v1 = field.GetValue(rendererWrapper);
                var v2 = field.GetValue(newWrapper);
                if(!v1.Equals(v2)) same = false;
            }
            if(same) return;
        }
        
        rendererWrapper = newWrapper;
        
    }
}
