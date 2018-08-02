using System.Runtime.InteropServices;

using SharpDX;

namespace Dig.Renderer.Models
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vertex
	{
		public Vector3 Position;
		public Color4 Color;

		public Vertex(float x, float y, float z, Color4? color = default)
			: this()
		{
			Position = new Vector3(x, y, z);
			Color = color ?? Color4.Black;
		}
	}
}
