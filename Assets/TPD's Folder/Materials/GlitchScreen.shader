Shader "Custom/LoadingScreenNoiseUI"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.5
        _NoiseSpeed ("Noise Speed", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float _NoiseStrength;
            float _NoiseSpeed;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Tạo noise động
                float noise = frac(sin(dot(i.uv.xy + _Time.y * _NoiseSpeed, float2(12.9898, 78.233))) * 43758.5453);

                // Lấy màu texture cơ bản
                fixed4 col = tex2D(_MainTex, i.uv);

                // Áp dụng noise lên màu
                col.rgb += noise * _NoiseStrength;
                col.a = 1; // Đảm bảo độ trong suốt luôn là 1

                return col;
            }
            ENDCG
        }
    }
}
