﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using EntityEngineV4.Collision;

using EntityEngineV4.Collision.Shapes;
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
    public class PlayerShip : BaseEntity
    {
        public ImageRender Render;
        public Gun Gun;
        public Circle Shape;

        //Controls
        public DoubleInput UpButton, DownButton, RightButton, LeftButton, FireButton;
        public GamePadAnalog LookAnalog;
        public GamePadTrigger ThrustTrigger, GravityTrigger;

        public PlayerShip(Node parent, string name) : base(parent, name)
        {
            Body.X = 0;
            Body.Y = 0;

            Physics.Drag = 0.97f;
            Physics.AngularDrag = 0.9f;

            Render = new ImageRender(this, "Render");
            Render.SetTexture(GetState().GetService<AssetCollector>().GetAsset<Texture2D>("ship"));
            Render.Layer = .01f;
            Render.Scale = new Vector2(.128f);
            Render.Origin = new Vector2(Render.Texture.Width / 2f, Render.Texture.Height / 2f);
            Render.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);
            
            Body.Bounds = Render.Bounds;

            Gun = new SimpleGun(this, "SimpleGun");
            Gun.LinkDependency(SimpleGun.DEPENDENCY_BODY, Body);

            Shape = new Circle(this, "Circle", Body.Width*.8f);
            Shape.Offset = new Vector2(Body.Width/2, Body.Height/2);
            Shape.Debug = true;
            Shape.LinkDependency(Circle.DEPENDENCY_BODY, Body);

            Collision.GroupMask.AddMask(0);
            Collision.PairMask.AddMask(1);
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
            GravityTrigger = new GamePadTrigger(this, "GravityTrigger", Triggers.Left, PlayerIndex.One);

            LookAnalog = new GamePadAnalog(this, "LookAnalog", Sticks.Left, PlayerIndex.One);
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
                Body.Angle = MathTools.Physics.GetAngle(new Vector2(-LookAnalog.Position.X, -LookAnalog.Position.Y));
            }

            if(ThrustTrigger.Value > ThrustTrigger.DeadZone) Physics.Thrust(ThrustTrigger.Value * _FLYSPEED);

            if(GravityTrigger.Value > GravityTrigger.DeadZone) GravityWell(GravityTrigger.Value);

            if (FireButton.RapidFire(50)) Gun.Fire();
        }


        public const float MAXGRAVITYDISTANCE = 400f;
        public const float GRAVITYSTRENGTH = 3f;
        private void GravityWell(float strength)
        {
            foreach (var bullet in Gun.Bullets)
            {
                Vector2 normal = bullet.Body.Position - Body.Position;
                if (normal.LengthSquared() > MAXGRAVITYDISTANCE*MAXGRAVITYDISTANCE) return;

                float distance = normal.Length();
                float gravity = 1f - distance/MAXGRAVITYDISTANCE;

                normal.Normalize();

                bullet.Physics.Velocity -= normal*gravity*GRAVITYSTRENGTH*strength;
                bullet.Physics.Velocity *= .95f;
            }
        }
    }
}
