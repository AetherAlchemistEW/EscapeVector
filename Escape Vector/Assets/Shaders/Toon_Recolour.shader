Shader "Toon/Simple Recolourable" 
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskTex1 ("Mask 1 (A)", 2D) = "white" {}
		_MaskTex2 ("Mask 2 (A)", 2D) = "white" {}
	    _Lighting ("Environment Map (Diffuse)", Cube) = "" {}
	    
	    _outlineColor 	("Outline (Color)", Color)  = (1.0, 1.0, 1.0, 1) // color
	    _baseColor 		("Base (Color)", Color)  	= (1.0, 1.0, 1.0, 1) // color
	    _detailColor1 	("Mask 1 (Color)", Color)  	= (1.0, 0.0, 0.0, 1) // color
	    _detailColor2 	("Mask 2 (Color)", Color)  	= (1.0, 0.0, 0.0, 1) // color
	    
	    _outlineScale ("Outline (Scale)", Range (0.0, 0.2)) = 0.2
	    
		//_rimValue ("Rim Light (Strength)", Range (0.1, 2.0)) = 1.0
		//_rimCutoff ("Rim Light (Cutoff)", Range (0.01, 1.0)) = 0.9
		//_EnviroScaleDiffuse ("Environment Scale (Diffuse)", Range (0.0, 2.0)) = 0.5
	}
	
	SubShader { 
		cull off
		
	    Pass {
			CGPROGRAM
			
				#pragma vertex vShader
				#pragma fragment pShader
				#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
				#pragma glsl
		 		#include "UnityCG.cginc"
		 		
		 		// Texturing
		 		uniform sampler2D _MainTex;
		 		uniform sampler2D _MaskTex1;
		 		uniform sampler2D _MaskTex2;
		 		
		 		uniform samplerCUBE _Lighting;
                                
                uniform float4 _baseColor;
                uniform float4 _detailColor1;
                uniform float4 _detailColor2;

                                                
				struct vOutput 
				{
					float4 position 	: POSITION;
					float2 uv			: TEXCOORD0;
					float3 cameraFwd	: TEXCOORD1;
					float3 normal		: TEXCOORD2;
					float3 localOffset	: TEXCOORD3;
				};
		 
				vOutput vShader(appdata_full v)
				{
					vOutput OUT;
					
					// Transform position to clip space:
					OUT.position = 	mul (UNITY_MATRIX_MVP, 	v.vertex);
					
					// Get the reflection vector for our cubemap:
					float3 positionWorld = 	mul (_Object2World, 			v.vertex).rgb;
					float3 normalWorld = 	mul ((float3x3)_Object2World, 	v.normal);
					
					normalWorld = normalWorld;
					
					OUT.cameraFwd = 	positionWorld - _WorldSpaceCameraPos;
					//OUT.cameraFwd = 	UNITY_MATRIX_V[2]; // This matrix added after this version of Unity :{
					
					OUT.normal = 		normalWorld;
					OUT.localOffset = 	positionWorld;
					
					// Get texture and lightmap UVS:					
					OUT.uv = 	v.texcoord;
										
					return OUT;
				}		
		 
				struct pOutput
				{
					float4 color : COLOR;
				};
		 
				pOutput pShader(vOutput IN)
				{
					pOutput OUT;
					
					float4 mainTex = 	tex2D( 		_MainTex, 		IN.uv );
					float  maskTex1 =	tex2D(		_MaskTex1,		IN.uv ).r;
					float  maskTex2 =	tex2D(		_MaskTex2,		IN.uv ).r;
					
					float4 colourMasked = _baseColor;
					colourMasked = lerp( colourMasked, _detailColor1,   maskTex1 );
					colourMasked = lerp( colourMasked, _detailColor2, maskTex2);
					colourMasked *= 2.0f;

					float3 sampleDiff = normalize(IN.normal);
					float4 cubeSample = texCUBE(_Lighting, sampleDiff);
					
					//float fresnel = 1.0f - abs( dot( normalize(IN.normal), -normalize(IN.cameraFwd) ));
					//float halfLambert = dot( IN.normal, _LightVector ) * 0.5f + 0.5f;
					//float4 rim = smoothstep( _rimCutoff - 0.05f, _rimCutoff + 0.05f, fresnel * halfLambert);
					//rim *= _rimValue * _baseColor;
					
					OUT.color = mainTex * colourMasked * cubeSample;
					
					return OUT;
				}				
			ENDCG
	    }  
	    
		cull front
		
	    Pass {
			CGPROGRAM
			
				#pragma vertex vShader
				#pragma fragment pShader
				#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
				#pragma glsl
		 		#include "UnityCG.cginc"
		 		
		 		// Texturing
		 		uniform sampler2D _MainTex;
		 		//uniform samplerCUBE _Cube;
		 		uniform samplerCUBE _Lighting;
                                
                uniform float4 _outlineColor;
                uniform float _EnviroScaleDiffuse;                
                                
                uniform float _outlineScale;
                                
				struct vOutput 
				{
					float4 position 	: POSITION;
					float2 uv			: TEXCOORD0;
					float3 cameraFwd	: TEXCOORD1;
					float3 normal		: TEXCOORD2;
					float3 localOffset	: TEXCOORD3;
				};
		 
				vOutput vShader(appdata_full v)
				{
					vOutput OUT;
					
					// Transform position to clip space:
					OUT.position = 	mul (UNITY_MATRIX_MVP, 	v.vertex + float4(v.normal.rgb, 0.0f) * _outlineScale );
					
					// Get the reflection vector for our cubemap:
					float3 positionWorld = 	mul (_Object2World, 			v.vertex).rgb;
					float3 normalWorld = 	mul ((float3x3)_Object2World, 	v.normal);
					normalWorld = normalize(normalWorld);
					
					OUT.cameraFwd = 	positionWorld - _WorldSpaceCameraPos;
					//OUT.cameraFwd = 	UNITY_MATRIX_V[2]; // This matrix added after this version of Unity :{
					
					OUT.normal = 		normalWorld;
					OUT.localOffset = 	positionWorld;
					
					// Get texture and lightmap UVS:					
					OUT.uv = 	v.texcoord;
										
					return OUT;
				}		
		 
				struct pOutput
				{
					float4 color : COLOR;
				};
		 
				pOutput pShader(vOutput IN)
				{
					pOutput OUT;
					
					// Outline - just sample the colour
					OUT.color = _outlineColor;
					
					return OUT;
				}				
			ENDCG
	    }  	    

	}
 
}
