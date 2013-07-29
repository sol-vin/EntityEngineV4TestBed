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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.AutoRunnerTest.Objects
{
    public class Player : Entity
    {
        public Body Body;
        public Physics Physics;
        public ImageRender Image;
        public Collision Collision;

        public DoubleInput Up, Down, Left, Right, ZoomIn, ZoomOut, CameraModifier;

        public Camera Camera;

        public bool CanJump
        {
            get { return Math.Abs(Body.Delta.Y) < .001; }
        }

        public Player(EntityState stateref) : base(stateref, "Player")
        {
            Body = new Body(this, "Body");
            Body.Position = new Vector2(70, 100);
            Body.Bounds = new Vector2(30, 50);

            Physics = new Physics(this, "Physics", Body);
            //Physics.Acceleration.Y = .1f; //Gravity

            Image = new ImageRender(this, "Image", Assets.Pixel, Body);
            Image.Scale = Body.Bounds;
            Image.Color = Color.Red;

            Collision = new Collision(this, "Collision", new AABB(), Body, Physics);
            Collision.GroupMask.AddMask(0);
            Collision.PairMask.AddMask(0);
            Collision.CollideEvent += OnCollision;
            Collision.Debug = true;

            //Camera.Position= new Vector2(Body.Position.X + EntityGame.Viewport.Width/2f - 30, -200);
            Camera = new Camera(this, "Camera");
            Camera.View();

            //TODO: Remove these
            CameraModifier = new DoubleInput(this, "CameraMod", Keys.LeftShift, Buttons.RightShoulder, PlayerIndex.One);
            Up = new DoubleInput(this, "Up", Keys.Up, Buttons.DPadUp, PlayerIndex.One);
            Down = new DoubleInput(this, "Down", Keys.Down, Buttons.DPadDown, PlayerIndex.One);
            Left = new DoubleInput(this, "Left", Keys.Left, Buttons.DPadLeft, PlayerIndex.One);
            Right = new DoubleInput(this, "Right", Keys.Right, Buttons.DPadRight, PlayerIndex.One);
            ZoomIn = new DoubleInput(this, "Zoom+", Keys.W, Buttons.LeftShoulder, PlayerIndex.One);
            ZoomOut = new DoubleInput(this, "Zoom-", Keys.S, Buttons.LeftTrigger, PlayerIndex.One);
        }

        public override void Update(GameTime gt)
        {
            if(!Collision.IsColliding)
                Image.Color = Color.Red;
            base.Update(gt);
            Camera.Position += Body.Delta;
            if (CameraModifier.Up())
            {
                if (Up.Down())
                    Body.Position.Y -= 1f;
                else if (Down.Down())
                    Body.Position.Y += 1f;

                if (Left.Down())
                    Body.Position.X -= 1f;
                else if (Right.Down())
                    Body.Position.X += 1f;
            }
            else if (CameraModifier.Down())
            {
                if (Up.Down())
                    Camera.Position.Y -= 1f;
                else if (Down.Down())
                    Camera.Position.Y += 1f;

                if (Left.Down())
                    Camera.Position.X -= 1f;
                else if (Right.Down())
                    Camera.Position.X += 1f;
            }
            if (ZoomIn.Down())
                Camera.Zoom += .05f;
            else if (ZoomOut.Down())
                Camera.Zoom -= .05f;

            //if (Camera.Position.Y > EntityGame.Viewport.Height/2f - 200)
            //{
            //    Camera.Position.Y = EntityGame.Viewport.Height/2f - 200;
            //}
        }

        public void Jump()
        {
            Physics.AddForce(-Vector2.UnitY * 4);
        }

        public void OnCollision(Collision c)
        {
            Image.Color = Color.Yellow;
            //Building b = c.Parent as Building;
            ////If we have a bottom collision
            //if (Collision.CollisionDirection.HasMatchingBit(CollisionHandler.DOWN))
            //{
            //    float penetrationdepth = Body.BoundingRect.Bottom - b.Body.BoundingRect.Top;
            //    Body.Position.Y -= penetrationdepth - .1f;
            //    Physics.Velocity.Y = 0;
            //}

            ////if the collision is on the sides
            //else if (Collision.CollisionDirection.HasMatchingBit(CollisionHandler.LEFT+CollisionHandler.RIGHT))
            //{
            //    float penetrationdepth = 0f;
            //    if (Collision.CollisionDirection.HasMatchingBit(CollisionHandler.LEFT))
            //    {
            //        penetrationdepth = b.Body.BoundingRect.Right - Body.BoundingRect.Left;
            //    }
            //    else
            //        penetrationdepth = Body.BoundingRect.Right - b.Body.BoundingRect.Left;
            //    Body.Position.X += penetrationdepth;
            //    Physics.Velocity.X = 0;
            //}
        }
    }
}
