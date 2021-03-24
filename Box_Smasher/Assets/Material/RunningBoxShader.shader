Shader "BS/BS_ImageEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Direction("Moving Direction", Vector) = (1, 0, 0, 0)
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

            fixed3 _Direction;

            v2f vert (appdata v)
            {
                float pi = 3.141592 * 0.5;
                float x_dir = cos(v.uv.x * pi) * saturate(_Direction.x) + sin(v.uv.x * pi) * saturate(-1 * _Direction.x);
                float y = v.vertex.y - 0.5 * sin(v.uv.y * pi) * x_dir;
                v.vertex.y = y;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = fixed4(col.r * sin(_Direction.x), 0, 0, 1);
                return col;
            }
            ENDCG
        }
    }
}
