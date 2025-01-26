Shader "Unlit/UIFalloffMonodirectional"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _TargetCenter ("Opacity Center", Float) = 0.5
        _Falloff ("Falloff", Range(0,  0.5)) = 0
        _Intensity ("Intensity", Float) = 1
        [Toggle] _FlipDir ("Flip Direction", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        ZWrite Off

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

            fixed4 _Color;
            fixed _TargetCenter;
            float _Intensity;
            half _Falloff;
            fixed _FlipDir;

            float invLerp(float from, float to, float value)
            {
                return (value - from) / (to - from);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float uv = abs(_FlipDir - i.uv.y);
                float step = smoothstep(0, 1, invLerp(0, _TargetCenter - (0.5 - _Falloff), uv));

                return _Color * _Intensity * step;
            }
            ENDCG
        }
    }
}
