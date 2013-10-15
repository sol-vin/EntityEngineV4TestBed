using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class ExampleShip : Entity
    {
        public Body Body;
        public Physics Physics;
        public ImageRender Render;
        public Gun Gun;

        //Controls
        public DoubleInput UpButton, DownButton, RightButton, LeftButton, FireButton, BombButton;
        public GamePadAnalog MoveAnalog, LookAnalog;
        public GamePadTrigger ThrustTrigger, BrakeTrigger;

        public ExampleShip(IComponent parent, string name)
            : base(parent, name)
        {
            Body = new Body(this, "Body");
            Body.X = 10;
            Body.Y = 10;

            Physics = new Physics(this, "Physics");
            Physics.Drag = 0.97f;
            Physics.AngularDrag = 0.9f;
            Physics.Link(Physics.DEPENDENCY_BODY, Body);

            Render = new ImageRender(this, "Render");
            Render.LoadTexture(@"AsteroidsGame/ship");
            Render.Layer = .01f;
            Render.Scale = new Vector2(.128f);
            Render.Origin = new Vector2(Render.Texture.Width / 2f, Render.Texture.Height / 2f);
            Render.Link(ImageRender.DEPENDENCY_BODY, Body);
            Body.Bounds = Render.Bounds;

            Gun = new SimpleGun(this, "SimpleGun");
            Gun.Link(SimpleGun.DEPENDENCY_BODY, Body);

            //Control
            UpButton = new DoubleInput(this, "UpButton", Keys.W, Buttons.DPadUp, PlayerIndex.One);
            DownButton = new DoubleInput(this, "DownButton", Keys.S, Buttons.DPadDown, PlayerIndex.One);
            LeftButton = new DoubleInput(this, "LeftButton", Keys.A, Buttons.DPadLeft, PlayerIndex.One);
            RightButton = new DoubleInput(this, "RightButton", Keys.D, Buttons.DPadRight, PlayerIndex.One);
            FireButton = new DoubleInput(this, "FireButton", Keys.Space, Buttons.A, PlayerIndex.One);
            ThrustTrigger = new GamePadTrigger(this, "ThrustTrigger", Triggers.Right, PlayerIndex.One);
            MoveAnalog = new GamePadAnalog(this, "MoveAnalog", Sticks.Left, PlayerIndex.One);
            LookAnalog = new GamePadAnalog(this, "LookAnalog", Sticks.Right, PlayerIndex.One);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            UpdateInput();
            UpdateOutOfBounds();
        }

        private const float _FLYSPEED = 0.3f;
        private const float _TURNSPEED = 0.01f;

        /// <summary>
        /// Checks the inputs to see if something needs to be triggered.
        /// </summary>
        private void UpdateInput()
        {
            if (UpButton.Down()) Physics.Thrust(_FLYSPEED);
            else if (DownButton.Down()) Physics.Thrust(-_FLYSPEED);

            if (LeftButton.Down()) Physics.AddAngularForce(-_TURNSPEED);
            else if (RightButton.Down()) Physics.AddAngularForce(_TURNSPEED);

            if (MoveAnalog.Position != Vector2.Zero)
            {
                Body.Angle = MathTools.Physics.GetAngle(new Vector2(-MoveAnalog.Position.X, -MoveAnalog.Position.Y));
            }
            if (ThrustTrigger.Value > 0) Physics.Thrust(ThrustTrigger.Value * _FLYSPEED);


            if (FireButton.RapidFire(100)) Gun.Fire();
        }

        /// <summary>
        /// Moves the ship to the other side of the screen if it moves out of bounds.
        /// </summary>
        private void UpdateOutOfBounds()
        {
            if (Body.Bottom < 0) Body.Y = EntityGame.Viewport.Height;
            else if (Body.Top > EntityGame.Viewport.Height) Body.Y = -Body.Height;
            if (Body.Right < 0) Body.X = EntityGame.Viewport.Width;
            else if (Body.Left > EntityGame.Viewport.Width) Body.X = -Body.Width;
        }
    }
}
