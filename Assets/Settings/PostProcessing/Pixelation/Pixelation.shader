Shader "CustomEffects/Pixelation"
{
    Properties
    {
        _BlitTexture("Texture", 2D) = "white" {}
    }
    
    HLSLINCLUDE
    
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
    
    int _PixelSize;
    int _ColorDepth;
    float _DitherStrength;
    float _TimeValue;
    
    // Матрица Байера 8x8 для дизеринга
    static const float bayerMatrix[64] = {
        0.0/64.0,  32.0/64.0,  8.0/64.0,  40.0/64.0,  2.0/64.0,  34.0/64.0, 10.0/64.0, 42.0/64.0,
        48.0/64.0, 16.0/64.0, 56.0/64.0, 24.0/64.0, 50.0/64.0, 18.0/64.0, 58.0/64.0, 26.0/64.0,
        12.0/64.0, 44.0/64.0,  4.0/64.0, 36.0/64.0, 14.0/64.0, 46.0/64.0,  6.0/64.0, 38.0/64.0,
        60.0/64.0, 28.0/64.0, 52.0/64.0, 20.0/64.0, 62.0/64.0, 30.0/64.0, 54.0/64.0, 22.0/64.0,
        3.0/64.0,  35.0/64.0, 11.0/64.0, 43.0/64.0,  1.0/64.0, 33.0/64.0,  9.0/64.0, 41.0/64.0,
        51.0/64.0, 19.0/64.0, 59.0/64.0, 27.0/64.0, 49.0/64.0, 17.0/64.0, 57.0/64.0, 25.0/64.0,
        15.0/64.0, 47.0/64.0,  7.0/64.0, 39.0/64.0, 13.0/64.0, 45.0/64.0,  5.0/64.0, 37.0/64.0,
        63.0/64.0, 31.0/64.0, 55.0/64.0, 23.0/64.0, 61.0/64.0, 29.0/64.0, 53.0/64.0, 21.0/64.0
    };
    
    float getBayer(uint x, uint y)
    {
        return bayerMatrix[(y % 8u) * 8u + (x % 8u)];
    }
    
    // Основной пасс с пикселизацией и палитризацией
    float4 PixelationMain(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        
        float2 uv = input.texcoord;

        // Пикселизация
        float2 pixelatedUV = uv;
        int2 pixelCoord = int2(0, 0);
        
        if (_PixelSize > 1)
        {
            float2 pixelCount = _ScreenParams.xy / _PixelSize;
            pixelatedUV = floor(uv * pixelCount) / pixelCount;
            pixelCoord = int2(floor(uv * _ScreenParams.xy));
        }
        
        float3 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, pixelatedUV).rgb;
        
        // Дизеринг перед уменьшением цветовой палитры
        if (_DitherStrength > 0.01 && _ColorDepth < 256)
        {
            float dither = getBayer(pixelCoord.x, pixelCoord.y);
            // Нормализуем дизер под текущую глубину цвета
            float ditherScale = 1.0 / _ColorDepth;
            color += (dither - 0.5) * ditherScale * _DitherStrength;
        }
        
        // Уменьшение цветовой глубины (палитризация)
        color = floor(color * _ColorDepth + 0.5) / _ColorDepth;

        return float4(color, 1.0);
    }
    
    // Простой пасс копирования
    float4 Copy(Varyings input) : SV_Target
    {
        return SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
    }
    
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        
        // Pass 0: Pixelation and Color Reduction
        Pass
        {
            Name "PixelationMain"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment PixelationMain
            ENDHLSL
        }
        
        // Pass 1: Copy
        Pass
        {
            Name "Copy"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Copy
            ENDHLSL
        }
    }
}