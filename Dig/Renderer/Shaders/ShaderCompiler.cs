using System;
using System.IO;
using System.Text;

using JetBrains.Annotations;

using NLog;

using SharpDX;
using SharpDX.D3DCompiler;

namespace Dig.Renderer.Shaders
{
	public static class ShaderCompiler
	{
		private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

		private sealed class IncludeImpl : Include
		{
			public IDisposable Shadow { get; set; }

			[NotNull]
			public Stream Open(IncludeType type, [NotNull] string fileName, [NotNull] Stream parentStream)
			{
				return new FileStream(Path.Combine(BasePath, fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
			}

			public void Close([NotNull] Stream stream)
			{
			}

			public void Dispose()
			{
				Shadow?.Dispose();
			}
		}

		public enum Profile
		{
			VertexShader5,
			PixelShader5
		}

		private static readonly IncludeImpl Callback = new IncludeImpl();
		public static string BasePath => Path.Combine("Assets", "Shaders");

		public static byte[] CompileVertexShader(string filename)
		{
			return Compile(filename, Profile.VertexShader5, "VSMain");
		}

		public static byte[] CompilePixelShader(string filename)
		{
			return Compile(filename, Profile.PixelShader5, "PSMain");
		}

		public static byte[] Compile(string filename, Profile profile, string entryPoint)
		{
			filename = $"{filename}.hlsl";

			var path = Path.Combine(BasePath, filename);
			var source = File.ReadAllText(path, Encoding.UTF8);

			var preprocessed = ShaderBytecode.Preprocess(source, include: Callback);

			try
			{
				var bytecode = ShaderBytecode.Compile(preprocessed, entryPoint, profile.GetName(), ShaderFlags.Debug, sourceFileName: filename);
				return bytecode;
			}
			catch (CompilationException e)
			{
				Log.Error($"Shader {filename} failed to compile:\n{e.Message}");
				throw;
			}
		}

		public static string GetName(this Profile profile)
		{
			switch (profile)
			{
				case Profile.VertexShader5:
					return "vs_5_0";
				case Profile.PixelShader5:
					return "ps_5_0";
				default:
					throw new ArgumentOutOfRangeException(nameof(profile), profile, null);
			}
		}
	}
}
