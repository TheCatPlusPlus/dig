using System.Runtime.InteropServices;

using Dig.Renderer.Shaders;

using SharpDX;

namespace Dig.Renderer.Models
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vertex
	{
		[Semantic("POSITION")]
		public Vector3 Position;
		[Semantic("TEXCOORD")]
		public Vector2 UV;

		public Vertex(float x, float y, float z, Vector2 uv = default)
			: this()
		{
			Position = new Vector3(x, y, z);
			UV = uv;
		}
	}
}
