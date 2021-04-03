Shader "Custom/ScatteredAtmosphere"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
		Cull Back
		Blend One One

        CGPROGRAM
		#pragma surface surf StandardScattering vertex:vert

		#include "UnityPBSLighting.cginc"
		struct SurfaceOutputScatter
		{
			fixed3 Albedo;      // base (diffuse or specular) color
			fixed3 Normal;      // tangent space normal, if written
			half3 Emission;
			half Metallic;      // 0=non-metal, 1=metal
			half Smoothness;    // 0=rough, 1=smooth
			half Occlusion;     // occlusion (default 1)
			fixed Alpha;        // alpha for transparencies
			float3 Origin;
		};
		bool rayIntersect
		(
			float3 O,
			float3 D,
			float3 C,
			float R,
			out float AO,
			out float BO
		)
		{
			float3 L = C - O;
			float DT = dot(L, D);
			float R2 = R * R;
			float CT2 = dot(L, L) - DT * DT;
			if (CT2 > R2)
				return false;
			float AT = sqrt(R2 - CT2);
			float BT = AT;
			AO = DT - AT;
			BO = DT + BT;
			return true;
		}
		bool lightSampling
		(
			float3 P,
			float3 S,
			out float opticalDepthCA
		)
		{
			float n;
			float C;
			rayIntersect(P, S, _PlanetCenter, _AtmosphereRadius, n, C);
			float time = 0;
			float ds = distance(P, P + S * C) / (float)(_LightSamples);
			for (int i = 0; i < _LightSamples; i++) {
				float3 Q = P + S * (time + lightSampleSize * 0.5);
				float height = distance(_PLanetCenter, Q) - _PlanetRadius;
				if (height < 0)
					return false;
				opticalDepthCA += exp(-height / _RayScaleHeight) * ds;
				time += ds;
			}
			return true;
		}

		inline fixed4 LightingStandardScattering(SurfaceOutputScatter s, fixed3 viewDir, UnityGI gi)
		{
			float3 L = gi.light.dir;
			float3 V = viewDir;
			float3 N = s.Normal;
			float3 O = s.Origin;

			float3 S = L; // sunlight direction
			float3 D = -V; // view ray direction

			float tA;
			float tB;
			if (!rayIntersect(O, D, _PlanetCenter, _AtmosphereRadius, tA, tB))
				return fixed4(0, 0, 0, 0);
			float pA, pB;
			if (rayIntersect(O, D, _PlanetCenter, _PlanetRadius, pA, pB))
				tB = pA;

			float3 totalViewSamples = 0;
			float opticalDepthPA = 0;
			float time = tA;
			float ds = (tB - tA) / (float)(_ViewSamples);
			for (int i = 0; i < _ViewSamples; i++) {
				float3 P = O + D * (time + ds * 0.5);
				float height = distance(C, P) - _PlanetRadius;
				float opticalDepthSegment = exp(-height / _ScaleHeight) * ds;

				opticalDepthPA += opticalDepthSegment;
				totalViewSamples += viewSampling(P, ds);
				time += ds;
			}
			float3 I = _SunIntensity * _ScatteringCoefficient * phase * totalViewSamples;

		}
		void LightingStandardScattering_GI(SurfaceOutputScatter s, UnityGIInput data, inout UnityGI gi) {
			SurfaceOutputStandard s2;
			s2.Albedo = s.Albedo;
			s2.Normal = s.Normal;
			s2.Emission = s.Emission;
			s2.Metallic = s.Metallic;
			s2.Smoothness = s.Smoothness;
			s2.Occlusion = s.Occlusion;
			s2.Alpha = s.Alpha;
			LightingStandard_GI(s2, data, gi);
		}
		
		struct Input
		{
			float3 worldPos;
			float3 center;
		};
		float4 _Color;
		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.center = mul(unity_ObjectToWorld, half4(0, 0, 0, 1));
		}
		void surf(Input IN, inout SurfaceOutputScatter o) {
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Origin = IN.worldPos;
		}

        ENDCG
    }
    FallBack "Diffuse"
}
