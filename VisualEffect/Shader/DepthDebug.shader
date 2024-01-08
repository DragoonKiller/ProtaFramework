Shader "Hidden/Prota/DepthDebug"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
    }
    
    SubShader
    {
        Cull Off ZWrite On ZTest Always
        
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
            float4 frag (v2f v) : SV_Target
            {
                float4 c = tex2D(_MainTex, v.uv);
                float d = c.r; // DECODE_EYEDEPTH(c.r);
                return float4(d, d, d, 1);
            }
            ENDCG
        }
    }
}
