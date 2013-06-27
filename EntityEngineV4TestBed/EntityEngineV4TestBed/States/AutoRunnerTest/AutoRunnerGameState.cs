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

namespace EntityEngineV4TestBed.States.AutoRunnerTest
{
    public class AutoRunnerGameState : EntityState
    {
        private Player _player;
        private Building b;
        public AutoRunnerGameState(EntityGame eg) : base(eg, "AutoRunnerGameState")
        {
            Services.Add(new InputHandler(this));
            Services.Add(new MouseHandler(this));
            Services.Add(new CollisionHandler(this));

            _player= new Player(this);
            AddEntity(_player);

            b = new Building(this, "Building");
            //b.Body.Position = new Vector2(10, 570);
            //b.Body.Bounds.X = 500;
            //b.Image.Scale = b.Body.Bounds;
            AddEntity(b);
        }
    }
}
