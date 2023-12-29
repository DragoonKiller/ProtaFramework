Shader "Hidden/Prota/GaussianBlurSinglePassVertical"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _Mult ("Mult", float) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            float4 _MainTex_TexelSize;
            float _Mult;
            
            static const float c[] ={
                0.011, 0.022, 0.032, 0.027, 0.025, 0.033, 0.045, 0.063, 0.09, 0.13, 0.17, 0.21
            };
            
            float4 frag (v2f v) : SV_Target
            {
                float4 total = float4(0.0, 0.0, 0.0, 0.0);
                float sum = 0.0;
                
                for(int i = -11; i <= 11; i++)
                {
                    int k = 11 - abs(i);
                    float2 offset = float2(0, i * _MainTex_TexelSize.y);
                    total += c[k] * tex2D(_MainTex, v.uv + offset);
                    sum += c[k];
                }
                
                return float4(total.r, total.g, total.b, 1) / sum * _Mult;
            }
            ENDCG
        }
    }
}
