// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BunjesFX/Circle_Gradient"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _GradientMultiply("Gradient Multiply", Range( 0 , 10)) = 1.425
        _GradientPower("Gradient Power", Range( 0 , 1)) = 0.85
        _Opacity("Opacity", Range( 0 , 1)) = 1
        _InnerRingColor("Inner Ring Color", Color) = (0.8113208,0.8113208,0.8113208,0)
        _InnerRingPadding("Inner Ring Padding", Float) = 0.98
        _InnerRingWidth("Inner Ring Width", Float) = 0.01

    }

    SubShader
    {
		LOD 0

        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

        Stencil
        {
        	Ref [_Stencil]
        	ReadMask [_StencilReadMask]
        	WriteMask [_StencilWriteMask]
        	Comp [_StencilComp]
        	Pass [_StencilOp]
        }


        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        
        Pass
        {
            Name "Default"
        CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
                
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            uniform float _GradientMultiply;
            uniform float _GradientPower;
            uniform float4 _InnerRingColor;
            uniform float _InnerRingPadding;
            uniform float _InnerRingWidth;
            uniform float _Opacity;

            
            v2f vert(appdata_t v )
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                

                v.vertex.xyz +=  float3( 0, 0, 0 ) ;

                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = v.texcoord;
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN ) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision)*invAlphaPrecision;

                float4 color12 = IsGammaSpace() ? float4(0.1686275,0.1686275,0.1686275,0) : float4(0.02415765,0.02415765,0.02415765,0);
                float4 color13 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
                float2 CenteredUV15_g2 = ( IN.texcoord.xy - float2( 0.5,0.5 ) );
                float2 break17_g2 = CenteredUV15_g2;
                float2 appendResult23_g2 = (float2(( length( CenteredUV15_g2 ) * 0.55 * 2.0 ) , ( atan2( break17_g2.x , break17_g2.y ) * ( 1.0 / 6.28318548202515 ) * 1.0 )));
                float4 lerpResult11 = lerp( color12 , color13 , saturate( pow( ( appendResult23_g2.x * _GradientMultiply ) , _GradientPower ) ));
                float2 appendResult11_g4 = (float2(_InnerRingPadding , _InnerRingPadding));
                float temp_output_17_0_g4 = length( ( (IN.texcoord.xy*2.0 + -1.0) / appendResult11_g4 ) );
                float temp_output_26_0 = ( _InnerRingPadding - _InnerRingWidth );
                float2 appendResult11_g5 = (float2(temp_output_26_0 , temp_output_26_0));
                float temp_output_17_0_g5 = length( ( (IN.texcoord.xy*2.0 + -1.0) / appendResult11_g5 ) );
                float4 lerpResult30 = lerp( lerpResult11 , _InnerRingColor , ( saturate( ( ( 1.0 - temp_output_17_0_g4 ) / fwidth( temp_output_17_0_g4 ) ) ) - saturate( ( ( 1.0 - temp_output_17_0_g5 ) / fwidth( temp_output_17_0_g5 ) ) ) ));
                float4 break17 = lerpResult30;
                float2 appendResult11_g3 = (float2(1.0 , 1.0));
                float temp_output_17_0_g3 = length( ( (IN.texcoord.xy*2.0 + -1.0) / appendResult11_g3 ) );
                float4 appendResult16 = (float4(break17.r , break17.g , break17.b , ( saturate( ( ( 1.0 - temp_output_17_0_g3 ) / fwidth( temp_output_17_0_g3 ) ) ) * _Opacity )));
                

                half4 color = appendResult16;

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
    CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.FunctionNode;2;-641,333.5;Inherit;True;Polar Coordinates;-1;;2;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;3;-300,386.5;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-130,469.5;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;7;89,507.5;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;8;300,524.5;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-453,637.5;Inherit;False;Property;_GradientMultiply;Gradient Multiply;0;0;Create;True;0;0;0;False;0;False;1.425;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-150,643.5;Inherit;False;Property;_GradientPower;Gradient Power;1;0;Create;True;0;0;0;False;0;False;0.85;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-809,431.5;Inherit;False;Constant;_Float5;Float 5;2;0;Create;True;0;0;0;False;0;False;0.55;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;11;559,436.5;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;12;276,40.5;Inherit;False;Constant;_ColorInner;Color Inner;2;0;Create;True;0;0;0;False;0;False;0.1686275,0.1686275,0.1686275,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;13;260,233.5;Inherit;False;Constant;_ColorOuter;Color Outer;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;17;950.939,433.1609;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;22;875.7228,1003.055;Inherit;False;Property;_Opacity;Opacity;2;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1;852.014,708.8983;Inherit;True;Ellipse;-1;;3;3ba94b7b3cfd5f447befde8107c04d52;0;3;2;FLOAT2;0,0;False;7;FLOAT;0.5;False;9;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;674.0141,795.8983;Inherit;False;Constant;_Float4;Float 4;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1720.397,482.6857;Float;False;True;-1;2;ASEMaterialInspector;0;3;BunjesFX/Circle_Gradient;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;True;2;5;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.DynamicAppendNode;16;1446.589,445.1941;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;1243.723,750.0545;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;23;986.2744,1130.526;Inherit;True;Ellipse;-1;;4;3ba94b7b3cfd5f447befde8107c04d52;0;3;2;FLOAT2;0,0;False;7;FLOAT;0.5;False;9;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;25;1019.274,1383.026;Inherit;True;Ellipse;-1;;5;3ba94b7b3cfd5f447befde8107c04d52;0;3;2;FLOAT2;0,0;False;7;FLOAT;0.5;False;9;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;26;860.2744,1310.026;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;28;1299.274,1132.026;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;30;1648.158,802.4809;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;24;694.2745,1215.526;Inherit;False;Property;_InnerRingPadding;Inner Ring Padding;4;0;Create;True;0;0;0;False;0;False;0.98;0.98;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;701.2744,1372.026;Inherit;False;Property;_InnerRingWidth;Inner Ring Width;5;0;Create;True;0;0;0;False;0;False;0.01;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;29;1888.158,907.4809;Inherit;False;Property;_InnerRingColor;Inner Ring Color;3;0;Create;True;0;0;0;False;0;False;0.8113208,0.8113208,0.8113208,0;0.8113208,0.8113208,0.8113208,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;2;3;15;0
WireConnection;3;0;2;0
WireConnection;6;0;3;0
WireConnection;6;1;9;0
WireConnection;7;0;6;0
WireConnection;7;1;10;0
WireConnection;8;0;7;0
WireConnection;11;0;12;0
WireConnection;11;1;13;0
WireConnection;11;2;8;0
WireConnection;17;0;30;0
WireConnection;1;7;14;0
WireConnection;1;9;14;0
WireConnection;0;0;16;0
WireConnection;16;0;17;0
WireConnection;16;1;17;1
WireConnection;16;2;17;2
WireConnection;16;3;20;0
WireConnection;20;0;1;0
WireConnection;20;1;22;0
WireConnection;23;7;24;0
WireConnection;23;9;24;0
WireConnection;25;7;26;0
WireConnection;25;9;26;0
WireConnection;26;0;24;0
WireConnection;26;1;27;0
WireConnection;28;0;23;0
WireConnection;28;1;25;0
WireConnection;30;0;11;0
WireConnection;30;1;29;0
WireConnection;30;2;28;0
ASEEND*/
//CHKSM=3B31CBBE2542F3DD44163C6F4E34D94569C7C188