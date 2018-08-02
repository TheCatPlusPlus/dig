using SharpDX;

namespace Dig.Entities
{
	// TODO temporary
	public struct Transform
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public Vector3 Scale;

		public Matrix Matrix => Matrix.Transformation(Vector3.Zero, Quaternion.Identity, Scale, Vector3.Zero, Rotation, Position);

		public Transform(Vector3 position = default, Quaternion rotation = default, Vector3? scale = default)
			: this()
		{
			Position = position;
			Rotation = rotation;
			Scale = scale ?? Vector3.One;
		}
	}
}
