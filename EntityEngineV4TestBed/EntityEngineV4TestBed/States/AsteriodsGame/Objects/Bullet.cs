﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class Bullet : BaseEntity
    {
        public const float SPEED = 5f;
        public ImageRender Render;
        public Timer LifeTimer;

        public Bullet(IComponent parent, string name) : base(parent, name)
        {
            Body = new Body(this, "Body");

            Physics = new Physics(this, "Physics");
            Physics.Link(Physics.DEPENDENCY_BODY, Body);

            Render = new ImageRender(this, "Render");
            Render.LoadTexture("AsteroidsGame/circle");
            Render.Layer = .1f;
            Render.Scale = new Vector2(.03f);
            Render.Color = Color.White;
            Render.Origin = new Vector2(Render.Texture.Width/2f, Render.Texture.Height/2f);
            Render.Link(ImageRender.DEPENDENCY_BODY, Body);
            Body.Width = Render.DrawRect.Width;
            Body.Height = Render.DrawRect.Height;

            LifeTimer = new Timer(this, "LifeTimer");
            LifeTimer.Milliseconds = 2000;
            LifeTimer.LastEvent += () => Destroy();
        }

        public override void Initialize()
        {
            base.Initialize();
            LifeTimer.Start();
            Physics.Thrust(SPEED);
            
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }
}