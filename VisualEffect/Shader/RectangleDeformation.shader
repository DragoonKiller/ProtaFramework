Shader "Hidden/RectangleDeformation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        [HDR] _Color ("Color", color) = (1, 1, 1, 1)
        
        // ratio in rectangle range
        _CoordBottomLeft ("CoordBottomLeft", vector) = (0, 0, 0, 0)
        _CoordBottomRight ("CoordBottomRight", vector) = (1, 0, 0, 0)
        _CoordTopLeft ("CoordTopLeft", vector) = (0, 1, 0, 0)
        _CoordTopRight ("CoordTopRight", vector) = (1, 1, 0, 0)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Cull Off
        ZWrite On
        ZTest Less
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "ProtaUtils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            CBUFFER_START(UnityPerDraw)
            CBUFFER_END
            
            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            float4 _Color;
            float2 _CoordBottomLeft;
            float2 _CoordBottomRight;
            float2 _CoordTopLeft;
            float2 _CoordTopRight;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float solveOneDirection(float2 baseA, float2 dirA, float2 baseB, float2 dirB, float2 target)
            {
                float a = cross(dirA, dirB);
                float b = cross(dirA, baseB - target) + cross(baseA - target, dirB);
                float c = cross(baseA - target, baseB - target);
                int rootCount = 0;
                float2 sol = float2(0, 0);
                solve2(a, b, c, sol, rootCount);
                
                if(rootCount == 0) return 0;
                if(rootCount == 1) return sol.x;
                
                float2 s = float2(
                    dot(sol.x * dirA + baseA - target, sol.x * dirB + baseB - target),
                    dot(sol.y * dirA + baseA - target, sol.y * dirB + baseB - target)
                );
                if(s.x < 0) return sol.x;
                if(s.y < 0) return sol.y;
                return sol.x;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float x = solveOneDirection(_CoordBottomLeft, _CoordBottomRight - _CoordBottomLeft, _CoordTopLeft, _CoordTopRight - _CoordTopLeft, i.uv);
                float y = solveOneDirection(_CoordBottomLeft, _CoordTopLeft - _CoordBottomLeft, _CoordBottomRight, _CoordTopRight - _CoordBottomRight, i.uv);
                float2 uv = float2(x, y);
                
                fixed4 col = tex2D(_MainTex, uv);
                if(!(0 <= uv.x && uv.x <= 1 && 0 <= uv.y && uv.y <= 1)) col = float4(0, 0, 0, 0);
                
                // col = float4(i.uv, 0, 1);
                // col = float4(uv, 0, 1);
                col *= i.color * _Color;
                return col;
            }
            ENDCG
        }
    }
}
