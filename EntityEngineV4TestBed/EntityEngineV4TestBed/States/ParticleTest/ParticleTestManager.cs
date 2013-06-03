using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using EntityEngineV4.Object;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.ParticleTest
{
    public class ParticleTestManager : Entity
    {
        private ControlHandler _controlHandler;
        public TestEmitter Emitter;
        private Body _body;

        private GamePadAnalog _moveCursor;
        private GamePadInput _emitButton;

        private DoubleInput _upkey, _downkey, _leftkey, _rightkey, _selectkey;

        public ParticleTestManager(EntityState stateref, ControlHandler ch) : base(stateref, "ParticleTestManager")
        {
            _controlHandler = ch;
            _body = new Body(this, "EmitterBody");
            Texture2D emitterTexture = EntityGame.Game.Content.Load<Texture2D>(@"ParticleTestState/particles");
            Emitter = new TestEmitter(this, "Emitter", emitterTexture, Vector2.One * 2, _body);
            
            _moveCursor = new GamePadAnalog(this, "MoveCursor", Sticks.Left, PlayerIndex.One);
            _emitButton = new GamePadInput(this, "EmitButton", Buttons.B, PlayerIndex.One);
            _upkey = new DoubleInput(this, "UpKey", Keys.Up, Buttons.DPadUp, PlayerIndex.One);
            _downkey = new DoubleInput(this, "DownKey", Keys.Down, Buttons.DPadDown, PlayerIndex.One);
            _leftkey = new DoubleInput(this, "LeftKey", Keys.Left, Buttons.DPadLeft, PlayerIndex.One);
            _rightkey = new DoubleInput(this, "RightKey", Keys.Right, Buttons.DPadRight, PlayerIndex.One);
            _selectkey = new DoubleInput(this, "SelectKey", Keys.Space, Buttons.A, PlayerIndex.One);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            MouseHandler.Cursor.Position = new Vector2(MouseHandler.Cursor.Position.X + (_moveCursor.Position.X*5),
                MouseHandler.Cursor.Position.Y - (_moveCursor.Position.Y*5));

            if(MouseHandler.IsMouseButtonDown(MouseButton.RightButton) || _emitButton.Down())
                Emitter.Emit(30);

            if (_upkey.Released())
                _controlHandler.UpControl();
            else if (_downkey.Released())
                _controlHandler.DownControl();
            else if (_leftkey.Released())
                _controlHandler.LeftControl();
            else if (_rightkey.Released())
                _controlHandler.RightControl();
            if (_selectkey.Released())
                _controlHandler.Select();
        }

        public class TestEmitter : Emitter
        {
            Random _random = new Random();
            private float _stength = 4f;
            public float Strength
            {
                get { return _stength; }
                set 
                {
                    _stength = value < 0 ? 0 : value;
                }
            }

            public Vector2 Acceleration = new Vector2(0,.2f);

            public TestEmitter(Entity e, string name, Texture2D texture, Vector2 tilesize, Body body) : base(e, name, texture, tilesize, body)
            {
            }

            protected override Particle GenerateNewParticle()
            {
                var p = new TestParticle(Parent.StateRef, Parent.StateRef, this);
                p.TileRender.Index = _random.Next(0, 10);
                p.Body.Position = new Vector2(MouseHandler.Cursor.Position.X, MouseHandler.Cursor.Position.Y);
                p.Body.Angle = (float)_random.NextDouble()*MathHelper.TwoPi;

                float thrust = (float)_random.NextDouble() * Strength;
                while (Math.Abs(thrust - 0) < .00001f)
                {
                    thrust = (float) _random.NextDouble()*Strength;
                }
                p.Physics.Thrust(thrust);
                p.Physics.AngularVelocity = (float) _random.NextDouble()*2f;
                p.Physics.Acceleration = Acceleration;
                p.TileRender.Scale = Vector2.One*3f*(float)_random.NextDouble()+Vector2.One;
                p.TileRender.Layer = 0f;
                return p;
            }
        }

        private class TestParticle : FadeParticle
        {
            public TestParticle(EntityState stateref, IComponent parent, Emitter e) : base(stateref, parent, 3000, e)
            {
                FadeAge = 1000;
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
                if(Body.Position.Y > EntityGame.Viewport.Height + 20)
                    Destroy();
            }
        }
    }
}
