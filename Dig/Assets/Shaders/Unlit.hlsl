#include "Common.hlsl"

struct VSOutput
{
	float4 Position : SV_POSITION;
	float4 Color : Color;
};

VSOutput VSMain(VSInput input)
{
	VSOutput output;
	output.Position = mul(float4(input.Position, 1), mul(Model, ViewProjection));
	output.Color = input.Color;
	return output;
}

float4 PSMain(VSOutput input) : SV_TARGET
{
	return input.Color;
}
