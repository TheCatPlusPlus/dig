using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

using D3D11VertexShader = SharpDX.Direct3D11.VertexShader;

namespace Dig.Renderer
{
	public static class VertexBufferLayout
	{
		private static readonly Dictionary<Type, Format> KnownFormats = new Dictionary<Type, Format>
		{
			{ typeof(float), Format.R32_Float },
			{ typeof(Vector2), Format.R32G32_Float },
			{ typeof(Vector3), Format.R32G32B32_Float },
			{ typeof(Color3), Format.R32G32B32_Float },
			{ typeof(Vector4), Format.R32G32B32A32_Float },
			{ typeof(Color4), Format.R32G32B32A32_Float },
			{ typeof(RectangleF), Format.R32G32B32A32_Float },
			{ typeof(Color), Format.R8G8B8A8_UNorm },
			{ typeof(ColorBGRA), Format.B8G8R8A8_UNorm },
			{ typeof(Half4), Format.R16G16B16A16_Float },
			{ typeof(Half2), Format.R16G16_Float },
			{ typeof(Half), Format.R16_Float },
			{ typeof(Int4), Format.R32G32B32A32_SInt },
			{ typeof(Int3), Format.R32G32B32_SInt },
			{ typeof(int), Format.R32_SInt },
			{ typeof(uint), Format.R32_UInt },
			{ typeof(Bool4), Format.R32G32B32A32_SInt },
			{ typeof(RawBool), Format.R32_SInt }
		};

		public static InputLayout Create<T>(DXContext ctx, VertexShader shader)
			where T : struct, IVertex
		{
			var type = typeof(T);
//
//			Debug.Assert(
//				type.GetCustomAttribute<StructLayoutAttribute>() != null,
//				"type.GetCustomAttribute<StructLayoutAttribute>() != null");
//			Debug.Assert(
//				type.GetCustomAttribute<StructLayoutAttribute>().Value == LayoutKind.Sequential,
//				"type.GetCustomAttribute<StructLayoutAttribute>().Value == LayoutKind.Sequential");

			var query = from field in type.GetFields()
				let offset = Marshal.OffsetOf<T>(field.Name).ToInt32()
				orderby offset
				let format = GetFormat(field.FieldType)
				select new InputElement(field.Name, 0, format, offset, 0);

			return new InputLayout(ctx.Device, shader.Bytecode, query.ToArray());
		}

		private static Format GetFormat(Type type)
		{
			Debug.Assert(KnownFormats.ContainsKey(type), $"Unsupported type in vertex struct: {type.Name}");
			return KnownFormats[type];
		}
	}
}
