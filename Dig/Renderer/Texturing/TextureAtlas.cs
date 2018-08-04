using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Dig.Utils;

using JetBrains.Annotations;

using Newtonsoft.Json;

using SharpDX.Direct3D11;

namespace Dig.Renderer.Texturing
{
	public sealed class TextureAtlas : IEnumerable<TextureAtlas.AtlasItem>, IDisposable
	{
		[JsonObject(MemberSerialization.OptOut)]
		public sealed class AtlasItem
		{
			public int Width { get; set; }
			public int Height { get; set; }
			public string Name { get; set; }
			public int Index { get; set; }
			public UVRect UVRect { get; set; }
			public int X { get; set; }
			public int Y { get; set; }
		}

		private readonly List<AtlasItem> _items;
		private readonly Dictionary<string, AtlasItem> _byName;

		public GPUTexture2D Texture { get; }
		public ShaderResourceView1 View => Texture.View;

		public UVRect this[int idx] => _items[idx].UVRect;
		public UVRect this[string name] => _byName[name].UVRect;

		public string DebugName
		{
			get => Texture.DebugName;
			set => Texture.DebugName = value;
		}

		public TextureAtlas(GPUTexture2D texture, IEnumerable<AtlasItem> items)
		{
			Texture = texture;
			_items = items.ToList();
			_byName = _items.ToDictionary(i => i.Name, i => i);
		}

		public static TextureAtlas Load(DXContext dx, string filename)
		{
			var texture = GPUTexture2D.Load(dx, Path.ChangeExtension(filename, "dds"));
			var items = JSON.Load<List<AtlasItem>>(Path.ChangeExtension(filename, "json"));
			return new TextureAtlas(texture, items);
		}

		public void Dispose()
		{
			Texture.Dispose();
		}

		[NotNull]
		public IEnumerator<AtlasItem> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		[NotNull]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
