Shader "Indigocoder/TerrainSplat"
{
    Properties
    {
        _SpecColor ("Specular Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Float) = 10
        _RimColor ("Rim Color", Color) = (1.0,1.0,1.0,1.0)
		_RimPower ("Rim Power", Range(0, 9.99)) = 3.0
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
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            LOD 100

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
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                fixed3 normalDir : TEXCOORD2;
            };

            float4 _LightColor0;

            fixed4 _SpecColor;
            half _Shininess;
            fixed4 _RimColor;
			half _RimPower;

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
                o.normalDir = normalize(mul(float4(v.normal, 0), unity_WorldToObject).xyz);

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

            fixed4 calculateBaseColor(float3 worldPos, float3 normalDir, fixed4 vertexColor)
            {
                fixed4 base = triplanarMap(_MainTex, worldPos, normalDir, _BaseScale);
                fixed4 detail1 = triplanarMap(_DetailTex1, worldPos, normalDir, _DetailScale1) * vertexColor.r;
                fixed4 detail2 = triplanarMap(_DetailTex2, worldPos, normalDir, _DetailScale2) * vertexColor.g;
                fixed4 detail3 = triplanarMap(_DetailTex3, worldPos, normalDir, _DetailScale3) * vertexColor.b;

                fixed4 detailCol = (detail1 + detail2 + detail3) / (vertexColor.r + vertexColor.g + vertexColor.b);
                detailCol = saturate(detailCol);
                float alpha = saturate(length(vertexColor.rgb));

                return lerp(base, detailCol, alpha);
            }

            fixed4 calculateLightFinal(float3 normalDirection, float3 posWorld)
            {
                //Vectors
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
				float3 lightDir;
				float atten;

				if(_WorldSpaceLightPos0.w == 0.0) //Directional light
				{
					atten = 1.0;
					lightDir = normalize(_WorldSpaceLightPos0.xyz);
				}
				else //Point light or spotlight
				{
					float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
					float distance = length(fragmentToLightSource);
					atten = 1 / distance;
					lightDir = normalize(fragmentToLightSource);
				}

				//Lighting
				float3 lightingModel = max(0.0, dot(normalDirection, lightDir));
				float3 diffuseReflection = atten * _LightColor0.xyz * lightingModel;

				float3 specularNoShiny = atten * max(0.0, dot(reflect(-lightDir, normalDirection), viewDir)) * lightingModel;
				float3 specularWithColor = pow(specularNoShiny, _Shininess) * _SpecColor.rgb;
				
				//Rim Lighting
				float rim = 1 - saturate(dot(viewDir, normalDirection));
				float3 rimLighting = saturate(dot(normalDirection, lightDir)) * pow(rim, 10 - _RimPower) * _RimColor * atten * _LightColor0.xyz;

				float3 lightFinal = rimLighting + diffuseReflection + specularWithColor + UNITY_LIGHTMODEL_AMBIENT.rgb;

				return fixed4(lightFinal, 1.0);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 lightFinal = calculateLightFinal(i.normalDir, i.worldPos);
                return calculateBaseColor(i.worldPos, i.normalDir, i.color) * lightFinal;
            }
            ENDCG
        }
        Pass
        {
            Tags { "LightMode"="ForwardAdd" }
            Blend One One
            LOD 100

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
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                fixed3 normalDir : TEXCOORD2;
            };

            float4 _LightColor0;

            fixed4 _SpecColor;
            half _Shininess;
            fixed4 _RimColor;
			half _RimPower;

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
                o.normalDir = normalize(mul(float4(v.normal, 0), unity_WorldToObject).xyz);

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

            fixed4 calculateBaseColor(float3 worldPos, float3 normalDir, fixed4 vertexColor)
            {
                fixed4 base = triplanarMap(_MainTex, worldPos, normalDir, _BaseScale);
                fixed4 detail1 = triplanarMap(_DetailTex1, worldPos, normalDir, _DetailScale1) * vertexColor.r;
                fixed4 detail2 = triplanarMap(_DetailTex2, worldPos, normalDir, _DetailScale2) * vertexColor.g;
                fixed4 detail3 = triplanarMap(_DetailTex3, worldPos, normalDir, _DetailScale3) * vertexColor.b;

                fixed4 detailCol = (detail1 + detail2 + detail3) / (vertexColor.r + vertexColor.g + vertexColor.b);
                detailCol = saturate(detailCol);
                float alpha = saturate(length(vertexColor.rgb));

                return lerp(base, detailCol, alpha);
            }

            fixed4 calculateLightFinal(float3 normalDirection, float3 posWorld)
            {
                //Vectors
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
				float3 lightDir;
				float atten;

				if(_WorldSpaceLightPos0.w == 0.0) //Directional light
				{
					atten = 1.0;
					lightDir = normalize(_WorldSpaceLightPos0.xyz);
				}
				else //Point light or spotlight
				{
					float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
					float distance = length(fragmentToLightSource);
					atten = 1 / distance;
					lightDir = normalize(fragmentToLightSource);
				}

				//Lighting
				float3 lightingModel = max(0.0, dot(normalDirection, lightDir));
				float3 diffuseReflection = atten * _LightColor0.xyz * lightingModel;

				float3 specularNoShiny = atten * max(0.0, dot(reflect(-lightDir, normalDirection), viewDir)) * lightingModel;
				float3 specularWithColor = pow(specularNoShiny, _Shininess) * _SpecColor.rgb;
				
				//Rim Lighting
				float rim = 1 - saturate(dot(viewDir, normalDirection));
				float3 rimLighting = saturate(dot(normalDirection, lightDir)) * pow(rim, 10 - _RimPower) * _RimColor * atten * _LightColor0.xyz;

				float3 lightFinal = rimLighting + diffuseReflection + specularWithColor;

				return fixed4(lightFinal, 1.0);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 lightFinal = calculateLightFinal(i.normalDir, i.worldPos);
                return calculateBaseColor(i.worldPos, i.normalDir, i.color) * lightFinal;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
