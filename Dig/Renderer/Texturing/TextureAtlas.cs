using SharpDX;

namespace Dig.Renderer.Texturing
{
	public sealed class TextureAtlas
	{
		public GPUTexture2D Texture { get; }
		public int ItemWidth { get; }
		public int ItemHeight { get; }
		public int Rows { get; }
		public int Columns { get; }

		public UVRect this[int idx] => GetUV(idx);

		public TextureAtlas(GPUTexture2D texture, int itemWidth, int itemHeight)
		{
			Texture = texture;
			ItemWidth = itemWidth;
			ItemHeight = itemHeight;

			Columns = Texture.Width / ItemWidth;
			Rows = Texture.Height / ItemHeight;
		}

		private UVRect GetUV(int idx)
		{
			var column = idx % Columns;
			var row = idx / Columns;

			var topLeftX = column * ItemWidth + 0.5f;
			var topLeftY = row * ItemHeight + 0.5f;

			var topRightX = topLeftX + ItemWidth - 1;
			var topRightY = topLeftY;

			var bottomLeftX = topLeftX;
			var bottomLeftY = topLeftY + ItemHeight - 1;

			var bottomRightX = topRightX;
			var bottomRightY = topRightY + ItemHeight - 1;

			var topLeftU = topLeftX / Texture.Width;
			var topLeftV = topLeftY / Texture.Height;
			var topRightU = topRightX / Texture.Width;
			var topRightV = topRightY / Texture.Height;
			var bottomLeftU = bottomLeftX / Texture.Width;
			var bottomLeftV = bottomLeftY / Texture.Height;
			var bottomRightU = bottomRightX / Texture.Width;
			var bottomRightV = bottomRightY / Texture.Height;

			var topLeftUV = new Vector2(topLeftU, topLeftV);
			var topRightUV = new Vector2(topRightU, topRightV);
			var bottomLeftUV = new Vector2(bottomLeftU, bottomLeftV);
			var bottomRightUV = new Vector2(bottomRightU, bottomRightV);

			return new UVRect
			{
				TopLeft = topLeftUV,
				TopRight = topRightUV,
				BottomLeft = bottomLeftUV,
				BottomRight = bottomRightUV
			};
		}
	}
}
