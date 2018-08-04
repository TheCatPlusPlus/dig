using System;

using SharpDX;

namespace Dig.Entities
{
	// TODO temporary
	public class Transform
	{
		private Vector3 _position;

		public Vector3 Position
		{
			get => _position;
			set
			{
				_position = value;
				OnChanged();
			}
		}

		private Quaternion _rotation;

		public Quaternion Rotation
		{
			get => _rotation;
			set
			{
				_rotation = value;
				OnChanged();
			}
		}

		private Vector3 _scale;

		public Vector3 Scale
		{
			get => _scale;
			set
			{
				_scale = value;
				OnChanged();
			}
		}

		public Matrix Matrix => Matrix.Transformation(Vector3.Zero, Quaternion.Identity, Scale, Vector3.Zero, Rotation, Position);
		public event Action Changed;

		public Transform(Vector3 position = default, Quaternion rotation = default, Vector3? scale = default)
		{
			Position = position;
			Rotation = rotation;
			Scale = scale ?? Vector3.One;
		}

		protected virtual void OnChanged()
		{
			Changed?.Invoke();
		}
	}
}
