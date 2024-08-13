Shader "Indigocoder/LightDistortion"
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

            float3 _OvoidCenter;
            float3 _OvoidRadii;
            fixed4 _Color;
            fixed4 _DistortionColor;
            float _DistortionAmplitude;
            float _Multiplier;
            float _EffectBoundaryMax;
            float _EffectBoundaryMin;
            float _BoundaryOffset;

            float4x4 _InverseRotationMatrix;

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

            //Returns (distToSphere, distThroughSphere). If ray misses box, distThroughSphere will be zero
            float3 rayOvoid(float3 center, float3 radii, float3 rayOrigin, float3 rayDir, float4x4 invRotationMatrix)
            {
                float3 localRayOrigin = rayOrigin - center;
                float3 localRayDir = rayDir;

                float3 transformOrigin = mul(float4(localRayOrigin, 1), invRotationMatrix).xyz;
                float3 transformRayDir = mul(float4(localRayDir, 0), invRotationMatrix).xyz;

                float3 rotatedRayOrigin = transformOrigin.xyz;
                float3 rotatedRayDir = transformRayDir.xyz;

                float3 originCenter = rotatedRayOrigin / radii;
                float3 dir = rotatedRayDir / radii;

                float a = dot(dir, dir);
                float b = 2 * dot(originCenter, dir);
                float c = dot(originCenter, originCenter) - 1;

                float d = b * b - 4 * a * c;

                if(d >= 0)
                {
                    float s = sqrt(d);
                    float dstNear = (-b - s) / (2 * a);
                    float dstFar = (-b + s) / (2 * a);

                    return float3(dstNear, dstFar, dstFar - dstNear);
                }

                return float3(9999999, 9999999, 0);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                //Create ray
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);

                float3 rayOvoidInfo = rayOvoid(_OvoidCenter, _OvoidRadii, rayOrigin, rayDir, _InverseRotationMatrix);
                float distToSphere = rayOvoidInfo.x;
                float distInsideSphere = rayOvoidInfo.z;

                float nonLinearDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = LinearEyeDepth(nonLinearDepth) * length(i.viewVector);

                fixed4 col = tex2D(_MainTex, i.uv);

                bool hitFromOutside = distToSphere >= 0 && distInsideSphere > 0;
                bool hitFromInside = distToSphere < 0 && distInsideSphere > 0;   

                bool hitBounds = ((hitFromOutside || hitFromInside) && distToSphere < depth);
                if(!hitBounds)
                {
                    return col; 
                }

                if(rayOvoidInfo.x < 0 && rayOvoidInfo.y < 0) return col;

                float maxDist = max(_OvoidRadii.x, max(_OvoidRadii.y, _OvoidRadii.z));
                float normalizedDist = (distInsideSphere / (maxDist * _Multiplier));

                if(normalizedDist < _EffectBoundaryMax && normalizedDist > _EffectBoundaryMin)
                {
                    float3 dir = normalize(i.worldPos - _OvoidCenter + 1e-5);

                    float distortionStrength = _DistortionAmplitude * (1 - normalizedDist / _EffectBoundaryMax);
                    distortionStrength = clamp(distortionStrength, 0, _DistortionAmplitude);

                    float viewAttenuation = dot(normalize(i.viewVector), normalize(_OvoidCenter - _WorldSpaceCameraPos));
                    viewAttenuation = clamp(abs(viewAttenuation), 0.1, 1.0);
                    distortionStrength *= viewAttenuation;

                    float3 hitPoint = rayOrigin + rayDir * distToSphere;
                    float centerProximity = length(hitPoint - _OvoidCenter);

                    float3 dirToCenter = normalize(hitPoint - _OvoidCenter);

                    float2 distortion = dir.xy * clamp(distortionStrength, 0, _DistortionAmplitude);

                    float2 uv = i.uv + distortion;
                    fixed4 warpedCol = tex2D(_MainTex, uv) * _DistortionColor;
                    warpedCol = clamp(warpedCol, 0.0, 1.0);

                    fixed4 cutoffCol = lerp(col, warpedCol, length(warpedCol.rgb) + _BoundaryOffset);
                    cutoffCol = clamp(cutoffCol, 0, 1);

                    return fixed4(cutoffCol.rgb, 1);
                }
                else if(normalizedDist < _EffectBoundaryMax)
                {
                    return col;
                }
                else
                {
                    fixed4 newCol = fixed4(_Color.rgb * normalizedDist, _Color.a);
                    return fixed4((col.rgb * newCol.rgb), col.a);
                }
            }
            ENDCG
        }
    }
}
