using System.Collections.Generic;

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

		public uint AddVertex(float x, float y, float z)
		{
			return AddVertex(new Vertex(x, y, z));
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

		public static Mesh Cube()
		{
			var cube = new MeshBuilder();

			cube.AddVertex(-1.0f, -1.0f, -1.0f);
			cube.AddVertex(-1.0f, +1.0f, -1.0f);
			cube.AddVertex(+1.0f, +1.0f, -1.0f);
			cube.AddVertex(+1.0f, -1.0f, -1.0f);
			cube.AddVertex(-1.0f, -1.0f, +1.0f);
			cube.AddVertex(-1.0f, +1.0f, +1.0f);
			cube.AddVertex(+1.0f, +1.0f, +1.0f);
			cube.AddVertex(+1.0f, -1.0f, +1.0f);

			// front face
			cube.AddTriangle(0, 1, 2);
			cube.AddTriangle(0, 2, 3);

			// back face
			cube.AddTriangle(4, 6, 5);
			cube.AddTriangle(4, 7, 6);

			// left face
			cube.AddTriangle(4, 5, 1);
			cube.AddTriangle(4, 1, 0);

			// right face
			cube.AddTriangle(3, 2, 6);
			cube.AddTriangle(3, 6, 7);

			// top face
			cube.AddTriangle(1, 5, 6);
			cube.AddTriangle(1, 6, 2);

			// bottom face
			cube.AddTriangle(4, 0, 3);
			cube.AddTriangle(4, 3, 7);

			return cube.Finish();
		}
	}
}
