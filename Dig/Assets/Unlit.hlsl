struct VSInput
{
	float3 Position : Position;
	float4 Color : Color;
};

struct VSOutput
{
	float4 Position : SV_POSITION;
	float4 Color : Color;
};

cbuffer PerObject
{
	float4x4 Projection;
};

VSOutput VSMain(VSInput input)
{
	VSOutput output;
	output.Position = mul(float4(input.Position, 1), Projection);
	output.Color = input.Color;
	return output;
}

float4 PSMain(VSOutput input) : SV_TARGET
{
	return input.Color;
}
