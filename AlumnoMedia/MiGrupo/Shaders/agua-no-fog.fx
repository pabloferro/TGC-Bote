/*
* Shader utilizado en ejemplo "EjemploShaderTgcMesh"
* Tiene varias techniques para hacer distintos tipos de efectos simples
*/

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

float freq1, freq2, freq3, freq4;
float amp1, amp2, amp3, amp4;
float phase1, phase2, phase3, phase4;
float2 dir1, dir2, dir3, dir4;

float4 eyePosition; //Posicion de la camara
float timer;
float reflection;

//Difuse
float3 vSunlightDirection =  float3(0,-1,0);
float3 vDiffuseColor = float3(1,1,0.70);
float DiffuseIntensity = 0.85;
//Specular
float Shininess = 500;
float4 SpecularColor = float4(1, 1, 1, 1);    
float SpecularIntensity = 0.85;

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

//Textura utilizada para EnvironmentMap
texture texCubeMap;
samplerCUBE cubeMap = sampler_state
{
	Texture = (texCubeMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

/*************************************************** Olas *********************************************************/
struct Wave {
  float freq;  // 2*PI / wavelength
  float amp;   // amplitude
  float phase; // speed * 2*PI / wavelength
  float2 dir;
};


#define NWAVES 2
Wave wave[NWAVES] = {
	{ 0, 0, 0, float2(1, 1) },
	{ 0, 0, 0, float2(1, 1) }
};

//#define NWAVES 3
//Wave wave[NWAVES] = {
//	{ 1.0, 0.25, 0.5, float2(0.7, 0.7) },
//	{ 2.0, 0.5, 1.3, float2(0, 1) },
//	{ 3.0, 0.15, 0.7, float2(0, 1) }
//};

float evaluateWave(Wave w, float2 pos, float t)
{
  return w.amp * sin( dot(w.dir, pos)*w.freq + t*w.phase);
}

// derivative of wave function
float evaluateWaveDeriv(Wave w, float2 pos, float t)
{
  return w.freq*w.amp * cos( dot(w.dir, pos)*w.freq + t*w.phase);
}

void loadWaves()
{
	wave[0].freq = freq1;
	wave[0].amp = amp1;
	wave[0].dir = dir1;
	wave[0].phase = phase1;
	wave[1].freq = freq2;
	wave[1].amp = amp2;
	wave[1].dir = dir2;
	wave[1].phase = phase2;
}

/*********************************************** Vertex Shaders ***************************************************/

struct VS_OUTPUT
{
	float4 Position : POSITION;
	float3 WorldPosition : TEXCOORD2;
	float4 Color : COLOR0;
	float3 Norm : TEXCOORD1;
	float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT VS_main (
	float4 Position : POSITION,
	float3 WorldPosition : TEXCOORD2,
	float3 Normal : NORMAL,
	float4 Color : COLOR
)
{
	VS_OUTPUT Out = (VS_OUTPUT)0;
	loadWaves();

	float ddx = 0.0, ddy = 0.0;
	for(int i=0; i<NWAVES; i++) {
		Position.y += evaluateWave(wave[i], Position.xz, timer);
		float deriv = evaluateWaveDeriv(wave[i], Position.xz, timer);
    	ddx += deriv * wave[i].dir.x;
    	ddy += deriv * wave[i].dir.y;		
    }
	Out.Position = mul(Position, matWorldViewProj);		
	Out.WorldPosition = mul(Position, matWorld).xyz;
	
	// compute tangent basis
    float3 B = float3(1, ddx, 0);
    float3 T = float3(0, ddy, 1);
    float3 N = float3(-ddx, 1, -ddy); //Vector normal
	
	Normal = float3(-ddx, -ddy, 1);

	//Iluminación
		//float3 vSpecularColor = float3(1,0,0);
		float3 vectorL = -vSunlightDirection; // L vector 
		//float3 R = normalize(reflect(-vectorL, Normal)); 
		//float3 R = 2.0 * dot(vectorL, Normal) * Normal - vectorL;
		//float3 vViewDir = eyePosition.xyz - Out.Position;
		
		//float fSpecularPower=0.75;
		//float fSpecularReflectance = saturate(dot(R, vViewDir)); 
		//float3 vSpecular = 0.5 * vSpecularColor * pow(fSpecularReflectance, fSpecularPower); 
		float3 vDiffuse = vDiffuseColor * max(0.0, dot(Normal, vectorL));    
		 
		// Lighting contribution is the sum of the diffuse and specular terms 
		float3 vSunlight = DiffuseIntensity*vDiffuse;  		
	//Iluminación
	
	float4 finalColor;
	finalColor.rgb = vSunlight;
	finalColor.a=1.0f;	
	
	Out.Norm = Normal;
	
	Out.Color = 0.5f * Color +  0.5f * finalColor;
	return Out;
}

/*********************************************** Pixel Shaders ***************************************************/

float4 PS_main(VS_OUTPUT input) : COLOR
{
    float3 normal = normalize(input.Norm);
    float3 r = normalize(2 * dot(vSunlightDirection, normal) * normal - vSunlightDirection);
	float3 ViewVector = eyePosition.xyz;// - input.Position;
    float3 v = normalize(mul(normalize(ViewVector), matWorld));

    float dotProduct = dot(r, v);
    float4 specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, Shininess), 0) * length(input.Color);
	
	//Obtener texel de CubeMap
	float3 Nn = normalize(input.Norm);
	float3 Vn = normalize(eyePosition.xyz - input.WorldPosition);
	float3 R1 = reflect(Vn,Nn);
    float4 reflectionColor = float4(texCUBE(cubeMap, R1).rgb, 1) * reflection;
	
    return saturate(input.Color + reflectionColor + specular * 0.5f); // + reflectionColor
}

/*********************************************** Techniques ***************************************************/

technique Oceano {
	pass p0 {
		VertexShader = compile vs_2_0 VS_main();
		PixelShader = compile ps_2_0 PS_main();
	}
}
