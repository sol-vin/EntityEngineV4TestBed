using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4TestBed.States.SuperTownDefence
{
    public class STDMenuState : TestBedState
    {
        private STDGameState _stdGameState;

        private Image _backgroundImage;
        private Label _startText;
        public STDMenuState(EntityGame eg) : base(eg, "STDMenuState")
        {
            _stdGameState = new STDGameState(Parent);
            _stdGameState.ChangeState += Show;
            ChangeState += _stdGameState.Show;

            Services.Add(new InputHandler(this));
            ControlHandler ch = new ControlHandler(this);
            ch.UseMouse = false;

            _backgroundImage = new Image(this, "BGImage", EntityGame.Game.Content.Load<Texture2D>(@"SuperTownDefence/menu/background"));
            _backgroundImage.ImageRender.Scale = Vector2.One*6;
            _backgroundImage.TabPosition = new Point(0, 0);
            _backgroundImage.ImageRender.Layer = 0.5f;
            ch.AddControl(_backgroundImage);

            _startText = new Label(this, "StartText");
            _startText.TextRender.Color = Color.White;
            _startText.Text = "Press Start to begin the game!";
            _startText.TabPosition = new Point(0,1);
            _startText.Body.Position = new Vector2(EntityGame.Viewport.Width/2f - _startText.Body.Bounds.X/2f,550);
            _startText.TextRender.Layer = 1f;
            ch.AddControl(_startText);

            AddEntity(new STDMenuStateManager(this));

            Services.Add(ch);
        }
    }
}
