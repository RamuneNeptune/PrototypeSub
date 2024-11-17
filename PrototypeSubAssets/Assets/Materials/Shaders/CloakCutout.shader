Shader "Indigocoder/CloakCutout"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            //User defined variables

            sampler2D _MainTex; //<-- Set by the Graphics.Blit; Contains the scene as a render texture
            sampler2D _CameraDepthTexture;

            float3 _SphereCenter;
            float _SphereRadius;
            
            float3 _HexCenter;
            float _HexHeight;
            float _HexRadius;

            fixed4 _InteriorColor;
            fixed4 _DistortionColor;
            fixed4 _VignetteColor;

            float _DistortionAmplitude;
            float _Multiplier;
            float _EffectBoundaryMax;
            float _EffectBoundaryMin;
            float _BoundaryOffset;
            float _RippleFrequency;
            float _RippleAmplitude;

            float _VignetteIntensity;
            float _VignetteSmoothness;
            float _VignetteOffset;
            float _VignetteFadeInDist;

            float _OscillationFrequency;
            float _OscillationAmplitude;
            float _OscillationSpeed;
            int _WaveCount;
            float _AmplitudeFalloff;
            float _FrequencyIncrease;

            uniform float4x4 _HexRotationMatrix;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewVector : TEXCOORD1;
                float3 worldPos : TEXCOOLRD2;
            };

            v2f vert (appdata v)
            {
                v2f o;   

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                //In OpenGL the forward vector of the camera is -1, as opposed to Unity's positive 1
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));

                o.worldPos = mul(unity_ObjectToWorld, float4(v.uv, 0, 1)).xyz;

                return o;
            }   

            // Sphere of size `radius` centered at point `center`
            // Returns distNear, distFar. Returns -1 if no intersection
            // Taken from here: https://iquilezles.org/articles/intersectors/
            float2 sphIntersect(float3 rayOrigin, float3 rayDir, float3 center, float radius)
            {
                float3 oc = rayOrigin - center;
                float b = dot(oc, rayDir);
                float3 qc = oc - b * rayDir;
                float h = radius * radius - dot(qc, qc);
                if (h < 0) return -1.0; // No intersection
                h = sqrt(h);
                return float2(-b - h, -b + h);
            }

            float3 sphNormal(float3 pos, float3 center, float radius)
            {
                return float3(pos - center) / radius;    
            }

            // Also taken from https://iquilezles.org/articles/intersectors/
            // I assume t_ = touch _, so tF is touch far (intersection far) and tN is touch near (intersection near)
            // Returns (intersectionNear, intersectionFar). If no intersection, returns -1
            float2 iHexPrism(float3 rayOrigin, float3 rayDir, float3 center, float radius, float height, float4x4 rotMatrix)
            {
                const float ks3 = 0.866025;

                float3 localRayOrigin = rayOrigin - center;
                float3 localRayDir = rayDir;

                float3 rotatedOrigin = mul(float4(localRayOrigin, 1), rotMatrix).xyz;
                float3 rotatedDir = mul(float4(localRayDir, 0), rotMatrix).xyz;

                // normals
                const float3 n1 = float3(1.0, 0.0, 0.0);
                const float3 n2 = float3(0.5, 0.0, ks3);
                const float3 n3 = float3(-0.5, 0.0, ks3);
                const float3 n4 = float3(0.0, 1.0, 0.0);

                // slabs intersections
                float3 t1 = float3((float2(radius, -radius) - dot(rotatedOrigin, n1)) / dot(rotatedDir, n1), 1.0);
                float3 t2 = float3((float2(radius, -radius) - dot(rotatedOrigin, n2)) / dot(rotatedDir, n2), 1.0);
                float3 t3 = float3((float2(radius, -radius) - dot(rotatedOrigin, n3)) / dot(rotatedDir, n3), 1.0);
                float3 t4 = float3((float2(height, -height) - dot(rotatedOrigin, n4)) / dot(rotatedDir, n4), 1.0);
    
                // inetsection selection
                if(t1.y < t1.x) t1 = float3(t1.yx, -1.0);
                if(t2.y < t2.x) t2 = float3(t2.yx, -1.0);
                if(t3.y < t3.x) t3 = float3(t3.yx, -1.0);
                if(t4.y < t4.x) t4 = float3(t4.yx, -1.0);
   
                float4          tN = float4(t1.x, t1.z * n1);
                if(t2.x > tN.x) tN = float4(t2.x, t2.z * n2);
                if(t3.x > tN.x) tN = float4(t3.x, t3.z * n3);
                if(t4.x > tN.x) tN = float4(t4.x, t4.z * n4);
    
                float tF = min(min(t1.y, t2.y), min(t3.y, t4.y));
                
                // No intersection
                if(tN.x > tF || tF < 0.0) return -1;

                return float2(tN.x, tF);  // return tF too for exit point
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                //Create ray
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);

                float samplePosX = i.uv.x * _OscillationFrequency + _Time * _OscillationSpeed;
                float samplePosY = i.uv.y * _OscillationFrequency + _Time * _OscillationSpeed;

                float oscillationX = 0;
                float oscillationY = 0;

                for(int j = 0; j < _WaveCount; j++)
                {
                    oscillationX += sin(samplePosX + _FrequencyIncrease * j) * (_OscillationAmplitude * pow(_AmplitudeFalloff, j));
                    oscillationY += sin(samplePosY + _FrequencyIncrease * j) * (_OscillationAmplitude * pow(_AmplitudeFalloff, j));
                }

                float3 offsetDir = rayDir + float3(oscillationX, oscillationY, oscillationX);
                
                float nonLinearDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = LinearEyeDepth(nonLinearDepth) * length(i.viewVector);

                fixed4 originalCol = tex2D(_MainTex, i.uv);
                
                float2 sphereInfo = sphIntersect(rayOrigin, offsetDir, _SphereCenter, _SphereRadius);
                float distToSphere = sphereInfo.x;
                float distInsideSphere = sphereInfo.x > 0 ? (sphereInfo.y - sphereInfo.x) : -1;

                float atten = dot(rayOrigin - _SphereCenter, offsetDir);
                bool hitOutsideSphere = distToSphere >= 0 && distInsideSphere > 0;
                if (atten > 0 && hitOutsideSphere) return originalCol;

                float3 sphereNormal = sphNormal(rayOrigin + offsetDir * distToSphere, _SphereCenter, _SphereRadius);

                float2 hexInfo = iHexPrism(rayOrigin, rayDir, _HexCenter, _HexRadius, _HexHeight, _HexRotationMatrix);
                bool hitHex = hexInfo.y > 0;
                bool hitSphere = distToSphere >= 0 || sphereInfo.y > 0;

                if (hitHex && atten > 0)
                {
                    return originalCol;
                }

                if (!hitSphere || distToSphere >= depth)
                {
                    return originalCol;
                }

                float effectStrength = 1 - dot(sphereNormal, -rayDir);
                
                if (effectStrength < _EffectBoundaryMax && effectStrength > _EffectBoundaryMin)
                {
                    float3 dir = normalize(i.worldPos - _SphereCenter + 1e-5);

                    float distortionStrength = _DistortionAmplitude * (effectStrength / _EffectBoundaryMax);
                    distortionStrength = clamp(distortionStrength, 0, _DistortionAmplitude);

                    float viewAttenuation = dot(rayDir, normalize(_SphereCenter - _WorldSpaceCameraPos));
                    viewAttenuation = clamp(abs(viewAttenuation), 0.1, 1.0);
                    distortionStrength *= viewAttenuation;

                    float3 hitPoint = rayOrigin + rayDir * distToSphere;
                    float centerProximity = length(hitPoint - _SphereCenter);

                    float3 dirToCenter = normalize(hitPoint - _SphereCenter);

                    float2 distortion = dir.xy * clamp(distortionStrength, 0, _DistortionAmplitude);

                    float2 uv = i.uv + distortion;
                    fixed4 warpedCol = tex2D(_MainTex, uv) * _DistortionColor;
                    warpedCol = clamp(warpedCol, 0.0, 1.0);

                    fixed4 cutoffCol = lerp(originalCol, warpedCol, length(warpedCol.rgb) + _BoundaryOffset);
                    cutoffCol = clamp(cutoffCol, 0, 1);

                    return fixed4(cutoffCol.rgb, 1);
                }
                else if (effectStrength > _EffectBoundaryMax)
                {
                    return originalCol;
                }
                else
                {
                    //Vignette UV
                    float2 screenUV = i.uv - fixed2(0.5, 0.5);
                    float dist = length(screenUV) + _VignetteOffset;

                    //Dist to surface
                    float normalizedDistToSurf = clamp(distToSphere, 0, _VignetteFadeInDist) / _VignetteFadeInDist;

                    //Vignette values
                    float vignette = smoothstep(0.5, 0.5 - _VignetteSmoothness, dist);
                    float fadeFactor = 1 - smoothstep(0, 1, normalizedDistToSurf);
                    float vignetteVal = lerp(1, vignette, _VignetteIntensity * fadeFactor);

                    //Interior color
                    fixed4 newCol = fixed4(_InteriorColor.rgb * (1 - effectStrength), _InteriorColor.a);                    
                    fixed4 nonVignetteCol = fixed4((originalCol.rgb * newCol.rgb), originalCol.a);

                    //Vignette color
                    fixed4 vignetteCol = fixed4(_VignetteColor.rgb, nonVignetteCol.a);

                    //Vignette color lerp
                    fixed4 finalColor = lerp(vignetteCol * nonVignetteCol, nonVignetteCol, vignetteVal);

                    return finalColor;
                }
            }
            ENDCG
        }
    }
}
