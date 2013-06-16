using System;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using EntityEngineV4.Object;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.FancyParticleTest
{
    public class FancyParticleTestState : TestBedState
    {
        public FancyParticleTestState(EntityGame eg)
            : base(eg, "FancyParticleTest")
        {
            Services.Add(new InputHandler(this));
            Services.Add(new MouseHandler(this));

            AddEntity(new FancyEntity(this, "FE"));
        }

        private class FancyEntity : Entity
        {
            public FancyEmitter FancyEmitterMouse, FancyEmitterAuto;
            public Body MouseBody, AutoBody;
            private Random _rand = new Random();

            private DoubleInput _emitkey;

            public FancyEntity(EntityState stateref, string name)
                : base(stateref, name)
            {
                MouseBody = new Body(this, "MouseBody");
                AutoBody = new Body(this, "AutoBody");
                FancyEmitterMouse = new FancyEmitter(this, "FancyEmitterMouse", MouseBody);
                FancyEmitterMouse.Color = Color.Red;

                FancyEmitterAuto = new FancyEmitter(this, "FancyEmitterAuto", AutoBody);
                FancyEmitterAuto.Color = Color.CornflowerBlue;
                FancyEmitterAuto.AutoEmit = true;
                FancyEmitterAuto.AutoEmitAmount = 1;

                _emitkey = new DoubleInput(this, "emitkey", Keys.Space, Buttons.A, PlayerIndex.One);
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);

                AutoBody.Position = new Vector2(_rand.Next(40, EntityGame.Viewport.Width - 40), -30);

                if (MouseHandler.IsMouseButtonDown(MouseButton.RightButton))
                {
                    MouseBody.Position = new Vector2(MouseHandler.Cursor.Position.X, MouseHandler.Cursor.Position.Y);
                    FancyEmitterMouse.Emit(1);
                }
                if (_emitkey.Released()) FancyEmitterAuto.AutoEmit = !FancyEmitterAuto.AutoEmit;
            }
        }

        private class FancyEmitter : Emitter
        {
            private Random _rand = new Random();
            private Body _body;
            public Color Color = Color.White;

            public FancyEmitter(Entity e, string name, Body body)
                : base(e, name, Assets.Pixel)
            {
                _body = body;
            }

            protected override Particle GenerateNewParticle()
            {
                var p = new ExplodingParticle(Parent.StateRef, 2000, this, Color);
                p.Body.Bounds = new Vector2(5, 5);
                p.Body.Position = _body.Position;

                p.Body.Angle = (float)_rand.NextDouble() * MathHelper.TwoPi;

                float thrust = (float)_rand.NextDouble() * 4f;
                while (Math.Abs(thrust) < .00001f)
                {
                    thrust = (float)_rand.NextDouble() * 4f;
                }
                p.Physics.Thrust(thrust);
                p.Physics.Acceleration = new Vector2(0, .1f);
                p.ImageRender.Scale = new Vector2(5, 5);
                return p;
            }

            private class ExplodingParticle : Particle
            {
                private int _floor = EntityGame.Viewport.Height - 20;

                public Physics Physics;
                public ImageRender ImageRender;

                public ExplodingParticle(EntityState stateref, int ttl, Emitter e, Color color)
                    : base(stateref, ttl, e)
                {
                    Physics = new Physics(this, "Physics", Body);

                    ImageRender = new ImageRender(this, "ImageRender", e.Texture, Body);
                    ImageRender.Color = color;

                    Emitter = new GibEmitter(this, "Emitter", Body, Physics, color);
                }

                public override void Update(GameTime gt)
                {
                    base.Update(gt);
                    if (Body.BoundingRect.Bottom > _floor)
                    {
                        //Find penetration depth
                        float depth = Body.Bottom - _floor;

                        //Move out of the floor, add a little extra for safety
                        Body.Position.Y -= depth + .1f;
                        Emitter.Emit(20);
                        Destroy();
                    }
                }

                private class GibEmitter : Emitter
                {
                    private Random _rand = new Random(DateTime.Now.Millisecond ^ DateTime.Now.Second);
                    private Body _body;
                    private Physics _physics;
                    private Color _color;

                    public GibEmitter(Entity parent, string name, Body body, Physics physics, Color color)
                        : base(parent, name, Assets.Pixel)
                    {
                        _body = body;
                        _physics = physics;
                        _color = color;
                    }

                    protected override Particle GenerateNewParticle()
                    {
                        var p = new GibParticle(Parent.StateRef, this);
                        p.Body.Bounds = new Vector2(2, 2);
                        p.Body.Position = _body.Position;

                        int sign = _rand.Next(0, 2) == 0 ? -1 : 1;
                        p.Body.Angle = (float) _rand.NextDouble()*MathHelper.PiOver2 * sign;

                        float thrust = ((float)_rand.NextDouble() + 1f) * (_physics.Velocity.Y/4);
                        p.Physics.Thrust(thrust);
                        p.Physics.Acceleration = new Vector2(0, .1f);
                        p.ImageRender.Scale = new Vector2(2, 2);
                        p.ImageRender.Color = _color;
                        return p;
                    }
                }

                private class GibParticle : FadeParticle
                {
                    private int _floor = EntityGame.Viewport.Height - 20;

                    public Physics Physics;
                    public ImageRender ImageRender;

                    public GibParticle(EntityState stateref, Emitter e)
                        : base(stateref, 3000, e)
                    {
                        FadeAge = TimeToLive / 5 * 4;
                        Physics = new Physics(this, "Physics", Body);
                        ImageRender = new ImageRender(this, "ImageRender", e.Texture, Body);
                        Render = ImageRender;
                    }

                    public override void Update(GameTime gt)
                    {
                        base.Update(gt);

                        if (!EntityGame.Viewport.Intersects(Body.BoundingRect))
                            Destroy();

                        if (Body.BoundingRect.Bottom > _floor)
                        {
                            //Find penetration depth
                            float depth = Body.Bottom - _floor;

                            //Move out of the floor, add a little extra for safety
                            Body.Position.Y -= depth + .1f;
                            Physics.Velocity.Y = -Physics.Velocity.Y * .2f; //Add restitution
                        }
                        //Add friction if it's on the ground
                        if (Math.Abs(Physics.Velocity.Y) < .001f && (Body.Bottom < _floor + 1))
                        {
                            Physics.Velocity.X *= .9f;
                            Physics.Velocity.Y = 0;
                        }
                    }
                }
            }
        }
    }
}