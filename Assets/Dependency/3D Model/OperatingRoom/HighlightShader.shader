Shader "Custom/HighlightShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HighlightColor ("Highlight Color", Color) = (1,0,0,1)
        _HighlightPosition ("Highlight Position", Vector) = (0,0,0)
        _HighlightRadius ("Highlight Radius", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        fixed4 _HighlightColor;
        float4 _HighlightPosition;
        float _HighlightRadius;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            float dist = distance(IN.worldPos, _HighlightPosition.xyz);

            if (dist < _HighlightRadius)
            {
                c = _HighlightColor;
            }

            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
