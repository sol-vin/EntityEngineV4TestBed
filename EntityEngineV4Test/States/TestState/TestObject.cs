using System;

using EntityEngineV4.Engine;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace EntityEngineV4Test.States.TestState
{
	public class TestObject : Entity
	{
		public Body Body;
		public Physics Physics;
		public ImageRender ImageRender;

		private DoubleInput _leftkey, _rightkey, _upkey, _downkey;
		private GamePadTrigger _gamepadtrigger;

		public TestObject (EntityState stateref, string name) 
			: base(stateref, stateref, name)
		{
			Body = new Body(this, "Body");

			Physics = new Physics(this, "Physics", Body);
			Physics.Drag = 0.9f;

			ImageRender = new ImageRender(this, "ImageRender", Body);
			ImageRender.LoadTexture(@"TestState/testobject");

			_leftkey = new DoubleInput(this, "LeftKey", Keys.Left, Buttons.DPadLeft, PlayerIndex.One);
			_rightkey = new DoubleInput(this, "RightKey", Keys.Right, Buttons.DPadRight, PlayerIndex.One);
			_downkey = new DoubleInput(this, "DownKey", Keys.Down, Buttons.DPadDown, PlayerIndex.One);
			_upkey = new DoubleInput(this, "UpKey", Keys.Up, Buttons.DPadUp, PlayerIndex.One);

			_gamepadtrigger = new GamePadTrigger(this, "GamePadTrigger", Triggers.Left, PlayerIndex.One);
		}

		public override void Update (GameTime gt)
		{
			if(_leftkey.Down())
				Physics.AddForce(-Vector2.UnitX * 1);
			else if(_rightkey.Down())
				Physics.AddForce(Vector2.UnitX * 1);

			if(_upkey.Down())
				Physics.AddForce(-Vector2.UnitY * 1);
			else if(_downkey.Down())
				Physics.AddForce(Vector2.UnitY * 1);

			if(_gamepadtrigger.Pressed())
				this.Destroy();
			base.Update (gt);
		}
	}
}

