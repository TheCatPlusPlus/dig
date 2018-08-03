#ifndef COMMON_HLSL
#define COMMON_HLSL

struct VSInput
{
	float3 Position : POSITION;
	float2 UV : TEXCOORD0;
};

cbuffer PerFrame : register(b0)
{
	float4x4 ViewProjection;
};

cbuffer PerObject : register(b1)
{
	float4x4 Model;
};

#endif
