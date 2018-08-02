using System;
using System.Collections.Generic;
using System.Linq;

namespace Dig.Renderer.Models
{
	public sealed class Mesh
	{
		private readonly Vertex[] _vertices;
		private readonly Triangle[] _triangles;

		public Span<Vertex> Vertices => _vertices;
		public Span<Triangle> Triangles => _triangles;

		public Mesh(Mesh other)
		{
			_vertices = other._vertices.ToArray();
			_triangles = other._triangles.ToArray();
		}

		public Mesh(IEnumerable<Vertex> vertices, IEnumerable<Triangle> triangles)
		{
			_vertices = vertices.ToArray();
			_triangles = triangles.ToArray();
		}

		public ref Vertex GetVertex(uint idx)
		{
			return ref _vertices[idx];
		}

		public ref Triangle GetTriangle(uint idx)
		{
			return ref _triangles[idx];
		}
	}
}
