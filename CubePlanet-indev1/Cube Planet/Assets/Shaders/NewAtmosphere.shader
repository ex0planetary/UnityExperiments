Shader "Custom/NewAtmosphere" {
	Properties{
	  _Color("Color", Color) = (0.0,0.0,0.0,0.0)
	  _RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
	  _RimPower("Rim Power", Range(0.5,8.0)) = 3.0
	  _Glossiness("Smoothness", Range(0,1)) = 0.5
	  _Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader{
		  Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		  CGPROGRAM
		  #pragma surface surf Standard fullforwardshadows alpha
		  #pragma target 3.0
		  struct Input {
			  float3 viewDir;
		  };
		  float4 _Color;
		  float4 _RimColor;
		  float _RimPower;
		  half _Glossiness;
		  half _Metallic;
		  void surf(Input IN, inout SurfaceOutputStandard o) {
			  o.Albedo = _Color.rgb;
			  o.Alpha = _Color.a;
			  half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			  //half rim = saturate(dot(normalize(IN.viewDir), o.Normal));
			  //o.Emission = _RimColor.rgb * pow(rim, _RimPower);
			  o.Metallic = _Metallic;
			  o.Smoothness = _Glossiness;
			  o.Albedo *= _RimColor.rgb * pow(rim, _RimPower);
			  o.Alpha *= _RimColor.a * pow(rim, _RimPower);
		  }
		  ENDCG
	  }
		  Fallback "Diffuse"
}