Shader "CPX_Custom/Mobile/Unlit Color GLSL"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            GLSLPROGRAM
            #ifdef VERTEX
            uniform mat4 unity_MatrixMVP;
            attribute vec4 _glesVertex;
            attribute vec2 _glesMultiTexCoord0;
            uniform vec4 _MainTex_ST;

            varying vec2 uv;

            void main()
            {
                uv = _glesMultiTexCoord0 * _MainTex_ST.xy + _MainTex_ST.zw;
                gl_Position = unity_MatrixMVP * _glesVertex;
            }
            #endif

            #ifdef FRAGMENT
            uniform sampler2D _MainTex;
            uniform vec4 _Color;
            varying vec2 uv;

            void main()
            {
                vec4 color = texture2D(_MainTex, uv);
                gl_FragColor = color * _Color;   // multiply by tint
            }
            #endif
            ENDGLSL
        }
    }
    Fallback "Unlit/Texture"
}
