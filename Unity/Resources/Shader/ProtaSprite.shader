Shader "Prota/Sprite"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 0.0
        [Enum(Off, 0, On, 1)] _ZWrite ("ZWrite", Float) = 0.0
        
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Blend Src", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Blend Dst", Float) = 0
        
        _MainTex ("Texture", 2D) = "white" { }
        _MaskTex("Mask", 2D) = "white" { }
        _Normal("Normal", 2D) = "bump" { }
        
        [HDR] _MaskUsage("Mask Weight", Color) = (0, 0, 0, 1)
        
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        [HDR] _AddColor ("Add Color", Color) = (0,0,0,0)
        [HDR] _OverlapColor ("Overlap Color", Color) = (0,0,0,0)
        
        _AlphaClip ("Alpha Clip", float) = 0.5
        
        _HueOffset ("Hue Offset", float) = 0.0
        _SaturationOffset ("Saturation Offset", float) = 0.0
        _BrightnessOffset ("Brightness Offset", float) = 0.0
        _ContrastOffset ("Contrast Offset", float) = 0.0
        _HueConcentrate ("Hue Concentration", float) = 0.0
        
        [Range(0, 255)] _StencilRef ("Stencil Ref", Integer) = 0
        [Range(0, 255)] _StencilReadMask ("Stencil Read Mask", Integer) = 0
        [Range(0, 255)] _StencilWriteMask ("Stencil Write Mask", Integer) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilCompare ("Stencil Compare", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Cull Off
        ZWrite [_ZWrite]
        ZTest [_ZTest]
        Blend [_BlendSrc] [_BlendDst]
        
        

        Pass
        {
            Tags { "LightMode" = "Universal2D" }
            
            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilCompare]
                Pass [_StencilOp]
            }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment
            
            #pragma multi_compile _USE_LIGHT __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __
            #pragma multi_compile _ DEBUG_DISPLAY

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2  uv          : TEXCOORD0;
                float2 maskUV       : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                float4   color       : COLOR;
                float2  uv          : TEXCOORD0;
                float2   lightingUV  : TEXCOORD1;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif
                float2 maskUV      : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
            
            #include "./ProtaUtils.cginc"
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            half4 _MainTex_ST;
            
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            half4 _MaskTex_ST;
            
            float4 _Color;
            float4 _AddColor;
            float4 _OverlapColor;
            float _HueOffset;
            float _SaturationOffset;
            float _BrightnessOffset;
            float _ContrastOffset;
            float _AlphaClip;
            float4 _Flip;
            float4 _UVOffset;
            float4 _MaskUsage;
            
            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(v.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);

                o.color = v.color * _Color;
                
                o.maskUV = TRANSFORM_TEX(v.maskUV, _MaskTex);
                
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            float4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                float4 main = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                
                const float4 mask = _MaskUsage * SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.maskUV);
                
                if(main.a * mask.a <= _AlphaClip) discard;
                
                float3 hsl;
                RGBtoHSL(main.rgb, hsl);
                HueOffsetHSL(hsl, _HueOffset);
                ContrastOffsetHSL(hsl, _ContrastOffset);
                SaturationOffsetHSL(hsl, _SaturationOffset);
                BrightnessOffsetHSL(hsl, _BrightnessOffset);
                HSLtoRGB(hsl, main.rgb);
                
                main.rgb += _AddColor.rgb;
                main.rgb = _OverlapColor.a * _OverlapColor.rgb + (1 - _OverlapColor.a) * main.rgb;
                
                main *= mask;
                
                #if _USE_LIGHT
                SurfaceData2D surfaceData;
                InputData2D inputData;
                InitializeSurfaceData(main.rgb, main.a, float4(1, 1, 1, 1), surfaceData);
                InitializeInputData(i.uv, i.lightingUV, inputData);
                return CombinedShapeLightShared(surfaceData, inputData);
                #else
                return main;
                #endif
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "NormalsRendering"}

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float4 tangent      : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                half4   color           : COLOR;
                float2  uv              : TEXCOORD0;
                half3   normalWS        : TEXCOORD1;
                half3   tangentWS       : TEXCOORD2;
                half3   bitangentWS     : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            half4 _MainTex_ST;
            
            TEXTURE2D(_Normal);
            SAMPLER(sampler_Normal);
            half4 _Normal_ST;

            Varyings NormalsRenderingVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _Normal);
                o.color = attributes.color;
                o.normalWS = -GetViewForwardDir();
                o.tangentWS = TransformObjectToWorldDir(attributes.tangent.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * attributes.tangent.w;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            half4 NormalsRenderingFragment(Varyings i) : SV_Target
            {
                const half4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                const half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_Normal, sampler_Normal, i.uv));
                return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz);
            }
            ENDHLSL
        }
        
    }

    Fallback "Sprites/Default"
}

