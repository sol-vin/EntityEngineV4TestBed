using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4TestBed.States.PrimitiveTest
{
    public class PrimitiveTestState : TestBedState
    {
        private DrawingTools.PrimitiveHandler _primitiveHandler;
        private Path _path = new Path();

        public PrimitiveTestState(EntityGame eg) : base(eg, "PrimitiveTestState")
        {
            _primitiveHandler = new DrawingTools.PrimitiveHandler(this);
            AddService(_primitiveHandler);

            AddLine(10, 10, 80, 80, 4, Color.Red);
            AddLine(10, 14, 50, 110, 3, Color.Orange);
            AddLine(10,60, 60, 10, 2, Color.Yellow);
            AddTriangle(new Vector2(300, 300), new Vector2(370, 280), new Vector2(335, 360), 1, Color.Crimson);

            _primitiveHandler.AddPrimitive(new DrawingTools.Rectangle(400, 400, 40, 20)
                {
                    Thickness = 1,
                    Color = Color.LimeGreen,
                    Fill = true
                });

            _primitiveHandler.AddPrimitive(new DrawingTools.Rectangle(400, 300, 40, 20)
            {
                Thickness = 10,
                Color = Color.Red,
                Fill = false
            });

            AddEntity(new PrimitiveTestEntity(this, "test1"));

            _path.AddPoint(new PathPoint(200,300));
            _path.AddPoint(new PathPoint(300,300));
            _path.AddPoint(new PathPoint(300,350));
            _path.AddPoint(new PathPoint(200,300));
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            _path.DrawDebug(sb);
        }

        public void AddLine(int x1, int y1, int x2, int y2, float thickness, Color color)
        {
            _primitiveHandler.AddPrimitive(new DrawingTools.Line(new Vector2(x1, y1), new Vector2(x2, y2), color){Thickness = thickness});
        }

        public void AddTriangle(Vector2 p1, Vector2 p2, Vector2 p3, float thickness, Color color)
        {
            _primitiveHandler.AddPrimitive(new DrawingTools.Triangle(p1,p2,p3,color) { Thickness = thickness});
        }

        private class PrimitiveTestEntity : Entity
        {
            public ShapeTypes.Rectangle Rectangle;

            public PrimitiveTestEntity(EntityState stateref, string name) : base(stateref, name)
            {
                Rectangle = new ShapeTypes.Rectangle(this, "Rect", 100,200, 40, 60);
                Rectangle.Thickness = 3;
                Rectangle.Color = Color.MediumAquamarine;
                Rectangle.Angle = MathHelper.PiOver4;
            }
        }
    }
}
