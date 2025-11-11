//
//  OutlineMask.shader
//  QuickOutline
//
//  Created by Chris Nolet on 2/21/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

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

            // Stencil
            // {
            //     Ref 1
            //     Comp Equal
            //     Fail Replace
            // }
        }
    }
}
