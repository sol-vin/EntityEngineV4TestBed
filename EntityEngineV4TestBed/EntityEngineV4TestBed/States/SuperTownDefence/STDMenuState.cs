using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.SuperTownDefence
{
    public class STDMenuState : TestBedState
    {
        private STDGameState _stdGameState;

        private Image _backgroundImage;
        private Label _startText;

        public STDMenuState(EntityGame eg)
            : base(eg, "STDMenuState")
        {
            _stdGameState = new STDGameState(Parent as EntityGame);
            _stdGameState.ChangeState += Show;
            ChangeState += _stdGameState.Show;

            AddService(new InputHandler(this));
            ControlHandler ch = new ControlHandler(this);
            ch.UseMouse = false;

            _backgroundImage = new Image(ch, "BGImage", EntityGame.Game.Content.Load<Texture2D>(@"SuperTownDefence/menu/background"));
            _backgroundImage.ImageRender.Scale = Vector2.One * 6;
            _backgroundImage.TabPosition = new Point(0, 0);
            _backgroundImage.ImageRender.Layer = 0.5f;
            ch.AddControl(_backgroundImage);

            _startText = new Label(ch, "StartText");
            _startText.TextRender.Color = Color.White;
            _startText.Text = "Press Start to begin the game!";
            _startText.TabPosition = new Point(0, 1);
            _startText.Body.Position = new Vector2(EntityGame.Viewport.Width / 2f - _startText.Body.Bounds.X / 2f, 550);
            _startText.TextRender.Layer = 1f;
            ch.AddControl(_startText);

            AddEntity(new STDMenuStateManager(this));

            AddService(ch);
        }
        private class STDMenuStateManager : Entity
        {
            private DoubleInput _startkey;

            public STDMenuStateManager(EntityState stateref)
                : base(stateref, "STDMenuStateManager")
            {
                _startkey = new DoubleInput(this, "StartKey", Keys.Enter, Buttons.Start, PlayerIndex.One);
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);

                if (_startkey.Released())
                    EntityGame.CurrentState.ChangeToState("STDGameState");
            }
        }
    }
}