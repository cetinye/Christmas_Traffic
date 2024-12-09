Shader "Unlit/DottedLineShader"
{
    Properties
    {
        _Color ("Dot Color", Color) = (1, 1, 1, 1)
        _RepeatCount("Repeat Count", float) = 1
        _Spacing("Spacing", float) = -0.9
        _Offset("Offset", float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _RepeatCount;
            float _Spacing;
            float _Offset;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;              
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
            };

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

                // Transform the vertex to world space
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

                // Billboard logic: Align to camera
                float3 toCamera = normalize(_WorldSpaceCameraPos - worldPos.xyz);
                worldPos.xyz += toCamera * _Spacing;

                // Transform to clip space
                o.vertex = UnityObjectToClipPos(worldPos);

                // Handle UV and offset logic
                o.uv = v.uv;
                o.uv.x = (o.uv.x + _Offset) * _RepeatCount * (1.0f + _Spacing);
                o.color = v.color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // UV wrapping for dotted effect
                i.uv.x = fmod(i.uv.x, 1.0f + _Spacing);
                float r = length(i.uv - float2(1.0f + _Spacing, 1.0f) * 0.5f) * 2.0f;

                fixed4 color = i.color;
                color.a *= saturate((0.99f - r) * 100.0f);

                return color * _Color;
            }
            ENDCG
        }
    }
}
