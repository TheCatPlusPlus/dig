#include "Common.hlsl"

struct VSOutput
{
	float4 Position : SV_POSITION;
	float2 UV : TEXCOORD0;
};

Texture2D tAtlas : register(t0);
SamplerState sAtlas : register(s0);

VSOutput VSMain(VSInput input)
{
	VSOutput output;
	output.Position = mul(float4(input.Position, 1), mul(Model, ViewProjection));
	output.UV = input.UV;
	return output;
}

float4 PSMain(VSOutput input) : SV_TARGET
{
	return tAtlas.Sample(sAtlas, input.UV);
}
