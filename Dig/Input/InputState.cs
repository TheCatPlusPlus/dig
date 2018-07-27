namespace Dig.Input
{
	public sealed class InputState
	{
		private InputAxis _dummy;

		public InputAxis MoveForward;
		public InputAxis MoveBackward;
		public InputAxis MoveLeft;
		public InputAxis MoveRight;
		public InputAxis LookX;
		public InputAxis LookY;
		public InputAxis Jump;
		public InputAxis Quit;
		public InputAxis Focus;

		public ref InputAxis this[InputCode code]
		{
			get
			{
				// ReSharper disable once SwitchStatementMissingSomeCases
				switch (code)
				{
					case InputCode.KeyW:
						return ref MoveForward;
					case InputCode.KeyS:
						return ref MoveBackward;
					case InputCode.KeyA:
						return ref MoveLeft;
					case InputCode.KeyD:
						return ref MoveRight;
					case InputCode.MouseX:
						return ref LookX;
					case InputCode.MouseY:
						return ref LookY;
					case InputCode.KeyEscape:
						return ref Quit;
					case InputCode.KeySpace:
						return ref Jump;
					default:
						return ref _dummy;
				}
			}
		}

		public InputState()
		{
			MoveForward = new InputAxis();
			MoveBackward = new InputAxis();
			MoveLeft = new InputAxis();
			MoveRight = new InputAxis();
			LookX = new InputAxis();
			LookY = new InputAxis();
			Quit = new InputAxis();
			Focus = new InputAxis();
			Jump = new InputAxis();
		}

		public void Commit()
		{
			MoveForward.Commit();
			MoveBackward.Commit();
			MoveLeft.Commit();
			MoveRight.Commit();
			LookX.Commit();
			LookY.Commit();
			Quit.Commit();
			Focus.Commit();
			Jump.Commit();
		}
	}
}
