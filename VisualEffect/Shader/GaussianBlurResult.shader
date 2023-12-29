Shader "Hidden/Prota/GaussianBlurResult"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _Intensity ("Intensity", float) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        
        Blend One OneMinusSrcAlpha
        
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
            float _Intensity;
            float4 frag (v2f v) : SV_Target
            {
                float4 c = tex2D(_MainTex, v.uv);
                return float4(
                    _Intensity * c.r,
                    _Intensity * c.g,
                    _Intensity * c.b,
                    _Intensity * max(max(c.r, c.g), c.b)
                );
            }
            ENDCG
        }
    }
}
