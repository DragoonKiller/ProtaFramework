Shader "Prota/Sprite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _AlphaClip ("Alpha Clip", float) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _AlphaClip;
            float4 frag (v2f v) : SV_Target
            {
                float4 color = tex2D(_MainTex, v.uv);
                if(color.a < _AlphaClip) discard;
                return float4(1, 1, 1, 1);
            }
            ENDCG
        }
    }
}
