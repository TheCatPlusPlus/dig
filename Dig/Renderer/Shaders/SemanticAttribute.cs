using System;

namespace Dig.Renderer.Shaders
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SemanticAttribute : Attribute
	{
		public string Name { get; }

		public SemanticAttribute(string name)
		{
			Name = name;
		}
	}
}
