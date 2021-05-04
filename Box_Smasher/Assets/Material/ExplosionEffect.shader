
Shader "Effects/ExplosionEffect"
{
	Properties
    {
        [PerRendererData]_MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("Normal Texture", 2D) = "white" {}
        _PaletteMap("PaletteMap", 2D) = "white" {}
        _GradientMap("GradientMap", 2D) = "white" {}
        _Offset("float", float) = 0
    }
    SubShader
    {
        Tags
        { 
            "RenderType" = "Transparent" "Queue" = "Transparent" 
        }

        GrabPass
        {
            "_BackgroundTexture"
        }


        Pass
        {
            Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
		    ZWrite Off 
            Blend SrcAlpha OneMinusSrcAlpha 
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define mod(x, y) x-y*floor(x/y)

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
                float4 grabPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);

                return o;
            }

            sampler2D _MainTex;
            sampler2D _BackgroundTexture;
            sampler2D _NoiseTex;
            sampler2D _PaletteMap;
            sampler2D _GradientMap;
            float _Offset;

            float rand(float2 co){
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 displacementPixel = tex2D(_NoiseTex, i.uv);

                float2 pos = i.grabPos / i.grabPos.w;
                pos.x += (displacementPixel.r * 2.0 - 1.0) * 0.025;
                pos.y -= (displacementPixel.g * 2.0 - 1.0) * 0.025;

                // get the displaced pixel
                float4 pixel = tex2D(_BackgroundTexture, pos);

                // proper grayscale conversion.
                float gray = dot(pixel.rgb, float3(0.299, 0.587, 0.114));
                float4 palettePixel = tex2D(_PaletteMap, gray);

                float gradientPos = mod(gray + palettePixel.g, 1);

                // Get the color from the gradient
                float4 gradientMapPixel = tex2D(_GradientMap, float2(gradientPos, palettePixel.r));
                
                pixel = lerp(pixel, gradientMapPixel, palettePixel.a);

                float4 color = tex2D(_MainTex, i.uv);
                float4 fragColor = pixel;
                fragColor.a = color.a;

                return fragColor;
            }
            ENDCG
        }
    }
}