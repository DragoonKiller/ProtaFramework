Shader "Prota/Sprite"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 0.0
        [Enum(Prota.Unity.OnOffEnum)] _ZWrite ("ZWrite", float) = 0.0
        
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Blend Src", Float) = 1.0
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Blend Dst", Float) = 0.0
        
        
        _MainTex ("Texture", 2D) = "white" { }
        
        _Color ("Color", Color) = (1,1,1,1)
        _AddColor ("Add Color", Color) = (0,0,0,0)
        
        _AlphaClip ("Alpha Clip", float) = 0.5
        
        _HueOffset ("Hue Offset", float) = 0.0
        _SaturationOffset ("Saturation Offset", float) = 1.0
        _BrightnessOffset ("Brightness Offset", float) = 1.0
        _ContrastOffset ("Contrast Offset", float) = 1.0
        
    }
    SubShader
    {
        Cull Off
        ZWrite [_ZWrite]
        ZTest [_ZTest]
        Blend [_BlendSrc] [_BlendDst]
        
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "LightMode" = "Universal2D"
        }
        
        Pass
        {
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "ProtaUtils.cginc"
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _HueOffset;
            float _SaturationOffset;
            float _BrightnessOffset;
            float _ContrastOffset;
            float _AlphaClip;
            float4 _AddColor;
            float4 _Flip;
            float4 _UVOffset;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f v) : SV_Target
            {
                float4 color = tex2D(_MainTex, v.uv);
                if(color.a <= _AlphaClip) discard;
                
                color = HueOffset(color, _HueOffset);
                color = ContrastOffset(color, _ContrastOffset);
                color = SaturationOffset(color, _SaturationOffset);
                color = BrightnessOffset(color, _BrightnessOffset);
                
                return color * _Color + _AddColor;
            }
            
            // ================================================================
            // ================================================================
            
            
            
            ENDCG
        }
    }
}
