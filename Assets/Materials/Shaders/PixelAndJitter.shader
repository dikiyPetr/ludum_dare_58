Shader "Custom/PixelAndJitter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Range(1, 512)) = 64
        _JitterAmount ("Jitter Amount", Range(0, 1)) = 0.5
        _JitterSpeed ("Jitter Speed", Range(0, 10)) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _PixelSize;
            float _JitterAmount;
            float _JitterSpeed;

            // Функция для генерации псевдослучайного числа
            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Получаем реальный размер текстуры и вычисляем количество пикселей
                float2 texSize = _MainTex_TexelSize.zw / _PixelSize;

                // Пикселизируем UV координаты
                float2 pixelatedUV = floor(i.uv * texSize) / texSize;
                
                // Добавляем jitter эффект
                float time = floor(_Time.y * _JitterSpeed);
                float2 jitter = float2(
                    random(pixelatedUV + time),
                    random(pixelatedUV + time + 0.1)
                ) * 2.0 - 1.0;
                
                jitter *= _JitterAmount / texSize;
                
                // Применяем jitter к UV координатам
                float2 finalUV = pixelatedUV + jitter;
                
                // Сэмплируем текстуру с пикселизированными и jitter координатами
                fixed4 col = tex2D(_MainTex, finalUV);
                
                return col;
            }
            ENDCG
        }
    }
}