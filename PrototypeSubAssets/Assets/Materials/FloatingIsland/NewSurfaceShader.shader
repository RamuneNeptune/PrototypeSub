Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _SpecColor ("Specular Color", Color) = (1,1,1,1)
        _Shininess ("Smoothness", Range(0, 1)) = 0.5
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
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows
        
        #include "UnityPBSLighting.cginc"

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        half _Shininess;

        sampler2D _MainTex;
        sampler2D _BaseNormal;
        fixed4 _MainTex_ST;
        sampler2D _DetailTex1;
        sampler2D _DetailNormal1;
        sampler2D _DetailTex2;
        sampler2D _DetailNormal2;
        sampler2D _DetailTex3;
        sampler2D _DetailNormal3;

        half _BaseScale;
        half _DetailScale1;
        half _DetailScale2;
        half _DetailScale3;

        struct Input
        {
            fixed4 color : COLOR;
            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };
        
        inline half4 LightingStandardDefaultGI(SurfaceOutputStandardSpecular s, half3 viewDir, UnityGI gi)
        {
            return LightingStandardSpecular(s, viewDir, gi);
        }
    
        inline void LightingStandardDefaultGI_GI(SurfaceOutputStandardSpecular s, UnityGIInput data, inout UnityGI gi)
        {
            LightingStandardSpecular_GI(s, data, gi);
            //gi.indirect.diffuse = max(gi.indirect.diffuse, _AmbientColor);
            //gi.indirect.specular = max(gi.indirect.specular, _AmbientColor);
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        
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
            blendWeight /= max(dot(blendWeight, 1), 0.0001);

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

        float3 WorldToTangentNormalVector(Input IN, float3 normal)
        {
            float3 t2w0 = WorldNormalVector(IN, float3(1,0,0));
            float3 t2w1 = WorldNormalVector(IN, float3(0,1,0));
            float3 t2w2 = WorldNormalVector(IN, float3(0,0,1));
            float3x3 t2w = float3x3(t2w0, t2w1, t2w2);
            return normalize(mul(t2w, normal));
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

        float3 calculateNormalTex(float3 worldPos, float3 normalDir, fixed4 vertexColor)
        {
            float3 base = triplanarNormal(worldPos, normalDir, _BaseScale, _BaseNormal);
            float3 detail1 = triplanarNormal(worldPos, normalDir, _DetailScale1, _DetailNormal1) * vertexColor.r;
            float3 detail2 = triplanarNormal(worldPos, normalDir, _DetailScale2, _DetailNormal2) * vertexColor.g;
            float3 detail3 = triplanarNormal(worldPos, normalDir, _DetailScale3, _DetailNormal3) * vertexColor.b;

            float3 detailCol = (detail1 + detail2 + detail3) / (vertexColor.r + vertexColor.g + vertexColor.b);
            detailCol = clamp(-1, 1, detailCol);
            float alpha = length(vertexColor.rgb);

            return lerp(base, detailCol, alpha);
        }

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            IN.worldNormal = WorldNormalVector(IN, float3(0,0,1));
            float3 norm = calculateNormalTex(IN.worldPos, IN.worldNormal, IN.color);

            float3 tangentNorm = WorldToTangentNormalVector(IN, norm);
            o.Albedo = calculateBaseColor(IN.worldPos, norm, IN.color);
            o.Normal = tangentNorm;
            o.Specular = _SpecColor.rgb;
            o.Smoothness = _Shininess;
            o.Occlusion = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
