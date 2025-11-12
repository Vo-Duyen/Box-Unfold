
Shader "Custom/Masker"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Transparent+200" }


        Pass
        {
            Name "Mask"

            Blend Zero One
        }
    }
}
