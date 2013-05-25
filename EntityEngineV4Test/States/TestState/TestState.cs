using System;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework.Input;
using EntityEngineV4.Input.MouseInput;

namespace EntityEngineV4Test.States.TestState
{
	public class TestState : EntityState
	{
		private TestStateManager _tsm;

		public TestState(EntityGame parent) : base(parent, "TestState")
		{
			Initialize();
		}
	
		public void Initialize()
		{
			//Add our services
			Services.Add (new InputHandler(this));
			Services.Add (new MouseHandler(this));

			AddEntity(new TestObject(this, "TestObject"));
			_tsm = new TestStateManager(this, "TestStateManager");
			AddEntity(_tsm);
		}

		public void Reset()
		{
			Clear();
			Services.Clear();
		}

		public override void Update (GameTime gt)
		{
			if (_tsm.Reset) {
				Reset();
				Initialize();
			}
			base.Update (gt);
		}
	}
}

