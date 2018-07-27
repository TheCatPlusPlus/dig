using System.Globalization;

using Dig.Utils;

using JetBrains.Annotations;

namespace Dig.Input
{
	public struct InputAxis
	{
		public bool IsHeld => !MathExt.ApproxEquals(Value, 0);
		public bool WasHeld => !MathExt.ApproxEquals(PreviousValue, 0);
		public bool HasChanged => !Equals(PreviousValue, Value);
		public bool IsUp => WasHeld && !IsHeld;
		public bool IsDown => !WasHeld && IsHeld;

		public float Value { get; private set; }
		public float PreviousValue { get; private set; }
		public float NextValue { get; private set; }

		public void Set(float value)
		{
			NextValue = value;
		}

		public void Set(bool value)
		{
			Set(value ? 1 : 0);
		}

		public void Add(float value)
		{
			NextValue += value;
		}

		public void Commit()
		{
			PreviousValue = Value;
			Value = NextValue;
			NextValue = 0;
		}

		[NotNull]
		public override string ToString()
		{
			return Value.ToString(CultureInfo.InvariantCulture);
		}
	}
}
