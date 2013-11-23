using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using EntityEngineV4.CollisionEngine;

using EntityEngineV4.CollisionEngine.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Engine.Services;
using EntityEngineV4.Input;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class PlayerShip : BaseNode
    {
        public ImageRender Render;
        public Gun Gun;
        public Circle Shape;

        //Controls
        public DoubleInput UpButton, DownButton, RightButton, LeftButton, FireButton;
        public GamePadAnalog LookAnalog;
        public GamePadTrigger ThrustTrigger;

        public PlayerShip(Node parent, string name) : base(parent, name)
        {
            Body.X = EntityGame.Viewport.Width/2f;
            Body.Y = EntityGame.Viewport.Height/2f;

            Physics.Drag = 0.97f;
            Physics.AngularDrag = 0.9f;

            Render = new ImageRender(this, "Render");
            Render.SetTexture(GetRoot<State>().GetService<AssetCollector>().GetAsset<Texture2D>("ship"));
            Render.Layer = .01f;
            Render.Scale = new Vector2(.128f);
            Render.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);
            
            Body.Bounds = Render.Bounds;
            Body.Origin = new Vector2(Render.Texture.Width / 2f, Render.Texture.Height / 2f);


            Gun = new SimpleGun(this, "SimpleGun");
            Gun.LinkDependency(SimpleGun.DEPENDENCY_BODY, Body);
            Gun.LinkDependency(SimpleGun.DEPENDENCY_PHYSICS, Physics);
            Shape = new Circle(this, "Circle", Body.Width*.8f);
            Shape.Offset = new Vector2(Body.Width/2, Body.Height/2);
            Shape.Debug = true;
            Shape.LinkDependency(Circle.DEPENDENCY_BODY, Body);

            Collision.Group.AddMask(0);
            Collision.Pair.AddMask(2);
            Collision.CollideEvent += OnCollide;
            Collision.Immovable = true;
            Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, Shape);
            Shape.LinkDependency(Circle.DEPENDENCY_COLLISION, Collision);


            //Control
            UpButton = new DoubleInput(this, "UpButton", Keys.W, Buttons.DPadUp, PlayerIndex.One);
            DownButton = new DoubleInput(this, "DownButton", Keys.S, Buttons.DPadDown, PlayerIndex.One);
            LeftButton = new DoubleInput(this, "LeftButton", Keys.A, Buttons.DPadLeft, PlayerIndex.One);
            RightButton = new DoubleInput(this, "RightButton", Keys.D, Buttons.DPadRight, PlayerIndex.One);
            FireButton = new DoubleInput(this, "FireButton", Keys.Space, Buttons.A, PlayerIndex.One);
            ThrustTrigger = new GamePadTrigger(this, "ThrustTrigger", Triggers.Right, PlayerIndex.One);
            //GravityTrigger = new GamePadTrigger(this, "GravityTrigger", Triggers.Left, PlayerIndex.One);

            LookAnalog = new GamePadAnalog(this, "LookAnalog", Sticks.Left, PlayerIndex.One);
        }

        public override void Initialize()
        {
            base.Initialize();
            //EntityGame.Camera.FollowPoint(Body);
            //EntityGame.Camera.FollowOffset = new Vector2(Body.Width/2f, Body.Height/2f);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            UpdateInput();
        }

        private const float _FLYSPEED = 0.3f;
        private const float _TURNSPEED = 0.01f;
        private void UpdateInput()
        {
            if(UpButton.Down()) Physics.Thrust(_FLYSPEED);
            else if (DownButton.Down()) Physics.Thrust(-_FLYSPEED);

            if (LeftButton.Down()) Physics.AddAngularForce(-_TURNSPEED);
            else if (RightButton.Down()) Physics.AddAngularForce(_TURNSPEED);

            if (LookAnalog.Position != Vector2.Zero)
            {
                Body.Angle = Physics.GetAngle(new Vector2(-LookAnalog.Position.X, -LookAnalog.Position.Y));
            }

            if(ThrustTrigger.Value > ThrustTrigger.DeadZone) Physics.Thrust(ThrustTrigger.Value * _FLYSPEED);

            if (FireButton.RapidFire(250)) Gun.Fire();
        }

        public void OnCollide(Manifold m)
        {
            Node otherNode = m.A != Collision ? m.A : m.B;
            if (otherNode.Parent is Asteroid)
            {
                Destroy();
            }
        }
    }
}
