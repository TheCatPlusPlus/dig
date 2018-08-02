using System;
using System.IO;

using Dig.Renderer.Shaders;

using SharpDX.D3DCompiler;

namespace Dig.Renderer
{
	public sealed class Material : IDisposable
	{
		private string _debugName;

		public readonly VertexShader VertexShader;
		public readonly PixelShader PixelShader;

		public string DebugName
		{
			get => _debugName;
			set
			{
				_debugName = value;
				VertexShader.DebugName = $"{value}.{nameof(VertexShader)}";
				PixelShader.DebugName = $"{value}.{nameof(PixelShader)}";
			}
		}

		public Material(DXContext dx, string filename)
		{
			VertexShader = new VertexShader(dx, filename);
			PixelShader = new PixelShader(dx, filename);
		}

		public void Dispose()
		{
			VertexShader.Dispose();
			PixelShader.Dispose();
		}
	}
}
