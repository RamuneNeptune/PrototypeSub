Shader "Unlit/EmissiveSlider"
{
    Properties
    {
        _EmisiveTex ("Emissive Texture", 2D) = "white" {}
        _FilledAmount ("Amount filled", Range(0, 1)) = 1
        _UnderlyingCol ("Underlying Color", Color) = (1, 1, 1, 1)
        _SpecColor ("Spec color", Color) = (1, 1, 1, 1)
        _RimColor ("Rim color", Color) = (1, 1, 1, 1)
        _Shininess ("Shininess", Float) = 1
        _RimPower ("Rim power", Float) = 1
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };

            sampler2D _EmisiveTex;
            float4 _EmisiveTex_ST;

            fixed4 _UnderlyingCol;
            fixed4 _SpecColor;
            fixed4 _RimColor;
            fixed _FilledAmount;
            float _Shininess;
            float _RimPower;

            float4 _LightColor0;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _EmisiveTex);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.normalDir = mul(float4(v.normal, 0), unity_WorldToObject).xyz;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 emissiveCol = tex2D(_EmisiveTex, i.uv);

                if (i.uv.y > (1 - _FilledAmount))
                {
                    return emissiveCol;
                }

                float3 normalDir = i.normalDir;
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 lightDir;
                float atten;

                if (_WorldSpaceLightPos0.w == 0.0) // Directional lightDir
                {
                    atten = 1;
                    lightDir = normalize(_WorldSpaceCameraPos.xyz);
                }
                else // Point light or spotlight
                {
                    float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					float distance = length(fragmentToLightSource);
					atten = 1.0 / distance;
					lightDir = normalize(fragmentToLightSource);
                }

                // Lighting
                float3 lightingModel = max(0.0, dot(normalDir, lightDir));
                float3 diffuseReflection = atten * _LightColor0.xyz * lightingModel;

                float3 specularNoShiny = atten * max(0.0, dot(reflect(-lightDir, normalDir), viewDir)) * lightingModel;
				float3 specularWithColor = pow(specularNoShiny, _Shininess) * _SpecColor.rgb;
				float3 specularReflectionFinal = specularWithColor + diffuseReflection + UNITY_LIGHTMODEL_AMBIENT;
				
				//Rim Lighting
				float rim = 1 - saturate(dot(viewDir, normalDir));
				float3 rimLighting = saturate(dot(normalDir, lightDir)) * pow(rim, 10 - _RimPower) * _RimColor * atten * _LightColor0.xyz;

				float3 lightFinal = rimLighting + diffuseReflection + specularReflectionFinal + UNITY_LIGHTMODEL_AMBIENT.rgb;

                return fixed4(lightFinal * _UnderlyingCol, 1);
            }
            ENDCG
        }
    }
}
