using System;
using System.IO;

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
			filename = $"{filename}.hlsl";

			var source = File.ReadAllText(Path.Combine("Assets", filename));
			var vsCode = ShaderBytecode.Compile(source, "VSMain", "vs_5_0", ShaderFlags.Debug, sourceFileName: filename);
			var psCode = ShaderBytecode.Compile(source, "PSMain", "ps_5_0", ShaderFlags.Debug, sourceFileName: filename);

			VertexShader = new VertexShader(dx, vsCode);
			PixelShader = new PixelShader(dx, psCode);
		}

		public void Dispose()
		{
			VertexShader.Dispose();
			PixelShader.Dispose();
		}
	}
}
