using System.Collections.Generic;

using Dig.Renderer.Texturing;

using SharpDX;

namespace Dig.Renderer.Models
{
	public sealed class MeshBuilder
	{
		private readonly List<Vertex> _vertices;
		private readonly List<Triangle> _triangles;

		public MeshBuilder()
		{
			_vertices = new List<Vertex>();
			_triangles = new List<Triangle>();
		}

		public Mesh Finish()
		{
			return new Mesh(_vertices.ToArray(), _triangles.ToArray());
		}

		public uint AddVertex(in Vertex vertex)
		{
			return Add(_vertices, vertex);
		}

		public uint AddVertex(float x, float y, float z, Vector2 uv)
		{
			return AddVertex(new Vertex(x, y, z, uv));
		}

		public uint AddTriangle(in Triangle triangle)
		{
			return Add(_triangles, triangle);
		}

		public uint AddTriangle(uint v0, uint v1, uint v2)
		{
			return AddTriangle(new Triangle(v0, v1, v2));
		}

		public void SetVertex(uint idx, in Vertex vertex)
		{
			_vertices[(int)idx] = vertex;
		}

		public void SetTriangle(uint idx, in Triangle triangle)
		{
			_triangles[(int)idx] = triangle;
		}

		private static uint Add<T>(ICollection<T> list, in T item)
			where T : struct
		{
			var idx = (uint)list.Count;
			list.Add(item);
			return idx;
		}

		public static Mesh Cube(UVRect topUV, UVRect bottomUV, UVRect sidesUV)
		{
			var cube = new MeshBuilder();

			// top face
			cube.AddVertex(-1.0f, 1.0f, -1.0f, topUV.BottomLeft);
			cube.AddVertex(-1.0f, 1.0f, 1.0f, topUV.TopLeft);
			cube.AddVertex(1.0f, 1.0f, 1.0f, topUV.TopRight);
			cube.AddVertex(1.0f, 1.0f, -1.0f, topUV.BottomRight);
			cube.AddTriangle(8, 9, 10);
			cube.AddTriangle(8, 10, 11);

			// bottom face
			cube.AddVertex(-1.0f, -1.0f, -1.0f, bottomUV.BottomRight);
			cube.AddVertex(1.0f, -1.0f, -1.0f, bottomUV.BottomLeft);
			cube.AddVertex(1.0f, -1.0f, 1.0f, bottomUV.TopLeft);
			cube.AddVertex(-1.0f, -1.0f, 1.0f, bottomUV.TopRight);
			cube.AddTriangle(12, 13, 14);
			cube.AddTriangle(12, 14, 15);

			// front face
			cube.AddVertex(-1.0f, -1.0f, -1.0f, sidesUV.BottomLeft);
			cube.AddVertex(-1.0f, 1.0f, -1.0f, sidesUV.TopLeft);
			cube.AddVertex(1.0f, 1.0f, -1.0f, sidesUV.TopRight);
			cube.AddVertex(1.0f, -1.0f, -1.0f, sidesUV.BottomRight);
			cube.AddTriangle(0, 1, 2);
			cube.AddTriangle(0, 2, 3);

			// back face
			cube.AddVertex(-1.0f, -1.0f, 1.0f, sidesUV.BottomRight);
			cube.AddVertex(1.0f, -1.0f, 1.0f, sidesUV.BottomLeft);
			cube.AddVertex(1.0f, 1.0f, 1.0f, sidesUV.TopLeft);
			cube.AddVertex(-1.0f, 1.0f, 1.0f, sidesUV.TopRight);
			cube.AddTriangle(4, 5, 6);
			cube.AddTriangle(4, 6, 7);

			// left face
			cube.AddVertex(-1.0f, -1.0f, 1.0f, sidesUV.BottomLeft);
			cube.AddVertex(-1.0f, 1.0f, 1.0f, sidesUV.TopLeft);
			cube.AddVertex(-1.0f, 1.0f, -1.0f, sidesUV.TopRight);
			cube.AddVertex(-1.0f, -1.0f, -1.0f, sidesUV.BottomRight);
			cube.AddTriangle(16, 17, 18);
			cube.AddTriangle(16, 18, 19);

			// right face
			cube.AddVertex(1.0f, -1.0f, -1.0f, sidesUV.BottomLeft);
			cube.AddVertex(1.0f, 1.0f, -1.0f, sidesUV.TopLeft);
			cube.AddVertex(1.0f, 1.0f, 1.0f, sidesUV.TopRight);
			cube.AddVertex(1.0f, -1.0f, 1.0f, sidesUV.BottomRight);
			cube.AddTriangle(20, 21, 22);
			cube.AddTriangle(20, 22, 23);

			return cube.Finish();
		}
	}
}
