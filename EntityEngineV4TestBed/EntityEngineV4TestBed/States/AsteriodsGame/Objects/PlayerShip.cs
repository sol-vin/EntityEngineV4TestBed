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
    public class PlayerShip : BaseEntity
    {
        public ImageRender Render;
        public Gun Gun;

        //Controls
        public DoubleInput UpButton, DownButton, RightButton, LeftButton, FireButton, BombButton;
        public GamePadAnalog MoveAnalog, LookAnalog;
        public GamePadTrigger ThrustTrigger;

        public PlayerShip(IComponent parent, string name) : base(parent, name)
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
            Render.Scale = new Vector2(.128f);
            Render.Origin = new Vector2(Render.Texture.Width/2f, Render.Texture.Height/2f);
            Render.Link(ImageRender.DEPENDENCY_BODY, Body);
            Body.Width = Render.DrawRect.Width;
            Body.Height = Render.DrawRect.Height;

            Gun = new SimpleGun(this, "SimpleGun");
            Gun.Link(SimpleGun.DEPENDENCY_BODY, Body);

            //Control
            UpButton = new DoubleInput(this, "UpButton", Keys.W, Buttons.DPadUp, PlayerIndex.One);
            DownButton = new DoubleInput(this, "DownButton", Keys.S, Buttons.DPadDown, PlayerIndex.One);
            LeftButton = new DoubleInput(this, "LeftButton", Keys.A, Buttons.DPadLeft, PlayerIndex.One);
            RightButton = new DoubleInput(this, "RightButton", Keys.D, Buttons.DPadRight, PlayerIndex.One);
            FireButton = new DoubleInput(this, "FireButton", Keys.Space, Buttons.LeftTrigger, PlayerIndex.One);
            BombButton = new DoubleInput(this, "BombButton", Keys.Z, Buttons.RightShoulder, PlayerIndex.One);
            ThrustTrigger = new GamePadTrigger(this, "ThrustTrigger", Triggers.Right, PlayerIndex.One);
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

            if (LeftButton.Down()) Physics.AddAngularForce(_TURNSPEED);
            else if (RightButton.Down()) Physics.AddAngularForce(-_TURNSPEED);

            if(FireButton.RapidFire(250))Gun.Fire();

            if (MoveAnalog.Position != Vector2.Zero)
            {
                Body.Angle = MathTools.Physics.GetAngle(new Vector2(-MoveAnalog.Position.X, -MoveAnalog.Position.Y));
            }
            if(ThrustTrigger.Value > 0) Physics.Thrust(ThrustTrigger.Value * _FLYSPEED); 
        }
    }
}
