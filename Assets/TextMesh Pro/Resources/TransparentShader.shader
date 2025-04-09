Shader "Custom/TransparentShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (0, 1, 1, 0.7) // 색, 반투명
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // 알파 블렌딩 (투명도 적용)
            ZWrite Off // 투명한 부분이 다른 오브젝트를 가리지 않게 변경
            ColorMask RGBA // 알파(투명도) 채널 활성화

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            fixed4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(_Color.rgb, _Color.a); // 알파 값 적용
            }
            ENDCG
        }
    }
}
