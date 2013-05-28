using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4TestBed.States.TestState
{
    public class TestControl : Control
    {
        public TextRender TextRender;

        public TestControl(EntityState stateref, string name)
            : base(stateref, name)
        {
            TextRender = new TextRender(this, "TextRender", Assets.Font, name, Body);
            TextRender.Color = Color.Black;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        public override void OnFocusLost(Control c)
        {
            base.OnFocusLost(c);
            TextRender.Color = Color.Black;
        }

        public override void OnFocusGain(Control c = null)
        {
            base.OnFocusGain(c);
            TextRender.Color = Color.Red;
        }
    }
}