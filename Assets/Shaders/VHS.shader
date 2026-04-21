Shader "Custom/VHS"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Noise ("Noise", Float) = 0.015
        _Scanline ("Scanline", Float) = 600
        _Distortion ("Distortion", Float) = 0.002
        _RGBShift ("RGB Shift", Float) = 0.001
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Noise;
            float _Scanline;
            float _Distortion;
            float _RGBShift;

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

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy,float2(12.9898,78.233))) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y;

                float2 uv = i.uv;

                // Horizontal VHS distortion
                float distortion = sin(uv.y * 50 + time * 5) * _Distortion;
                uv.x += distortion;

                // Noise
                float noise = rand(uv + time) * _Noise;

                // Scanlines
                float scan = sin(uv.y * _Scanline) * 0.04;

                // RGB Shift
                float r = tex2D(_MainTex, uv + float2(_RGBShift, 0)).r;
                float g = tex2D(_MainTex, uv).g;
                float b = tex2D(_MainTex, uv - float2(_RGBShift, 0)).b;

                fixed4 col = fixed4(r, g, b, 1.0);

                col.rgb += noise;
                col.rgb -= scan;

                return col;
            }

            ENDCG
        }
    }
}