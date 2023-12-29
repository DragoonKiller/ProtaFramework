// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Prota/RadialBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _SampleCount ("Sample Count", Range(1, 100)) = 10
        _Center ("Center (in pixel, screen space)", Vector) = (0, 0, 0, 0)
        _Radius ("Light Radius (in pixel, screen space)", float) = 10
        _SampleRadius ("Sample Radius (in pixel, screen space)", float) = 1
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
                float4 pos : POSITION1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _Center;
            float _Radius;
            float _SampleRadius;
            uniform int _SampleCount;
            float4 frag (v2f v) : SV_Target
            {
                float2 screenSize = float2(_MainTex_TexelSize.z, _MainTex_TexelSize.w);
                float2 normalizedScreenPos = float2(v.pos.x, -v.pos.y) / 2 + float2(0.5, 0.5);
                float2 screenPos = normalizedScreenPos * screenSize;
                
                float2 dir = screenPos - _Center.xy;
                float2 sampleDir = normalize(dir) * min(length(dir), _SampleRadius);
                
                float4 total = float4(0, 0, 0, 0);
                float sum = 0;
                for(int i = 0; i < _SampleCount; i++)
                {
                    float sampleDirScale = i / (_SampleCount - 1.0);
                    float2 dirScaled = sampleDir * sampleDirScale;
                    float2 sampleScreenPos = _Center.xy + dirScaled;
                    float4 color = tex2D(_MainTex, sampleScreenPos / screenSize);
                    float weight = 1;
                    total += color * weight;
                    sum += weight;
                }
                total /= sum;
                
                float dirlen = length(dir);
                total *= dirlen < _SampleRadius ? 1
                    : dirlen < _Radius ? 1 - (dirlen - _SampleRadius) / (_Radius - _SampleRadius)
                    : 0;
                
                
                /*
                float c = length(dir) / _Radius;
                total = float4(c, c, c, 1);
                */
                
                return total;
            }
            ENDCG
        }
    }
}
