Shader "Toon/Vertex Lighted Blend Outline" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (.002, 0.03)) = .005
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {} 
		_Blend ("Blend (RGB)", 2D) = "white" {}				
	}

	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Overlay"}
		UsePass "Mobile/VertexLit (Only Directional Lights) Blend/FORWARD"
		UsePass "Toon/Outline/OUTLINE"
	} 
	
	Fallback "Toon/Lighted"
}
