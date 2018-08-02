#include "Common.hlsl"

// used only to get an input signature
float4 VSMain(VSInput input) : SV_POSITION
{
	return float4(input.Position, 1);
}
