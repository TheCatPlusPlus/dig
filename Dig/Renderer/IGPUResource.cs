using D3D11Resource = SharpDX.Direct3D11.Resource;

namespace Dig.Renderer
{
	// ReSharper disable once UnusedTypeParameter
	public interface IGPUResource<T>
		where T : struct
	{
		DXContext Parent { get; }
		D3D11Resource Resource { get; }
		bool IsDynamic { get; }
		int Count { get; }
		int ByteSize { get; }
		int Stride { get; }
	}
}
