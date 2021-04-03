Shader "Custom/RimLit" {
	Properties{
	  _RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
	  _RimPower("Rim Power", Range(0.0,8.0)) = 3.0
	  _MainColor("Main Color", Color) = (0.0,0.0,0.0,0.0)
	}
		SubShader{
		  Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		  CGPROGRAM
		  #pragma surface surf Lambert alpha
		  struct Input {
			  float3 viewDir;
		  };
		  float4 _RimColor;
		  float _RimPower;
		  float4 _MainColor;
		  void surf(Input IN, inout SurfaceOutput o) {
			  //o.Albedo = _MainColor.rgb;
			  o.Alpha = _MainColor.a;
			  half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			  o.Albedo = _RimColor.rgb * pow(rim, _RimPower);
			  o.Alpha = _RimColor.a * pow(rim, _RimPower);
		  }
		  ENDCG
	  }
		  Fallback "Diffuse"
}
