// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/SpaceClouds"
{
	Properties
	{
		_Center("Center", Vector) = (0,0,0)
		_Radius("Radius", Float) = 1
		_Density("Density", Range(0,1)) = 0.1
		_Scale("Scale", Float) = 1
		_Sector("Galaxy Sector", Int) = 0
		_Seed("Seed", Float) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100
		BlendOp Add
		Blend One One
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

			//#define STEPS 30
			//#define STEP_SIZE 0.167
			//#define INTENSITY 30
			#define STEPS 15
			#define STEP_SIZE 0.5
			#define INTENSITY 150

			// BEGIN NOISE FUNCTION FROM GITHUB
			float3 mod289(float3 x)
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}

			float4 mod289(float4 x) {
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}

			float4 permute(float4 x)
			{
				return mod289((x * 34.0 + 1.0) * x);
			}

			float4 taylorInvSqrt(float4 r)
			{
				return 1.79284291400159 - 0.85373472095314 * r;
			}

			float snoise(float3 v)
			{
				const float2 C = float2(1.0 / 6.0, 1.0 / 3.0);

				// First corner
				float3 i = floor(v + dot(v, C.yyy));
				float3 x0 = v - i + dot(i, C.xxx);

				// Other corners
				float3 g = step(x0.yzx, x0.xyz);
				float3 l = 1.0 - g;
				float3 i1 = min(g.xyz, l.zxy);
				float3 i2 = max(g.xyz, l.zxy);

				// x1 = x0 - i1  + 1.0 * C.xxx;
				// x2 = x0 - i2  + 2.0 * C.xxx;
				// x3 = x0 - 1.0 + 3.0 * C.xxx;
				float3 x1 = x0 - i1 + C.xxx;
				float3 x2 = x0 - i2 + C.yyy;
				float3 x3 = x0 - 0.5;

				// Permutations
				i = mod289(i); // Avoid truncation effects in permutation
				float4 p =
				  permute(permute(permute(i.z + float4(0.0, i1.z, i2.z, 1.0))
										+ i.y + float4(0.0, i1.y, i2.y, 1.0))
										+ i.x + float4(0.0, i1.x, i2.x, 1.0));

				// Gradients: 7x7 points over a square, mapped onto an octahedron.
				// The ring size 17*17 = 289 is close to a multiple of 49 (49*6 = 294)
				float4 j = p - 49.0 * floor(p * (1.0 / 49.0));  // mod(p,7*7)

				float4 x_ = floor(j * (1.0 / 7.0));
				float4 y_ = floor(j - 7.0 * x_);  // mod(j,N)

				float4 x = x_ * (2.0 / 7.0) + 0.5 / 7.0 - 1.0;
				float4 y = y_ * (2.0 / 7.0) + 0.5 / 7.0 - 1.0;

				float4 h = 1.0 - abs(x) - abs(y);

				float4 b0 = float4(x.xy, y.xy);
				float4 b1 = float4(x.zw, y.zw);

				//vec4 s0 = vec4(lessThan(b0, 0.0)) * 2.0 - 1.0;
				//vec4 s1 = vec4(lessThan(b1, 0.0)) * 2.0 - 1.0;
				float4 s0 = floor(b0) * 2.0 + 1.0;
				float4 s1 = floor(b1) * 2.0 + 1.0;
				float4 sh = -step(h, float4(0.0,0.0,0.0,0.0));

				float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
				float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;

				float3 g0 = float3(a0.xy, h.x);
				float3 g1 = float3(a0.zw, h.y);
				float3 g2 = float3(a1.xy, h.z);
				float3 g3 = float3(a1.zw, h.w);

				// Normalise gradients
				float4 norm = taylorInvSqrt(float4(dot(g0, g0), dot(g1, g1), dot(g2, g2), dot(g3, g3)));
				g0 *= norm.x;
				g1 *= norm.y;
				g2 *= norm.z;
				g3 *= norm.w;

				// Mix final noise value
				float4 m = max(0.6 - float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0);
				m = m * m;
				m = m * m;

				float4 px = float4(dot(x0, g0), dot(x1, g1), dot(x2, g2), dot(x3, g3));
				return 42.0 * dot(m, px);
			}

			// END NOISE FUNCTION FROM GITHUB

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float3 wPos : TEXCOORD1; // World position
            };

            float4 _MainTex_ST;
			float3 _Center;
			float _Radius;
			float _Density;
			float _Scale;
			int _Sector;
			float _Seed;

			// Sectors: 0=Epsilon, 1=Delta, 2=Gamma, 3=Beta, 4=Alpha

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float tsnoise(float3 p, float seedOffset) {
				return snoise(((p - _Center) + seedOffset) * _Scale);
			}

			float fsnoise(float3 p, float seedOffset) {
				float value = 0;
				float3 seed3 = float3(seedOffset, seedOffset, seedOffset);
				float3 t26148 = float3(26148, 26148, 26148);
				float3 t15680 = float3(15680, 15680, 15680);
				float3 t95710 = float3(95710, 95710, 95710);
				float3 t67592 = float3(67592, 67592, 67592);
				value += snoise(((p - _Center)) * _Scale);
				value += snoise(((p - _Center) + seed3 + t26148) * _Scale * 2) * 0.5;
				value += snoise(((p - _Center) + seed3 + t15680) * _Scale * 4) * 0.25;
				value += snoise(((p - _Center) + seed3 + t95710) * _Scale * 8) * 0.125;
				value += snoise(((p - _Center) + seed3 + t67592) * _Scale * 16) * 0.0625;
				return value;
			}

			float lerp1(float a, float b, float t) {
				return a * (1 - t) + b * t;
			}

			float3 lerp3(float3 a, float3 b, float t) {
				float3 temp;
				temp.x = lerp1(a.x, b.x, t);
				temp.y = lerp1(a.y, b.y, t);
				temp.z = lerp1(a.z, b.z, t);
				return temp;
			}

			float3 bilerp3(float3 a, float3 b, float3 c, float3 d, float x, float y) {
				float3 top = lerp3(a, b, x);
				float3 bot = lerp3(c, d, x);
				return lerp3(top, bot, y);
			}

			float3 bbilerp3(float3 a, float3 b, float3 c, float3 d, float x, float y) {
				float tx;
				float ty;
				if (x < 0.25)
				{
					tx = 0;
				}
				else if (x > 0.75)
				{
					tx = 1;
				}
				else {
					tx = (x - 0.25) * 2;
				}
				if (y < 0.25)
				{
					ty = 0;
				}
				else if (y > 0.75)
				{
					ty = 1;
				}
				else {
					ty = (y - 0.25) * 2;
				}
				return bilerp3(a, b, c, d, tx, ty);
			}

			fixed3 procColor(float3 p) {
				fixed3 col;
				float lerpVal = 0;
				float secLerp = snoise(((p - _Center)* 0.025 + 89754 + _Seed) * _Scale);
				float secLerp2 = snoise(((p - _Center)* 0.025 + 50721 + _Seed) * _Scale);
				float secLerpN1 = snoise(((p - _Center)* 0.025 + 89754 + _Seed) * _Scale);
				float secLerpN2 = snoise(((p - _Center)* 0.025 + 50721 + _Seed) * _Scale);
				float secLerpN3 = snoise(((p - _Center)* 0.025 + 19112 + _Seed) * _Scale);
				//if (_Sector != 4) {
					lerpVal = snoise(((p - _Center)* 0.25 + 12345 + _Seed) * _Scale);

				/*}
				else {
					lerpVal = -1.25 * fsnoise(p, _Seed);
				}
				float transformedLerpVal = -1;
				col1 = bbilerp3(float3(0, 0.30, 0.5), float3(0.75, 0, 0), float3(0, 0.75, 0), float3(0.75, 0.1, 0), secLerp, secLerp2);
				col2 = bbilerp3(float3(0, 0, 0.75), float3(0, 0, 0.5), float3(0.5, 0.25, 0), float3(0.75, 0.75, 0), secLerp, secLerp2);
				*/
				float3 col1, col2;
				if (secLerpN1 < 0.5) {
					if (secLerpN2 < 0.5) {
						if (secLerpN3 < 0.5) {
							col1 = float3(0xFF, 0x00, 0x8C) / 255;
							col2 = float3(0x00, 0x26, 0xFF) / 255;
						}
						else {
							col1 = float3(0xFF, 0x00, 0x43) / 255;
							col2 = float3(0x5D, 0x00, 0xFF) / 255;
						}
					}
					else {
						if (secLerpN3 < 0.5) {
							col1 = float3(0x00, 0x94, 0xFF) / 255;
							col2 = float3(0x00, 0xFF, 0x21) / 255;
						}
						else {
							col1 = float3(0x00, 0x94, 0xFF) / 255;
							col2 = float3(0x7C, 0x51, 0x00) / 255;
						}
					}
				}
				else {
					if (secLerpN2 < 0.5) {
						if (secLerpN3 < 0.5) {
							col1 = float3(0x00, 0x00, 0x00) / 255;
							col2 = float3(0x4E, 0x00, 0x72) / 255;
						}
						else {
							col1 = float3(0x77, 0x00, 0xFF) / 255;
							col2 = float3(0xFF, 0x00, 0xDC) / 255;
						}
					}
					else {
						if (secLerpN3 < 0.5) {
							col1 = float3(0xFF, 0x00, 0x00) / 255;
							col2 = float3(0xFF, 0xFF, 0x00) / 255;
						}
						else {
							col1 = float3(0x00, 0x00, 0x00) / 255;
							col2 = float3(0x00, 0x26, 0xFF) / 255;
						}
					}
				}
				/*if (_Sector == 0) {
					col1 = float3(0, 0.30, 0.5);
					col2 = float3(0, 0, 0.75);
				}
				else if (_Sector == 1) {
					col1 = float3(0.75, 0.1, 0);
					col2 = float3(0.75, 0.75, 0);
				}
				else if (_Sector == 2) {
					col1 = float3(0, 0.75, 0);
					col2 = float3(0.5, 0.25, 0);
				}
				else if (_Sector == 3) {
					col1 = float3(0.75, 0, 0);
					col2 = float3(0, 0, 0.5);
				}
				else {
					//col1 = float3(0.37, 0, 0.75);
					//col2 = float3(0.5, 0, 0.5);
					if (lerpVal < 0.125) {
						transformedLerpVal = lerpVal * 8;
						col1 = float3(1, 1, 0);
						col2 = float3(1, 0, 0);
					}
					else if (lerpVal < 0.5) {
						transformedLerpVal = (lerpVal - 0.125) * 8;
						col1 = float3(0, 1, 0);
						col2 = float3(1, 1, 0);
					}
					else if (lerpVal < 0.875) {
						transformedLerpVal = (lerpVal - 0.5) * 8;
						col1 = float3(0, 1, 1);
						col2 = float3(0, 1, 0);
					}
					else {
						transformedLerpVal = (lerpVal - 0.875) * 8;
						col1 = float3(0, 0.5, 1);
						col2 = float3(0, 1, 1);
					}
				}*/
				//if (_Sector != 4) {
					col = lerp3(col1, col2, lerpVal);
				/*}
				else {
					col = lerp3(col1, col2, 1 - transformedLerpVal);
				}*/
				//col.r = (tsnoise(p,12345) + 1) / 2;
				//col.g = (tsnoise(p,54321) + 1) / 2;
				//col.b = (tsnoise(p,0) + 1) / 2;
				return col;
			}

			float sphereDistance(float3 p)
			{
				//return distance(p, _Center) - _Radius;
				return tsnoise(p,0) - _Radius;
			}

			float density(float3 p) {
				float emptinessModifier = ((fsnoise(p, 54321)) + 3) / 4;
				//float emptinessModifier = 1;
				if (emptinessModifier > -0.3)
					return -1 * _Density * fsnoise(p, _Seed) * emptinessModifier;
				else
					return 0;
			}

			fixed4 raymarch(float3 position, float3 direction)
			{
				fixed4 col;
				//col.rgb = procColor(position);
				col.rgb = fixed3(0, 0, 0);
				col.a = 0;
				for (int i = 0; i < STEPS; i++)
				{
					float dist = sphereDistance(position);
					if (density(position) > 0) {
						col.a += density(position);
						col.rgb += density(position) * procColor(position) * INTENSITY;
					}
					position += direction * STEP_SIZE;
				}

				return col;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float3 worldPosition = i.wPos;
				float3 viewDirection = normalize(i.wPos - _WorldSpaceCameraPos);
				return raymarch(worldPosition, viewDirection);
            }
            ENDCG
        }
    }
}
