using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input.MouseInput;
using EntityEngineV4.PowerTools;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4TestBed.States.PrimitiveTest
{
    public class PrimitiveTestState : TestBedState
    {
        //DrawingTools.Primitives
        private DrawingTools.Rectangle _r1;

        private DrawingTools.Line _lx1, _lx2, _lx3, _ly1, _ly2, _ly3;
        private DrawingTools.Triangle _triangleEquilateral, _triangleIsosoles, _triangleRight;

        //GUI
        private Label _drawingtoolsTitle, _renderTitle;

        private DrawingTools.Line _middleLine;

        public PrimitiveTestState()
            : base("PrimitiveTestState")
        {
        }

        public override void Create()
        {
            base.Create();

            ControlHandler ch = new ControlHandler(this);

            //Add our labels to the top
            _drawingtoolsTitle = new Label(ch, "drawingtoolsTitle");
            _drawingtoolsTitle.Text = "DrawingTools.Primitives";
            _drawingtoolsTitle.Body.Position = new Vector2(EntityGame.Viewport.Width / 4 - _drawingtoolsTitle.TextRender.DrawRect.Width / 2, 15);
            _drawingtoolsTitle.TabPosition = new Point(0, 0);
            ch.AddControl(_drawingtoolsTitle);

            _renderTitle = new Label(ch, "renderTitle");
            _renderTitle.Text = "Rendering.Primitives";
            _renderTitle.Body.Position = new Vector2(EntityGame.Viewport.Width - (EntityGame.Viewport.Width / 4) - _renderTitle.TextRender.DrawRect.Width / 2, 15);
            _renderTitle.TabPosition = new Point(1, 0);
            ch.AddControl(_renderTitle);

            _middleLine = new DrawingTools.Line(new Vector2(EntityGame.Viewport.Width / 2f, 10), new Vector2(EntityGame.Viewport.Width / 2f, EntityGame.Viewport.Height - 10));

            //Setup our prmitives
            _r1 = new DrawingTools.Rectangle(20, 40, 260, 60);
            _r1.Color = Color.Salmon;
            _r1.Fill = true;

            //Horizontal Lines
            _lx1 = new DrawingTools.Line(new Vector2(20, 105), new Vector2(270, 105), Color.Red);
            _lx1.Thickness = 1;

            _lx2 = new DrawingTools.Line(new Vector2(20, 110), new Vector2(270, 110), Color.Orange);
            _lx2.Thickness = 2;

            _lx3 = new DrawingTools.Line(new Vector2(20, 115), new Vector2(270, 115), Color.Yellow);
            _lx3.Thickness = 3;

            _ly1 = new DrawingTools.Line(new Vector2(270, 120), new Vector2(270, 290), Color.MediumPurple);
            _ly1.Thickness = 1;

            _ly2 = new DrawingTools.Line(new Vector2(275, 120), new Vector2(275, 290), Color.DodgerBlue);
            _ly2.Thickness = 2;

            _ly3 = new DrawingTools.Line(new Vector2(280, 120), new Vector2(280, 290), Color.LawnGreen);
            _ly3.Thickness = 3;

            //Add component based classes
            new SpinningRect(this, "Spinning", EntityGame.Viewport.Width/2f + 75, 200, 30, 30, false, 3);
            new SpinningRect(this, "Spinning", EntityGame.Viewport.Width / 2f + 75, 200, 30, 30, false, 2);
            new SpinningRect(this, "Spinning", EntityGame.Viewport.Width / 2f + 75, 200, 30, 30, false, 1);
            new SpinningRect(this, "Spinning", EntityGame.Viewport.Width / 2f + 150 + 75, 200, 30, 30, true);

            new SpinningRect(this, "Spinning", EntityGame.Viewport.Width / 2f + 75, 300, 70, 30, false, 3);
            new SpinningRect(this, "Spinning", EntityGame.Viewport.Width / 2f + 75, 300, 70, 30, false, 2);
            new SpinningRect(this, "Spinning", EntityGame.Viewport.Width / 2f + 75, 300, 70, 30, false, 1);
            new SpinningRect(this, "Spinning", EntityGame.Viewport.Width / 2f + 150 + 75, 300, 30, 70, true);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            //Draw GUI Elements
            _middleLine.Draw(sb);

            //Draw Primitives
            _r1.Draw(sb);

            //Horizontal Lines
            _lx1.Draw(sb);
            _lx2.Draw(sb);
            _lx3.Draw(sb);

            //Vertical Lines
            _ly1.Draw(sb);
            _ly2.Draw(sb);
            _ly3.Draw(sb);
        }

        private class PrimitiveTestEntity : Entity
        {
            public Body Body;
            public Physics Physics;

            public PrimitiveTestEntity(IComponent parent, string name)
                : base(parent, name)
            {
                Body = new Body(this, "Body");
                Physics = new Physics(this, "Physics", Body);
                Physics.AngularVelocity = .01f;
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
            }
        }

        private class SpinningRect : PrimitiveTestEntity
        {
            private ShapeTypes.Rectangle Rectangle;

            public SpinningRect(IComponent parent, string name, float x, float y, float width, float height, bool fill = false, float thickness = 3)
                : base(parent, name)
            {
                Body.X = x;
                Body.Y = y;
                Body.Width = width;
                Body.Height = height;
                Rectangle = new ShapeTypes.Rectangle(this, "Rectangle", Body, fill);

                Rectangle.Thickness = thickness;
                Rectangle.Debug = true;
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
            }
        }
    }
}