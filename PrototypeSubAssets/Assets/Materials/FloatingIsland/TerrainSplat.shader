Shader "Unlit/TerrainSplat"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BaseScale ("Base Scale", Float) = 1
        _DetailTex1 ("Detail Tex 1", 2D) = "white" {}
        _DetailScale1 ("Detail Scale 1", Float) = 1
        _DetailTex2 ("Detail Tex 2", 2D) = "white" {}
        _DetailScale2 ("Detail Scale 2", Float) = 1
        _DetailTex3 ("Detail Tex 3", 2D) = "white" {}
        _DetailScale3 ("Detail Scale 3", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                fixed4 color : COLOR;
                fixed3 normalDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST;
            sampler2D _DetailTex1;
            fixed4 _DetailTex1_ST;
            sampler2D _DetailTex2;
            sampler2D _DetailTex3;

            half _BaseScale;
            half _DetailScale1;
            half _DetailScale2;
            half _DetailScale3;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.color = v.color;
                o.normalDir = mul(float4(v.normal, 0), unity_WorldToObject).xyz;
                return o;
            }

            float4 triplanarMap(sampler2D tex, float3 worldPos, float3 normal, float scale)
            {
                float3 blendWeights = abs(normal);
                blendWeights /= (blendWeights.x + blendWeights.y + blendWeights.z);
                float2 uvX = worldPos.zy * scale;
                float2 uvY = worldPos.xz * scale;
                float2 uvZ = worldPos.xy * scale;

                float4 texX = tex2D(tex, uvX);
                float4 texY = tex2D(tex, uvY);
                float4 texZ = tex2D(tex, uvZ);

                return texX * blendWeights.x + texY * blendWeights.y + texZ * blendWeights.z;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 base = triplanarMap(_MainTex, i.worldPos, i.normalDir, _BaseScale);
                fixed4 detail1 = triplanarMap(_DetailTex1, i.worldPos, i.normalDir, _DetailScale1) * i.color.r;
                fixed4 detail2 = triplanarMap(_DetailTex2, i.worldPos, i.normalDir, _DetailScale2) * i.color.g;
                fixed4 detail3 = triplanarMap(_DetailTex3, i.worldPos, i.normalDir, _DetailScale3) * i.color.b;

                fixed4 detailCol = (detail1 + detail2 + detail3) / (i.color.r + i.color.g + i.color.b);
                detailCol = saturate(detailCol);
                float alpha = saturate(length(i.color.rgb));

                return lerp(base, detailCol, alpha);
            }
            ENDCG
        }
    }
}
