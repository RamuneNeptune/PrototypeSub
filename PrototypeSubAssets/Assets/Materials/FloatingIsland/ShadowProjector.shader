Shader "Unlit/ShadowProjector"
{
    Properties
    {

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
