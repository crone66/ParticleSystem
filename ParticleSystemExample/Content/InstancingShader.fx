float4x4 ViewProjection;
texture Texture;

sampler TextureSampler = sampler_state
{
    texture = <Texture>;
    mipfilter = LINEAR;
    minfilter = LINEAR;
    magfilter = LINEAR;
};

struct InstancingVSinput
{
    float4 Position : SV_Position0;
    float2 TexCoord : TEXCOORD0;
};

struct InstancingVSoutput
{
    float4 Position : SV_Position0;
    float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR1;
};

InstancingVSoutput InstancingVS(InstancingVSinput input, float alphaValue : BLENDWEIGHT1, float4x4 instancingWorld : BINORMAL1)
{
    InstancingVSoutput output;

	output.Color = float4(1,1,1, alphaValue);

	float4 pos = mul(instancingWorld, input.Position);
    pos = mul(pos, ViewProjection);

    output.Position = pos;
	output.TexCoord = input.TexCoord;

    return output;
}

float4 InstancingPS(InstancingVSoutput input) : COLOR0
{
    return tex2D(TextureSampler, input.TexCoord) * input.Color;
}

technique Instancing
{
    pass Pass0
    {
		#if SM4

		PixelShader = compile ps_4_0_level_9_1 InstancingPS();
		VertexShader = compile vs_4_0_level_9_1 InstancingVS();
		#elif SM3

		PixelShader = compile ps_3_0 InstancingPS();
		VertexShader = compile vs_3_0 InstancingVS();

		#else

		PixelShader = compile ps_2_0 InstancingPS();
		VertexShader = compile vs_2_0 InstancingVS();

		#endif
    }
}