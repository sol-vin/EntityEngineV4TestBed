using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Collision;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using EntityEngineV4TestBed.States.AutoRunnerTest.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.AutoRunnerTest
{
    public class AutoRunnerGameState : EntityState
    {
        private Player _player;
        private Building b;
        public AutoRunnerGameState(EntityGame eg) : base(eg, "AutoRunnerGameState")
        {
            AddService(new InputHandler(this));
            AddService(new MouseHandler(this));
            AddService(new CollisionHandler(this));
            AddEntity(new AutoRunnerGameManager(this));
            _player= new Player(this);
            AddEntity(_player);

            b = new Building(this, "Building");
            AddEntity(b);
        }

        public class AutoRunnerGameManager : Entity
        {
            public DoubleInput BackKey;
            public AutoRunnerGameManager(EntityState stateref) : base(stateref, "AutoRunnerGameManager")
            {
                BackKey = new DoubleInput(this, "BackKey", Keys.Back, Buttons.Back, PlayerIndex.One);
            }

            public override void Update(GameTime gt)
            {
                if (BackKey.Released())
                {
                    if (Parent == null) throw new Exception("Parent is null!");
                    else
                    {
                        (Parent as EntityState).Show("AutoRunnerMenuState");
                    }
                }
                base.Update(gt);
            }
        }
    }
}
