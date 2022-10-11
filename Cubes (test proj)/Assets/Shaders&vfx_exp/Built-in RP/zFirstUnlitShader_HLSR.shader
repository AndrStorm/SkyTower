Shader "Unlit/zFirstUnlitShader_HLSR"
{
    Properties
    {
        _ColorBase ("Base Color", color) = (1,1,1,1)
        _ColorSecond ("Secondary Color", color) = (1,1,1,1)
        _AmbientLight ("Ambient Light", color) = (0,0,0,0)
        
        _waveAmp ("Wave Amplitude", float) = 0.1
        _pulseAmp ("Pulse Amplitude", float) = 1
        _distortionAmp ("Distortion Amplitude", float) = 0.035
        _pulseDist ("Pulse Distance", Range(0,1)) = 1
        
        _Gloss ("Gloss", Range(0,1)) = 1
        _NormalIntensity ("Normal Intensity", Range(0,1)) = 1
        _DispStrenght ("Dispacement Strenght", Range(0,0.1)) = 0
        
        _UOffset ("U Offset", Range(0,1)) = 0
        _VOffset ("V Offset", Range(0,1)) = 0
        
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset]_NormalTex("Normal map", 2D) = "Bump" {} //bump is a flat normal map
        [NoScaleOffset]_HeightTex("Height Tex", 2D) = "gray" {} //0.5 gray withot offset
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" //inform the render pipline (postproccessing)
            "Queue" = "Transparent" //"Queue" = "Transparent" //render order // Geometry causing appear AO (pp) stuff
            "RenderPipeline" = "LightweightPipeline"
         }
        LOD 100
        //base pass
        Pass
        {
            //Tags{ "LightMode" = "ForwardBase"}
            //Cull Off //removing mesh render part front/back/off
            ZWrite Off //Depth buffer
            //ZTest LEqual //drawing through other meshes, default LEqual/Always/GEqual
            Blend One One //additive
            //Blend OneMinusDstColor One
            //Blend SrcAlpha OneMinusSrcAlpha // aphaBlending src*a + dist* (1-a) 


            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_builtin
            //#pragma multi_compile_fog // make fog work
            
            
            #define BASE_PASS //key difinition
            #include "zFirstUnlitShaderLighting_HLSR.cginc"
            
            ENDHLSL
        }
         
        //dir light pass
        /*
        Pass
        {
            
            Tags{ "LightMode" = "UniversalForward"}
            Blend One One //additive

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd //attenation work with
            //#pragma multi_compile_fog // make fog work
            
            #define USE_LIGHTING //key difinition
            #include "zFirstUnlitShaderLighting_HLSR.cginc"
            
            ENDHLSL
        }*/
        
    }
}
