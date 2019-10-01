Shader "Custom/ColorHeight"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _GraphRange ("GraphRange", Range(2, 8)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        struct Input
        {
			float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        half _GraphRange;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo.r = IN.worldPos.x * 0.5f + 0.5f;
            o.Albedo.g = IN.worldPos.y * 0.5f + 0.5f;
            o.Albedo.b = IN.worldPos.z * 0.5f + 0.5f;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
