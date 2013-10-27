using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Collision;

using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class PlayerShip : BaseEntity
    {
        public ImageRender Render;
        public Gun Gun;
        public Circle Shape;

        //Controls
        public DoubleInput UpButton, DownButton, RightButton, LeftButton, FireButton, BombButton;
        public GamePadAnalog MoveAnalog, LookAnalog;
        public GamePadTrigger ThrustTrigger, BrakeTrigger;

        public PlayerShip(IComponent parent, string name) : base(parent, name)
        {
            Body.X = 0;
            Body.Y = 0;

            Physics.Drag = 0.97f;
            Physics.AngularDrag = 0.9f;

            Render = new ImageRender(this, "Render");
            Render.LoadTexture(@"AsteroidsGame/ship");
            Render.Layer = .01f;
            Render.Scale = new Vector2(.128f);
            Render.Origin = new Vector2(Render.Texture.Width / 2f, Render.Texture.Height / 2f);
            Render.Link(ImageRender.DEPENDENCY_BODY, Body);
            
            Body.Bounds = Render.Bounds;

            Gun = new SimpleGun(this, "SimpleGun");
            Gun.Link(SimpleGun.DEPENDENCY_BODY, Body);

            Shape = new Circle(this, "Shape", Body.Width);
            Shape.Offset = new Vector2(Body.Width/2, Body.Height/2);
            Shape.Debug = true;
            Shape.Link(Circle.DEPENDENCY_BODY, Body);

            Collision.GroupMask.AddMask(0);
            Collision.PairMask.AddMask(1);
            Collision.Immovable = true;
            Collision.Link(Collision.DEPENDENCY_SHAPE, Shape);
            Shape.Link(Circle.DEPENDENCY_COLLISION, Collision);


            //Control
            UpButton = new DoubleInput(this, "UpButton", Keys.W, Buttons.DPadUp, PlayerIndex.One);
            DownButton = new DoubleInput(this, "DownButton", Keys.S, Buttons.DPadDown, PlayerIndex.One);
            LeftButton = new DoubleInput(this, "LeftButton", Keys.A, Buttons.DPadLeft, PlayerIndex.One);
            RightButton = new DoubleInput(this, "RightButton", Keys.D, Buttons.DPadRight, PlayerIndex.One);
            FireButton = new DoubleInput(this, "FireButton", Keys.Space, Buttons.A, PlayerIndex.One);
            BombButton = new DoubleInput(this, "BombButton", Keys.Z, Buttons.B, PlayerIndex.One);
            ThrustTrigger = new GamePadTrigger(this, "ThrustTrigger", Triggers.Right, PlayerIndex.One);
            //TODO: Add brake code
            BrakeTrigger = new GamePadTrigger(this, "BrakeTrigger", Triggers.Left, PlayerIndex.One);
            MoveAnalog = new GamePadAnalog(this, "MoveAnalog", Sticks.Left, PlayerIndex.One);
            LookAnalog = new GamePadAnalog(this, "LookAnalog", Sticks.Right, PlayerIndex.One);
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

            if (MoveAnalog.Position != Vector2.Zero)
            {
                Body.Angle = MathTools.Physics.GetAngle(new Vector2(-MoveAnalog.Position.X, -MoveAnalog.Position.Y));
            }
            if(ThrustTrigger.Value > 0) Physics.Thrust(ThrustTrigger.Value * _FLYSPEED);


            if (FireButton.RapidFire(50)) Gun.Fire();
        }
    }
}
