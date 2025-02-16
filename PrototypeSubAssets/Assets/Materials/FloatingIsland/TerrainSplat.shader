Shader "Indigocoder/TerrainSplat"
{
    Properties
    {
        _AmbientColor ("Ambient Color", Color) = (1,1,1,1)
        _FogColor ("Fog Color", Color) = (1, 1, 1, 1)
        _FogMaxDist ("Fog Max Dist", Float) = 50
        _SpecColor ("Specular Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Float) = 10
        _RimColor ("Rim Color", Color) = (1.0,1.0,1.0,1.0)
		_RimPower ("Rim Power", Range(0, 9.99)) = 3
        _BumpStrength ("Normal Strength", Range(0, 1)) = 0.5
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
        _NightMultiplier ("Debug Night Multiplier", Range(0,1)) = 1
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

            #pragma multi_compile_fog
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
                float3 normalWorld : TEXCOORD2;
            };

            float4 _LightColor0;
            
            fixed4 _AmbientColor;
            fixed4 _SpecColor;
            fixed4 _FogColor;
            half _Shininess;
            fixed4 _RimColor;
			half _RimPower;
            float _FogMaxDist;

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

            fixed _NightMultiplier = 1;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.color = v.color;

                o.normalWorld = UnityObjectToWorldNormal(v.normal);

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

            // Reoriented Normal Mapping
            // http://blog.selfshadow.com/publications/blending-in-detail/
            // Altered to take normals (-1 to 1 ranges) rather than unsigned normal maps (0 to 1 ranges)
            float3 blend_rnm(float3 n1, float3 n2)
            {
	            n1.z += 1;
	            n2.xy = -n2.xy;

	            return n1 * dot(n1, n2) / n1.z - n2;
            }

            // Taken from Sebastian Lague
            // https://github.com/SebLague/Solar-System/blob/0c60882be69b8e96d6660c28405b9d19caee76d5/Assets/Scripts/Celestial/Shaders/Includes/Triplanar.cginc
            float3 triplanarNormal(float3 worldPos, float3 normal, float scale, sampler2D normalMap)
            {
                float3 absNormal = abs(normal);
                float3 blendWeight = saturate(pow(normal, 4));
                blendWeight /= dot(blendWeight, 1);

                float2 uvX = worldPos.zy * scale;
	            float2 uvY = worldPos.xz * scale;
	            float2 uvZ = worldPos.xy * scale;

                float3 tangentNormalX = UnpackNormal(tex2D(normalMap, uvX));
	            float3 tangentNormalY = UnpackNormal(tex2D(normalMap, uvY));
	            float3 tangentNormalZ = UnpackNormal(tex2D(normalMap, uvZ));

                tangentNormalX = blend_rnm(half3(normal.zy, absNormal.x), tangentNormalX);
	            tangentNormalY = blend_rnm(half3(normal.xz, absNormal.y), tangentNormalY);
	            tangentNormalZ = blend_rnm(half3(normal.xy, absNormal.z), tangentNormalZ);

                float3 axisSign = sign(normal);
	            tangentNormalX.z *= axisSign.x;
	            tangentNormalY.z *= axisSign.y;
	            tangentNormalZ.z *= axisSign.z;

                float3 outputNormal = normalize(
		            tangentNormalX.zyx * blendWeight.x +
		            tangentNormalY.xzy * blendWeight.y +
		            tangentNormalZ.xyz * blendWeight.z
	            );

                return outputNormal;
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

            fixed3 calculateNormalTex(float3 worldPos, float3 normalDir, fixed4 vertexColor)
            {
                fixed3 base = triplanarNormal(worldPos, normalDir, _BaseScale, _BaseNormal);
                fixed3 detail1 = triplanarNormal(worldPos, normalDir, _DetailScale1, _DetailNormal1) * vertexColor.r;
                fixed3 detail2 = triplanarNormal(worldPos, normalDir, _DetailScale2, _DetailNormal2) * vertexColor.g;
                fixed3 detail3 = triplanarNormal(worldPos, normalDir, _DetailScale3, _DetailNormal3) * vertexColor.b;

                fixed3 detailCol = (detail1 + detail2 + detail3) / (vertexColor.r + vertexColor.g + vertexColor.b);
                detailCol = saturate(detailCol);
                float alpha = saturate(length(vertexColor.rgb));

                return lerp(base, detailCol, alpha).xyz;
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

            float4 calculateLightFinal(float3 normalDirection, float3 posWorld, fixed4 vertexColor, float3 normalWorld)
            {
                //Vectors
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
				float3 lightDir;
                float atten;

				if(_WorldSpaceLightPos0.w == 0.0) //Directional light
				{
					atten = _NightMultiplier;
					lightDir = normalize(_WorldSpaceLightPos0.xyz);
				}
				else //Point light or spot light
				{
                    float4 properties = calculateLightProperties(posWorld);
                    lightDir = properties.xyz;
                    atten = properties.w;                    
				}

                // Calcuate normal direction
                float3 newNormalDirection = calculateNormalTex(posWorld, normalWorld, vertexColor);
                newNormalDirection = lerp(normalWorld, newNormalDirection, _BumpStrength);

				//Lighting
				float3 lightingModel = max(0.0, dot(newNormalDirection, lightDir));
				float3 diffuseReflection = atten * _LightColor0.xyz * lightingModel;

				float3 specularNoShiny = atten * max(0.0, dot(reflect(-lightDir, newNormalDirection), viewDir)) * lightingModel;
				float3 specularWithColor = pow(specularNoShiny, _Shininess) * _SpecColor.rgb;
				
				//Rim lighting
				float rim = 1 - saturate(dot(viewDir, newNormalDirection));
				float3 rimLighting = saturate(dot(newNormalDirection, lightDir)) * pow(rim, 10 - _RimPower) * _RimColor * atten * _LightColor0.xyz;

				float3 lightFinal = max(rimLighting + diffuseReflection + specularWithColor, _AmbientColor.rgb);

				return float4(lightFinal, 1);
            }

            fixed getFogScalar(float3 posWorld)
            {
                float dist = length(posWorld - _WorldSpaceCameraPos);
                float scalar = _FogMaxDist * 50 / (dist * dist);
                scalar = 1 - saturate(scalar);

                return scalar;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 lightFinal = calculateLightFinal(i.normalWorld, i.worldPos, i.color, i.normalWorld);
                fixed4 finalColor = calculateBaseColor(i.worldPos, i.normalWorld, i.color) * lightFinal;

                return lerp(finalColor, _FogColor, getFogScalar(i.worldPos));
            }
            ENDCG
        }
        Pass
        {
            Tags { "LightMode" = "ForwardAdd" }
            Blend One One
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog
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
                float3 normalWorld : TEXCOORD2;
            };

            float4 _LightColor0;
            
            fixed4 _AmbientColor;
            fixed4 _SpecColor;
            fixed4 _FogColor;
            half _Shininess;
            fixed4 _RimColor;
			half _RimPower;
            float _FogMaxDist;

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

            fixed _NightMultiplier = 1;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.color = v.color;

                o.normalWorld = UnityObjectToWorldNormal(v.normal);

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

            // Reoriented Normal Mapping
            // http://blog.selfshadow.com/publications/blending-in-detail/
            // Altered to take normals (-1 to 1 ranges) rather than unsigned normal maps (0 to 1 ranges)
            float3 blend_rnm(float3 n1, float3 n2)
            {
	            n1.z += 1;
	            n2.xy = -n2.xy;

	            return n1 * dot(n1, n2) / n1.z - n2;
            }

            // Taken from Sebastian Lague
            // https://github.com/SebLague/Solar-System/blob/0c60882be69b8e96d6660c28405b9d19caee76d5/Assets/Scripts/Celestial/Shaders/Includes/Triplanar.cginc
            float3 triplanarNormal(float3 worldPos, float3 normal, float scale, sampler2D normalMap)
            {
                float3 absNormal = abs(normal);
                float3 blendWeight = saturate(pow(normal, 4));
                blendWeight /= dot(blendWeight, 1);

                float2 uvX = worldPos.zy * scale;
	            float2 uvY = worldPos.xz * scale;
	            float2 uvZ = worldPos.xy * scale;

                float3 tangentNormalX = UnpackNormal(tex2D(normalMap, uvX));
	            float3 tangentNormalY = UnpackNormal(tex2D(normalMap, uvY));
	            float3 tangentNormalZ = UnpackNormal(tex2D(normalMap, uvZ));

                tangentNormalX = blend_rnm(half3(normal.zy, absNormal.x), tangentNormalX);
	            tangentNormalY = blend_rnm(half3(normal.xz, absNormal.y), tangentNormalY);
	            tangentNormalZ = blend_rnm(half3(normal.xy, absNormal.z), tangentNormalZ);

                float3 axisSign = sign(normal);
	            tangentNormalX.z *= axisSign.x;
	            tangentNormalY.z *= axisSign.y;
	            tangentNormalZ.z *= axisSign.z;

                float3 outputNormal = normalize(
		            tangentNormalX.zyx * blendWeight.x +
		            tangentNormalY.xzy * blendWeight.y +
		            tangentNormalZ.xyz * blendWeight.z
	            );

                return outputNormal;
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

            fixed3 calculateNormalTex(float3 worldPos, float3 normalDir, fixed4 vertexColor)
            {
                fixed3 base = triplanarNormal(worldPos, normalDir, _BaseScale, _BaseNormal);
                fixed3 detail1 = triplanarNormal(worldPos, normalDir, _DetailScale1, _DetailNormal1) * vertexColor.r;
                fixed3 detail2 = triplanarNormal(worldPos, normalDir, _DetailScale2, _DetailNormal2) * vertexColor.g;
                fixed3 detail3 = triplanarNormal(worldPos, normalDir, _DetailScale3, _DetailNormal3) * vertexColor.b;

                fixed3 detailCol = (detail1 + detail2 + detail3) / (vertexColor.r + vertexColor.g + vertexColor.b);
                detailCol = saturate(detailCol);
                float alpha = saturate(length(vertexColor.rgb));

                return lerp(base, detailCol, alpha).xyz;
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

            fixed getFogScalar(float3 posWorld)
            {
                float dist = length(posWorld - _WorldSpaceCameraPos);
                float scalar = _FogMaxDist * 50 / (dist * dist);
                scalar = 1 - saturate(scalar);

                return scalar;
            }

            float4 calculateLightFinal(float3 normalDirection, float3 posWorld, fixed4 vertexColor, float3 normalWorld)
            {
                //Vectors
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
				float3 lightDir;
                float atten;

				if(_WorldSpaceLightPos0.w == 0.0) //Directional light
				{
					atten = _NightMultiplier;
					lightDir = normalize(_WorldSpaceLightPos0.xyz);
				}
				else //Point light or spot light
				{
                    float4 properties = calculateLightProperties(posWorld);
                    lightDir = properties.xyz;
                    atten = properties.w;                    
				}

                // Calcuate normal direction
                float3 newNormalDirection = calculateNormalTex(posWorld, normalWorld, vertexColor);
                newNormalDirection = lerp(normalWorld, newNormalDirection, _BumpStrength);

				//Lighting
				float3 lightingModel = max(0.0, dot(newNormalDirection, lightDir));
				float3 diffuseReflection = atten * _LightColor0.xyz * lightingModel;

				float3 specularNoShiny = atten * max(0.0, dot(reflect(-lightDir, newNormalDirection), viewDir)) * lightingModel;
				float3 specularWithColor = pow(specularNoShiny, _Shininess) * _SpecColor.rgb;
				
				//Rim lighting
				float rim = 1 - saturate(dot(viewDir, newNormalDirection));
				float3 rimLighting = saturate(dot(newNormalDirection, lightDir)) * pow(rim, 10 - _RimPower) * _RimColor * atten * _LightColor0.xyz;

				float3 lightFinal = rimLighting + diffuseReflection + specularWithColor;

				return float4(lightFinal, 1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 lightFinal = calculateLightFinal(i.normalWorld, i.worldPos, i.color, i.normalWorld);
                fixed4 finalColor = calculateBaseColor(i.worldPos, i.normalWorld, i.color) * lightFinal;

                return lerp(finalColor, _FogColor, getFogScalar(i.worldPos));;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
