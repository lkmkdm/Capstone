Shader "Custom/TransparentShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (0, 1, 1, 0.7) // ��, ������
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // ���� ���� (���� ����)
            ZWrite Off // ������ �κ��� �ٸ� ������Ʈ�� ������ �ʰ� ����
            ColorMask RGBA // ����(����) ä�� Ȱ��ȭ

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
                return fixed4(_Color.rgb, _Color.a); // ���� �� ����
            }
            ENDCG
        }
    }
}
