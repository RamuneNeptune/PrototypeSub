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
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                //In OpenGL the forward vector of the camera is -1, as opposed to Unity's positive 1
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));
                return o;
            }

            //User defined variables

            sampler2D _MainTex; //<-- Set by the Graphics.Blit; Contains the scene as a render texture
            sampler2D _CameraDepthTexture;

            float3 SphereCenter;
            float SphereRadius;
            fixed4 _Color;
            float _Multiplier;

            //Returns (distToSphere, distThroughSphere). If ray misses box, distThroughSphere will be zero
            float2 raySphere(float3 sphereCenter, float sphereRadius, float3 rayOrigin, float3 rayDir)
            {
                float3 offset = rayOrigin - sphereCenter;
                float a = 1;
                float b = 2 * dot(offset, rayDir);
                float c = dot(offset, offset) - sphereRadius * sphereRadius;
                float d = b * b - 4 * a * c; // Discriminant from quadratic formula

                //Num intersections: 0 when d < 0; 1 when d = 0; 2 when d > 0;
                if(d > 0)
                {
                    float s = sqrt(d);
                    float dstToSphereNear = max(0, (-b - s) / (2 * a));
                    float dstToSphereFar = (-b + s) / (2 * a);

                    //Ignore intersections behind the ray
                    if(dstToSphereFar >= 0)
                    {
                        return float2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
                    }
                }

                return float2(9999999, 0);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                //Create ray
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);

                float2 raySphereInfo = raySphere(SphereCenter, SphereRadius, rayOrigin, rayDir);
                float distToSphere = raySphereInfo.x;
                float distInsideSphere = raySphereInfo.y;

                float nonLinearDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = LinearEyeDepth(nonLinearDepth) * length(i.viewVector);

                bool hitBounds = (distInsideSphere > 0 && distToSphere < depth);
                if(hitBounds)
                {
                    float normalizedDist = (distInsideSphere / (SphereRadius * _Multiplier));
                    fixed4 newCol = fixed4(_Color.rgb * normalizedDist, _Color.a);

                    col = fixed4((col.rgb * newCol.rgb), col.a);
                }

                return col;
            }
            ENDCG
        }
    }
}
