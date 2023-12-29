// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Prota/DrawCulled"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _Cull ("Cull", 2D) = "white" { }
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
                float4 color : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 pos : POSITION1;
                float4 color : COLOR0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _Cull;
            float4 frag (v2f v) : SV_Target
            {
                float4 color = tex2D(_Cull, float2(v.pos.x, -v.pos.y) / 2 + float2(0.5, 0.5));
                if(color.r > 0.99) discard;
                color = tex2D(_MainTex, v.uv);
                return color * v.color;
            }
            ENDCG
        }
    }
}
