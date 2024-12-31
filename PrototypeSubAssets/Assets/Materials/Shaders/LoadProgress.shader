Shader "Indigocoder/LoadProgress"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Nosie Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _LoadProgress ("Load Progress", Range(0,1)) = 1
        _RemapMin ("Remap Min", Float) = 0
        _RemapMax ("Remap Max", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _NoiseTex;

            fixed4 _Color;
            fixed _LoadProgress;
            float _RemapMin;
            float _RemapMax;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const float epsilon = 1e-7;

                fixed4 noiseCol = tex2D(_NoiseTex, i.uv);
                float val = dot(noiseCol, fixed3(0.3, 0.59, 0.11));
                float testValue = lerp(_RemapMin, _RemapMax, _LoadProgress);

                if (val < (1 - testValue)) discard;

                fixed4 col = tex2D(_MainTex, i.uv);
                return col * _Color;
            }
            ENDCG
        }
    }
}
