Shader "Custom/Unlit_SphereInside" {
    Properties{
        _MainTex("Base (RGB)", 2D) = "gray" {}
    }
        SubShader{
            Tags {"RenderType" = "Opaque"}
            LOD 100

            Pass {
                Lighting Off
                Cull Front // 剔除球外表面的渲染
                SetTexture[_MainTex] { combine texture }
            }
    }
}
