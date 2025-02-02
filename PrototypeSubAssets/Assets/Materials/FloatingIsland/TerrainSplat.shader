Shader "Indigocoder/TerrainSplat"
{
    Properties
    {
        _AmbientColor ("Ambient Color", Color) = (1,1,1,1)
        _SpecColor ("Specular Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Float) = 10
        _RimColor ("Rim Color", Color) = (1.0,1.0,1.0,1.0)
		_RimPower ("Rim Power", Range(0, 9.99)) = 3
        _BumpStrength ("Normal Strength", Range(-2, 2)) = 1
        [NoScaleOffset] _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset] _BaseNormal ("Base Normal", 2D) = "bump" {}
        _BaseScale ("Base Scale", Float) = 1
        [NoScaleOffset] _DetailTex1 ("Detail Tex 1", 2D) = "white" {}
        [NoScaleOffset] _DetailNormal1 ("Detail Normal 1", 2D) = "bump" {}
        _DetailScale1 ("Detail Scale 1", Float) = 1
        [NoScaleOffset] _DetailTex2 ("Detail Tex 2", 2D) = "white" {}
        [NoScaleOffset] _DetailNormal2 ("Detail Normal 1", 2D) = "bump" {}
        _DetailScale2 ("Detail Scale 2", Float) = 1
        [NoScaleOffset]_DetailTex3 ("Detail Tex 3", 2D) = "white" {}
        [NoScaleOffset] _DetailNormal3 ("Detail Normal 1", 2D) = "bump" {}
        _DetailScale3 ("Detail Scale 3", Float) = 1
    }
    SubShader
    {
        Pass
        {
            Tags { "LightMode" = "ForwardBase" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
            Blend One Zero
            LOD 100
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma multi_compile SPOT POINT

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                fixed3 normalWorld : TEXCOORD2;
                fixed3 tangentWorld : TEXCOORD3;
                fixed3 binormalWorld : TEXCOORD4;
                SHADOW_COORDS(5)
            };

            float4 _LightColor0;
            
            fixed4 _AmbientColor;
            fixed4 _SpecColor;
            half _Shininess;
            fixed4 _RimColor;
			half _RimPower;

            sampler2D _MainTex;
            sampler2D _BaseNormal;
            fixed4 _MainTex_ST;
            sampler2D _DetailTex1;
            sampler2D _DetailNormal1;
            sampler2D _DetailTex2;
            sampler2D _DetailNormal2;
            sampler2D _DetailTex3;
            sampler2D _DetailNormal3;

            float _BumpStrength;
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

                o.normalWorld = normalize(mul(float4(v.normal, 0), unity_WorldToObject).xyz);
                o.tangentWorld = normalize(mul(v.tangent, unity_WorldToObject).xyz);
                o.binormalWorld = normalize(cross(o.normalWorld, o.tangentWorld) * v.tangent.w); // Multiply by W to get correct length

                TRANSFER_SHADOW(o)

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

                // Combine all detail textures and retain brightness
                fixed4 detailCol = (detail1 + detail2 + detail3) / (vertexColor.r + vertexColor.g + vertexColor.b);
                detailCol = saturate(detailCol);
                float alpha = saturate(length(vertexColor.rgb));

                return lerp(base, detailCol, alpha);
            }

            fixed4 calculateNormalTex(float3 worldPos, float3 normalDir, fixed4 vertexColor)
            {
                fixed4 base = triplanarMap(_BaseNormal, worldPos, normalDir, _BaseScale);
                fixed4 detail1 = triplanarMap(_DetailNormal1, worldPos, normalDir, _DetailScale1) * vertexColor.r;
                fixed4 detail2 = triplanarMap(_DetailNormal2, worldPos, normalDir, _DetailScale2) * vertexColor.g;
                fixed4 detail3 = triplanarMap(_DetailNormal3, worldPos, normalDir, _DetailScale3) * vertexColor.b;

                fixed4 detailCol = (detail1 + detail2 + detail3) / (vertexColor.r + vertexColor.g + vertexColor.b);
                detailCol = saturate(detailCol);
                float alpha = saturate(length(vertexColor.rgb));

                return lerp(base, detailCol, alpha);
            }

            float angleBetween(float3 dirA, float3 dirB)
            {
                const fixed epsilon = 1e-6;

                float sqrMagA = length(dirA);
                sqrMagA *= sqrMagA;
                float sqrMagB = length(dirB);
                sqrMagB *= sqrMagB;

                float denominator = sqrt(sqrMagA * sqrMagB);
                if (denominator < epsilon) return 0;

                float dotProd = clamp(dot(dirA, dirB) / denominator, -1, 1);
                return acos(dotProd);
            }

            float invLerp(float from, float to, float value)
            {
                return (value - from) / (to - from);
            }

            #ifdef SPOT
            float4 calculateLightProperties(float3 posWorld)
            {
                float4 lightCoord = mul(unity_WorldToLight, float4(posWorld, 1));
                float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
                float distance = length(fragmentToLightSource);
                float range = distance / length(lightCoord.xyz);
                float cotanHalfSpotAngle = 2 * lightCoord.z / lightCoord.w;
  
                float3 lightDir = normalize(fragmentToLightSource);

                float3 worldLightUnitDir = normalize(mul(float3(0,0,-1), (float3x3)unity_WorldToLight));

                float outerCutoff = atan(1 / cotanHalfSpotAngle);

                float normAngle = invLerp(0, outerCutoff, angleBetween(lightDir, worldLightUnitDir));
                float angleMultiplier = lerp(1, 0, normAngle);

                float atten = angleMultiplier * lerp(1, 0, smoothstep(0, range, distance * 0.9));

                return float4(lightDir, atten);
            }
            #endif

            #ifdef POINT
            float4 calculateLightProperties(float3 posWorld)
            {
                float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
                float3 lightDir = normalize(fragmentToLightSource);
			    float distance = length(fragmentToLightSource);

                float4 lightCoord = mul(unity_WorldToLight, float4(posWorld, 1));
                float range = distance / length(lightCoord.xyz);

                float normDist = distance / range;
                float remappedDist = lerp(0, 5000, invLerp(0, range * range, normDist * normDist));
			    float atten = (range / 10) / (1 + distance * distance);
                return float4(lightDir, atten);
            }
            #endif

            float4 calculateLightFinal(float3 normalDirection, float3 posWorld, fixed4 vertexColor, float3 tangentWorld, float3 binormalWorld, float3 normalWorld, float shadow)
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
				else //Point light or spot light
				{
                    float4 properties = calculateLightProperties(posWorld);
                    lightDir = properties.xyz;
                    atten = properties.w;                    
				}

                fixed4 normalTex = calculateNormalTex(posWorld, normalDirection, vertexColor);

                float3 localCoords = float3(2 * normalTex.ag - float2(1, 1), 0);
                localCoords.z = _BumpStrength;

                // Normal transpose matrix
                float3x3 local2WorldTranspose = float3x3(
                    tangentWorld,
                    binormalWorld,
                    normalWorld
                );

                // Calcuate normal direction
                float3 newNormalDirection = normalize(mul(localCoords, local2WorldTranspose));

				//Lighting
				float3 lightingModel = max(0.0, dot(newNormalDirection, lightDir));
				float3 diffuseReflection = atten * _LightColor0.xyz * lightingModel;

				float3 specularNoShiny = atten * max(0.0, dot(reflect(-lightDir, newNormalDirection), viewDir)) * lightingModel;
				float3 specularWithColor = pow(specularNoShiny, _Shininess) * _SpecColor.rgb;
				
				//Rim lighting
				float rim = 1 - saturate(dot(viewDir, newNormalDirection));
				float3 rimLighting = saturate(dot(newNormalDirection, lightDir)) * pow(rim, 10 - _RimPower) * _RimColor * atten * _LightColor0.xyz;

				float3 lightFinal = rimLighting + diffuseReflection + specularWithColor + _AmbientColor.rgb;
                lightFinal *= shadow;

				return float4(lightFinal, 1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float shadowMult = SHADOW_ATTENUATION(i);

                fixed4 lightFinal = calculateLightFinal(i.normalWorld, i.worldPos, i.color, i.tangentWorld, i.binormalWorld, i.normalWorld, shadowMult);
                return calculateBaseColor(i.worldPos, i.normalWorld, i.color) * lightFinal;
            }
            ENDCG
        }
        Pass
        {
            Tags { "LightMode" = "ForwardAdd" }
            Blend One One
            LOD 100
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma multi_compile SPOT POINT

            #include "UnityCG.cginc"
            #include "UnityShaderVariables.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                fixed3 normalWorld : TEXCOORD2;
                fixed3 tangentWorld : TEXCOORD3;
                fixed3 binormalWorld : TEXCOORD4;
                SHADOW_COORDS(5)
            };

            float4 _LightColor0;
            
            fixed4 _AmbientColor;
            fixed4 _SpecColor;
            half _Shininess;
            fixed4 _RimColor;
			half _RimPower;

            sampler2D _MainTex;
            sampler2D _BaseNormal;
            fixed4 _MainTex_ST;
            sampler2D _DetailTex1;
            sampler2D _DetailNormal1;
            sampler2D _DetailTex2;
            sampler2D _DetailNormal2;
            sampler2D _DetailTex3;
            sampler2D _DetailNormal3;

            float _BumpStrength;
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

                o.normalWorld = normalize(mul(float4(v.normal, 0), unity_WorldToObject).xyz);
                o.tangentWorld = normalize(mul(v.tangent, unity_WorldToObject).xyz);
                o.binormalWorld = normalize(cross(o.normalWorld, o.tangentWorld) * v.tangent.w); // Multiply by W to get correct length

                TRANSFER_SHADOW(o)

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

                // Combine all detail textures and retain brightness
                fixed4 detailCol = (detail1 + detail2 + detail3) / (vertexColor.r + vertexColor.g + vertexColor.b);
                detailCol = saturate(detailCol);
                float alpha = saturate(length(vertexColor.rgb));

                return lerp(base, detailCol, alpha);
            }

            fixed4 calculateNormalTex(float3 worldPos, float3 normalDir, fixed4 vertexColor)
            {
                fixed4 base = triplanarMap(_BaseNormal, worldPos, normalDir, _BaseScale);
                fixed4 detail1 = triplanarMap(_DetailNormal1, worldPos, normalDir, _DetailScale1) * vertexColor.r;
                fixed4 detail2 = triplanarMap(_DetailNormal2, worldPos, normalDir, _DetailScale2) * vertexColor.g;
                fixed4 detail3 = triplanarMap(_DetailNormal3, worldPos, normalDir, _DetailScale3) * vertexColor.b;

                fixed4 detailCol = (detail1 + detail2 + detail3) / (vertexColor.r + vertexColor.g + vertexColor.b);
                detailCol = saturate(detailCol);
                float alpha = saturate(length(vertexColor.rgb));

                return lerp(base, detailCol, alpha);
            }

            float angleBetween(float3 dirA, float3 dirB)
            {
                const fixed epsilon = 1e-6;

                float sqrMagA = length(dirA);
                sqrMagA *= sqrMagA;
                float sqrMagB = length(dirB);
                sqrMagB *= sqrMagB;

                float denominator = sqrt(sqrMagA * sqrMagB);
                if (denominator < epsilon) return 0;

                float dotProd = clamp(dot(dirA, dirB) / denominator, -1, 1);
                return acos(dotProd);
            }

            float invLerp(float from, float to, float value)
            {
                return (value - from) / (to - from);
            }

            #ifdef SPOT
            float4 calculateLightProperties(float3 posWorld)
            {
                float4 lightCoord = mul(unity_WorldToLight, float4(posWorld, 1));
                float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
                float distance = length(fragmentToLightSource);
                float range = distance / length(lightCoord.xyz);
                float cotanHalfSpotAngle = 2 * lightCoord.z / lightCoord.w;
  
                float3 lightDir = normalize(fragmentToLightSource);

                float3 worldLightUnitDir = normalize(mul(float3(0,0,-1), (float3x3)unity_WorldToLight));

                float outerCutoff = atan(1 / cotanHalfSpotAngle);

                float normAngle = invLerp(0, outerCutoff, angleBetween(lightDir, worldLightUnitDir));
                float angleMultiplier = lerp(1, 0, normAngle);

                float atten = angleMultiplier * lerp(1, 0, smoothstep(0, range, distance * 0.9));

                return float4(lightDir, saturate(atten));
            }
            #endif

            #ifdef POINT
            float4 calculateLightProperties(float3 posWorld)
            {
                float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
                float3 lightDir = normalize(fragmentToLightSource);
			    float distance = length(fragmentToLightSource);

                float4 lightCoord = mul(unity_WorldToLight, float4(posWorld, 1));
                float range = distance / length(lightCoord.xyz);

                float normDist = distance / range;
                float remappedDist = lerp(0, 5000, invLerp(0, range * range, normDist * normDist));
			    float atten = (range / 10) / (1 + distance * distance);
                return float4(lightDir, atten);
            }
            #endif

            float4 calculateLightFinal(float3 normalDirection, float3 posWorld, fixed4 vertexColor, float3 tangentWorld, float3 binormalWorld, float3 normalWorld, float shadow)
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
				else //Point light or spot light
				{
                    float4 properties = calculateLightProperties(posWorld);
                    lightDir = properties.xyz;
                    atten = properties.w;                    
				}

                fixed4 normalTex = calculateNormalTex(posWorld, normalDirection, vertexColor);

                float3 localCoords = float3(2 * normalTex.ag - float2(1, 1), 0);
                localCoords.z = _BumpStrength;

                // Normal transpose matrix
                float3x3 local2WorldTranspose = float3x3(
                    tangentWorld,
                    binormalWorld,
                    normalWorld
                );

                // Calcuate normal direction
                float3 newNormalDirection = normalize(mul(localCoords, local2WorldTranspose));

				//Lighting
				float3 lightingModel = max(0.0, dot(newNormalDirection, lightDir));
				float3 diffuseReflection = atten * _LightColor0.xyz * lightingModel;

				float3 specularNoShiny = atten * max(0.0, dot(reflect(-lightDir, newNormalDirection), viewDir)) * lightingModel;
				float3 specularWithColor = pow(specularNoShiny, _Shininess) * _SpecColor.rgb;
				
				//Rim lighting
				float rim = 1 - saturate(dot(viewDir, newNormalDirection));
				float3 rimLighting = saturate(dot(newNormalDirection, lightDir)) * pow(rim, 10 - _RimPower) * _RimColor * atten * _LightColor0.xyz;

				float3 lightFinal = rimLighting + diffuseReflection + specularWithColor;
                lightFinal *= shadow;

				return float4(lightFinal, 1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float shadowMult = SHADOW_ATTENUATION(i);

                fixed4 lightFinal = calculateLightFinal(i.normalWorld, i.worldPos, i.color, i.tangentWorld, i.binormalWorld, i.normalWorld, shadowMult);
                return calculateBaseColor(i.worldPos, i.normalWorld, i.color) * lightFinal;
            }
            ENDCG
        }    }

    Fallback "Diffuse"
}
