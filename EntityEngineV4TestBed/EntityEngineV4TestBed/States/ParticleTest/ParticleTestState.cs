using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.ParticleTest
{
    public class ParticleTestState : TestBedState
    {
        private Label _screeninfo;

        private Label _strengthText, _strengthValue;
        private LinkLabel _strengthUp, _strengthDown;

        private Label _gravityText, _gravityXValue, _gravityYValue;
        private LinkLabel _gravityXDown, _gravityYDown, _gravityXUp, _gravityYUp;

        private ParticleTestManager _ptm;

        public ParticleTestState(EntityGame eg) : base(eg, "ParticleTestState")
        {
            Services.Add(new MouseHandler(this));
            Services.Add(new InputHandler(this));
            
            ControlHandler ch = new ControlHandler(this);

            _ptm = new ParticleTestManager(this, ch);
            AddEntity(_ptm);

            _screeninfo = new Label(this, "ScreenInfo");
            _screeninfo.Body.Position = Vector2.One*20;
            ch.AddControl(_screeninfo);

            _strengthText = new Label(this, "StrengthText");
            _strengthText.Text = "Strength:";
            _strengthText.Body.Position = new Vector2(20, 50);
            _strengthText.TabPosition= new Point(0,1);
            ch.AddControl(_strengthText);

            _strengthDown = new LinkLabel(this, "StrengthDown");
            _strengthDown.Text = "<-";
            _strengthDown.TabPosition = new Point(1,1);
            _strengthDown.Body.Position = new Vector2(_strengthText.Body.BoundingRect.Right + 5, 50);
            _strengthDown.Selected += control => _ptm.Emitter.Strength -= .1f;
            ch.AddControl(_strengthDown);

            _strengthValue = new Label(this, "StrengthValue");
            _strengthValue.Text = _ptm.Emitter.Strength.ToString();
            _strengthValue.Body.Position = new Vector2(_strengthDown.Body.BoundingRect.Right + 5, 50);
            _strengthValue.TabPosition = new Point(2,1);
            ch.AddControl(_strengthValue);

            _strengthUp = new LinkLabel(this, "StrengthUp");
            _strengthUp.Text = "->";
            _strengthUp.TabPosition = new Point(3, 1);
            _strengthUp.Body.Position = new Vector2(_strengthValue.Body.BoundingRect.Right + 5, 50);
            _strengthUp.Selected += control => _ptm.Emitter.Strength += .1f;
            ch.AddControl(_strengthUp);

            _gravityText = new Label(this, "GravityText");
            _gravityText.Text = "Gravity:";
            _gravityText.Body.Position = new Vector2(20, 80);
            _gravityText.TabPosition = new Point(0,2);
            ch.AddControl(_gravityText);

            _gravityXDown = new LinkLabel(this, "GravityXDown");
            _gravityXDown.Text = "<-";
            _gravityXDown.TabPosition = new Point(1,2);
            _gravityXDown.Body.Position = new Vector2(_gravityText.Body.BoundingRect.Right + 5, 80);
            _gravityXDown.Selected += control => _ptm.Emitter.Acceleration.X-=.1f;
            ch.AddControl(_gravityXDown);

            _gravityXValue = new Label(this, "GravityXValue");
            _gravityXValue.Text = "X:"+_ptm.Emitter.Acceleration.X.ToString();
            _gravityXValue.TabPosition = new Point(2,2);
            _gravityXValue.Body.Position = new Vector2(_gravityXDown.Body.BoundingRect.Right + 5, 80);
            ch.AddControl(_gravityXValue);

            _gravityXUp = new LinkLabel(this, "GravityXUp");
            _gravityXUp.Text = "->";
            _gravityXUp.TabPosition = new Point(3, 2);
            _gravityXUp.Body.Position = new Vector2(_gravityXValue.Body.BoundingRect.Right + 5, 80);
            _gravityXUp.Selected += control => _ptm.Emitter.Acceleration.X+=.1f;
            ch.AddControl(_gravityXUp);

            _gravityYDown = new LinkLabel(this, "GravityYDown");
            _gravityYDown.Text = "<-";
            _gravityYDown.TabPosition = new Point(1, 3);
            _gravityYDown.Body.Position = new Vector2(_gravityText.Body.BoundingRect.Right + 5, 110);
            _gravityYDown.Selected += control => _ptm.Emitter.Acceleration.Y-=.1f;
            ch.AddControl(_gravityYDown);

            _gravityYValue = new Label(this, "GravityYValue");
            _gravityYValue.Text = "Y:" + _ptm.Emitter.Acceleration.Y.ToString();
            _gravityYValue.TabPosition = new Point(2, 3);
            _gravityYValue.Body.Position = new Vector2(_gravityYDown.Body.BoundingRect.Right + 5, 110);
            ch.AddControl(_gravityYValue);

            _gravityYUp = new LinkLabel(this, "GravityYUp");
            _gravityYUp.Text = "->";
            _gravityYUp.TabPosition = new Point(3, 3);
            _gravityYUp.Body.Position = new Vector2(_gravityYValue.Body.BoundingRect.Right + 5, 110);
            _gravityYUp.Selected += control => _ptm.Emitter.Acceleration.Y+=.1f;
            ch.AddControl(_gravityYUp);

            Services.Add(ch);
            
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            _screeninfo.Text = "Active: " + this.Count() + "\n";
            
            _strengthValue.Text = Math.Round(_ptm.Emitter.Strength, 1).ToString();
            _strengthUp.Body.Position.X = _strengthValue.Body.BoundingRect.Right + 5;

            _gravityYValue.Text = "Y:" + Math.Round(_ptm.Emitter.Acceleration.Y,1).ToString();
            _gravityYUp.Body.Position = new Vector2(_gravityYValue.Body.BoundingRect.Right + 5, 110);


            _gravityXValue.Text = "X:" + Math.Round(_ptm.Emitter.Acceleration.X,1).ToString();
            _gravityXUp.Body.Position = new Vector2(_gravityXValue.Body.BoundingRect.Right + 5, 80);
        }
    }
}
