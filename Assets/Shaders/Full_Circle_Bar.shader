// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BunjesFX/Full_Circle_Bar"
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
        _InnerRingColor("Inner Ring Color", Color) = (0.8113208,0.8113208,0.8113208,0)
        _FillingAmount("FillingAmount", Range( 0 , 1)) = 0.5280819
        _ColorBackground("Color Background", Color) = (0.3113208,0.3113208,0.3113208,0)
        _PaddingBackground("Padding Background", Float) = 0.1
        _CenterPadding("Center Padding", Float) = 0.05
        _InnerRingPadding("Inner Ring Padding", Float) = 0.05
        _InnerRingSize("Inner Ring Size", Float) = 0.025
        _Opacity("Opacity", Range( 0 , 1)) = 1

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

            uniform float _PaddingBackground;
            uniform float _FillingAmount;
            uniform float4 _ColorBackground;
            uniform float _GradientMultiply;
            uniform float _GradientPower;
            uniform float _CenterPadding;
            uniform float4 _InnerRingColor;
            uniform float _InnerRingPadding;
            uniform float _InnerRingSize;
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

                float4 color31 = IsGammaSpace() ? float4(0.9339623,0.5247396,0.08370414,0) : float4(0.8562991,0.2376585,0.007677305,0);
                float4 COLOR_BAR103 = color31;
                float2 appendResult11_g3 = (float2(1.0 , 1.0));
                float temp_output_17_0_g3 = length( ( (IN.texcoord.xy*2.0 + -1.0) / appendResult11_g3 ) );
                float temp_output_27_0 = ( 1.0 - _PaddingBackground );
                float2 appendResult11_g6 = (float2(temp_output_27_0 , temp_output_27_0));
                float temp_output_17_0_g6 = length( ( (IN.texcoord.xy*2.0 + -1.0) / appendResult11_g6 ) );
                float2 texCoord54 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
                float2 temp_cast_0 = (0.5).xx;
                float cos53 = cos( ( -1.0 * UNITY_PI ) );
                float sin53 = sin( ( -1.0 * UNITY_PI ) );
                float2 rotator53 = mul( texCoord54 - temp_cast_0 , float2x2( cos53 , -sin53 , sin53 , cos53 )) + temp_cast_0;
                float2 CenteredUV15_g7 = ( rotator53 - float2( 0.5,0.5 ) );
                float2 break17_g7 = CenteredUV15_g7;
                float2 appendResult23_g7 = (float2(( length( CenteredUV15_g7 ) * 1.0 * 2.0 ) , ( atan2( break17_g7.x , break17_g7.y ) * ( 1.0 / 6.28318548202515 ) * 1.0 )));
                float MASK_FILLING97 = step( appendResult23_g7.y , ( _FillingAmount - 0.5 ) );
                float temp_output_75_0 = ( ( saturate( ( ( 1.0 - temp_output_17_0_g3 ) / fwidth( temp_output_17_0_g3 ) ) ) - saturate( ( ( 1.0 - temp_output_17_0_g6 ) / fwidth( temp_output_17_0_g6 ) ) ) ) * MASK_FILLING97 );
                float temp_output_72_0 = ( 1.0 - ( _PaddingBackground / 2.0 ) );
                float2 appendResult11_g8 = (float2(temp_output_72_0 , temp_output_72_0));
                float temp_output_17_0_g8 = length( ( (IN.texcoord.xy*2.0 + -1.0) / appendResult11_g8 ) );
                float MASK_BACKGROUND112 = saturate( ( ( 1.0 - temp_output_17_0_g8 ) / fwidth( temp_output_17_0_g8 ) ) );
                float4 lerpResult73 = lerp( ( COLOR_BAR103 * temp_output_75_0 ) , _ColorBackground , MASK_BACKGROUND112);
                float MASK_BAR105 = temp_output_75_0;
                float4 lerpResult78 = lerp( lerpResult73 , COLOR_BAR103 , MASK_BAR105);
                float4 color10 = IsGammaSpace() ? float4(0.1686275,0.1686275,0.1686275,0) : float4(0.02415765,0.02415765,0.02415765,0);
                float4 color11 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
                float2 CenteredUV15_g2 = ( IN.texcoord.xy - float2( 0.5,0.5 ) );
                float2 break17_g2 = CenteredUV15_g2;
                float2 appendResult23_g2 = (float2(( length( CenteredUV15_g2 ) * 0.55 * 2.0 ) , ( atan2( break17_g2.x , break17_g2.y ) * ( 1.0 / 6.28318548202515 ) * 1.0 )));
                float4 lerpResult9 = lerp( color10 , color11 , saturate( pow( ( appendResult23_g2.x * _GradientMultiply ) , _GradientPower ) ));
                float4 COLOR_INNER_GRADIENT93 = lerpResult9;
                float temp_output_80_0 = ( temp_output_72_0 - _CenterPadding );
                float2 appendResult11_g10 = (float2(temp_output_80_0 , temp_output_80_0));
                float temp_output_17_0_g10 = length( ( (IN.texcoord.xy*2.0 + -1.0) / appendResult11_g10 ) );
                float4 lerpResult82 = lerp( lerpResult78 , COLOR_INNER_GRADIENT93 , saturate( ( ( 1.0 - temp_output_17_0_g10 ) / fwidth( temp_output_17_0_g10 ) ) ));
                float temp_output_84_0 = ( temp_output_80_0 - _InnerRingPadding );
                float2 appendResult11_g11 = (float2(temp_output_84_0 , temp_output_84_0));
                float temp_output_17_0_g11 = length( ( (IN.texcoord.xy*2.0 + -1.0) / appendResult11_g11 ) );
                float temp_output_87_0 = ( temp_output_84_0 - _InnerRingSize );
                float2 appendResult11_g12 = (float2(temp_output_87_0 , temp_output_87_0));
                float temp_output_17_0_g12 = length( ( (IN.texcoord.xy*2.0 + -1.0) / appendResult11_g12 ) );
                float4 lerpResult90 = lerp( lerpResult82 , _InnerRingColor , ( saturate( ( ( 1.0 - temp_output_17_0_g11 ) / fwidth( temp_output_17_0_g11 ) ) ) - saturate( ( ( 1.0 - temp_output_17_0_g12 ) / fwidth( temp_output_17_0_g12 ) ) ) ));
                float4 break107 = lerpResult90;
                float4 appendResult108 = (float4(break107.r , break107.g , break107.b , ( saturate( ( MASK_BAR105 + MASK_BACKGROUND112 ) ) * _Opacity )));
                

                half4 color = appendResult108;

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
Node;AmplifyShaderEditor.CommentaryNode;98;-4109.8,1453.847;Inherit;False;1704.568;468.0509;Mask Filling;12;58;36;38;97;54;55;57;53;37;41;96;95;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;92;-3277.972,1983.11;Inherit;False;2017.291;795.5121;Inner Gradient;12;93;1;2;11;10;9;8;7;6;5;4;3;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;91;-2366.895,1359.984;Inherit;False;1105.096;573.0872;Inner Ring (Bright);8;25;89;88;85;87;84;86;83;;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;14;-2527.498,280.5514;Inherit;False;Ellipse;-1;;3;3ba94b7b3cfd5f447befde8107c04d52;0;3;2;FLOAT2;0,0;False;7;FLOAT;0.5;False;9;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-3143.432,363.9957;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;70;-2956.56,711.047;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-3147.56,777.047;Inherit;False;Constant;_Float8;Float 8;3;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;72;-2799.56,627.047;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;59;-2495.228,776.1135;Inherit;False;Ellipse;-1;;8;3ba94b7b3cfd5f447befde8107c04d52;0;3;2;FLOAT2;0,0;False;7;FLOAT;0.5;False;9;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;80;-2607.348,1046.76;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;83;-1931.395,1409.984;Inherit;True;Ellipse;-1;;11;3ba94b7b3cfd5f447befde8107c04d52;0;3;2;FLOAT2;0,0;False;7;FLOAT;0.5;False;9;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;86;-1844.727,1670.969;Inherit;True;Ellipse;-1;;12;3ba94b7b3cfd5f447befde8107c04d52;0;3;2;FLOAT2;0,0;False;7;FLOAT;0.5;False;9;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;84;-2140.395,1450.984;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;87;-2026.727,1645.969;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;89;-1532.11,1667.51;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-2316.895,1518.484;Inherit;False;Property;_InnerRingPadding;Inner Ring Padding;7;0;Create;True;0;0;0;False;0;False;0.05;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;88;-2227.727,1691.969;Inherit;False;Property;_InnerRingSize;Inner Ring Size;8;0;Create;True;0;0;0;False;0;False;0.025;0.025;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-3167.432,591.9963;Inherit;False;Property;_PaddingBackground;Padding Background;5;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-4059.8,1715.85;Inherit;False;Constant;_Float7;Float 7;3;0;Create;True;0;0;0;False;0;False;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;36;-3076.311,1504.249;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.StepOpNode;38;-2885.302,1529.874;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;97;-2629.23,1524.32;Inherit;False;MASK_FILLING;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;54;-3947.31,1506.993;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;55;-3874.352,1637.193;Inherit;False;Constant;_Float6;Float 6;3;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;57;-3912.945,1721.71;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;53;-3637.169,1505.572;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;37;-3416.781,1503.847;Inherit;True;Polar Coordinates;-1;;7;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-3398.418,1721.858;Inherit;False;Property;_FillingAmount;FillingAmount;3;0;Create;True;0;0;0;False;0;False;0.5280819;0.2647059;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-3269.763,1805.897;Inherit;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;95;-3094.262,1727.597;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;-2234.453,623.741;Inherit;False;97;MASK_FILLING;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;31;-2295.087,200.7396;Inherit;False;Constant;_ColorBar;Color Bar;3;0;Create;True;0;0;0;False;0;False;0.9339623,0.5247396,0.08370414,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;33;-1958.669,622.0042;Inherit;False;Property;_ColorBackground;Color Background;4;0;Create;True;0;0;0;False;0;False;0.3113208,0.3113208,0.3113208,0;0.3113208,0.3113208,0.3113208,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;104;-1667.567,854.7935;Inherit;False;103;COLOR_BAR;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;78;-1316.029,838.2997;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;106;-1550.567,962.7935;Inherit;False;105;MASK_BAR;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;73;-1491.56,735.047;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;105;-1859.567,537.7935;Inherit;False;MASK_BAR;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;79;-2009.069,1142.536;Inherit;False;Ellipse;-1;;10;3ba94b7b3cfd5f447befde8107c04d52;0;3;2;FLOAT2;0,0;False;7;FLOAT;0.5;False;9;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;94;-1335.149,1073.634;Inherit;False;93;COLOR_INNER_GRADIENT;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;82;-983.5457,1089.289;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-2817.678,1118.007;Inherit;False;Property;_CenterPadding;Center Padding;6;0;Create;True;0;0;0;False;0;False;0.05;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;90;-499.1085,1636.026;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-2548.972,2462.11;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;4;-2329.973,2500.11;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;5;-2118.973,2517.11;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2871.972,2630.11;Inherit;False;Property;_GradientMultiply;Gradient Multiply;0;0;Create;True;0;0;0;False;0;False;1.425;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-2568.972,2636.11;Inherit;False;Property;_GradientPower;Gradient Power;1;0;Create;True;0;0;0;False;0;False;0.85;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-3227.972,2424.11;Inherit;False;Constant;_Float5;Float 5;2;0;Create;True;0;0;0;False;0;False;0.55;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;9;-1859.972,2429.11;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;10;-2142.973,2033.11;Inherit;False;Constant;_ColorInner;Color Inner;2;0;Create;True;0;0;0;False;0;False;0.1686275,0.1686275,0.1686275,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;-2158.973,2226.11;Inherit;False;Constant;_ColorOuter;Color Outer;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;2;-2718.972,2379.11;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.FunctionNode;1;-3059.972,2324.988;Inherit;True;Polar Coordinates;-1;;2;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;93;-1550.172,2426.691;Inherit;False;COLOR_INNER_GRADIENT;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;107;-214.1212,1785.473;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;108;140.8788,1804.473;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;109;-906.3696,1886.638;Inherit;True;105;MASK_BAR;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;111;-932.8043,2113.583;Inherit;False;112;MASK_BACKGROUND;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;113;-670.7744,2008.28;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;114;-389.593,2057.487;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;303.5871,1811.644;Float;False;True;-1;2;ASEMaterialInspector;0;3;BunjesFX/Full_Circle_Bar;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;True;2;5;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;112;-2257.563,771.7575;Inherit;False;MASK_BACKGROUND;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;27;-2685.989,506.8394;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;29;-2531.023,478.6588;Inherit;False;Ellipse;-1;;6;3ba94b7b3cfd5f447befde8107c04d52;0;3;2;FLOAT2;0,0;False;7;FLOAT;0.5;False;9;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;69;-2255.56,454.047;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-2019.473,453.5475;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-1851.029,352.2997;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;103;-2053.567,346.7935;Inherit;False;COLOR_BAR;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;25;-1525.106,1461.873;Inherit;False;Property;_InnerRingColor;Inner Ring Color;2;0;Create;True;0;0;0;False;0;False;0.8113208,0.8113208,0.8113208,0;0.8113208,0.8113208,0.8113208,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;-75.6908,2118.807;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-442.6908,2198.807;Inherit;False;Property;_Opacity;Opacity;9;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
WireConnection;14;7;26;0
WireConnection;14;9;26;0
WireConnection;70;0;28;0
WireConnection;70;1;71;0
WireConnection;72;0;26;0
WireConnection;72;1;70;0
WireConnection;59;7;72;0
WireConnection;59;9;72;0
WireConnection;80;0;72;0
WireConnection;80;1;81;0
WireConnection;83;7;84;0
WireConnection;83;9;84;0
WireConnection;86;7;87;0
WireConnection;86;9;87;0
WireConnection;84;0;80;0
WireConnection;84;1;85;0
WireConnection;87;0;84;0
WireConnection;87;1;88;0
WireConnection;89;0;83;0
WireConnection;89;1;86;0
WireConnection;36;0;37;0
WireConnection;38;0;36;1
WireConnection;38;1;95;0
WireConnection;97;0;38;0
WireConnection;57;0;58;0
WireConnection;53;0;54;0
WireConnection;53;1;55;0
WireConnection;53;2;57;0
WireConnection;37;1;53;0
WireConnection;95;0;41;0
WireConnection;95;1;96;0
WireConnection;78;0;73;0
WireConnection;78;1;104;0
WireConnection;78;2;106;0
WireConnection;73;0;77;0
WireConnection;73;1;33;0
WireConnection;73;2;112;0
WireConnection;105;0;75;0
WireConnection;79;7;80;0
WireConnection;79;9;80;0
WireConnection;82;0;78;0
WireConnection;82;1;94;0
WireConnection;82;2;79;0
WireConnection;90;0;82;0
WireConnection;90;1;25;0
WireConnection;90;2;89;0
WireConnection;3;0;2;0
WireConnection;3;1;6;0
WireConnection;4;0;3;0
WireConnection;4;1;7;0
WireConnection;5;0;4;0
WireConnection;9;0;10;0
WireConnection;9;1;11;0
WireConnection;9;2;5;0
WireConnection;2;0;1;0
WireConnection;1;3;8;0
WireConnection;93;0;9;0
WireConnection;107;0;90;0
WireConnection;108;0;107;0
WireConnection;108;1;107;1
WireConnection;108;2;107;2
WireConnection;108;3;115;0
WireConnection;113;0;109;0
WireConnection;113;1;111;0
WireConnection;114;0;113;0
WireConnection;0;0;108;0
WireConnection;112;0;59;0
WireConnection;27;0;26;0
WireConnection;27;1;28;0
WireConnection;29;7;27;0
WireConnection;29;9;27;0
WireConnection;69;0;14;0
WireConnection;69;1;29;0
WireConnection;75;0;69;0
WireConnection;75;1;99;0
WireConnection;77;0;103;0
WireConnection;77;1;75;0
WireConnection;103;0;31;0
WireConnection;115;0;114;0
WireConnection;115;1;116;0
ASEEND*/
//CHKSM=BDAD36F6D420C90024195BE938ED1633A6B8869D