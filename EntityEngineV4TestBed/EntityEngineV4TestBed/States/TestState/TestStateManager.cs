using System;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed
{
	public class TestStateManager : Entity
	{
		private DoubleInput _resetkey;

		private bool _reset;
		public bool Reset {
			get { return _reset;}
		}
		public TestStateManager (EntityState stateref, string name) 
			: base(stateref, stateref, name)
		{
			_resetkey = new DoubleInput(this, "ResetKey", Keys.Escape, Buttons.Start, PlayerIndex.One);
		}
		public override void Update (GameTime gt)
		{
			_reset = _resetkey.Pressed();
			base.Update (gt);
		}
	}
}

