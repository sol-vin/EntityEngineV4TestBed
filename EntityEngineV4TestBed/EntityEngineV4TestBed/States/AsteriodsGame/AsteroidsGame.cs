using System.Linq;
using EntityEngineV4.CollisionEngine;
using EntityEngineV4.Engine;
using EntityEngineV4.Engine.Services;
using EntityEngineV4.GUI;
using EntityEngineV4.PowerTools;
using EntityEngineV4TestBed.States.AsteriodsGame.Objects;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.AsteriodsGame
{
    public class AsteroidsGame : TestBedState
    {
        private PlayerShip _player;
        private Label _statusLabel;
        private bool _playerDied, _asteroidsDied;

        public AsteroidsGame()
            : base("AsteroidsGame")
        {
        }


        public override void Initialize()
        {
            base.Initialize();
            new AssetCollector(this);
            GetService<AssetCollector>().LoadXML(@"Content/AsteroidsGame/assets.xml");

            //Change bgcolor to black
            EntityGame.BackgroundColor = Color.Black;
            EntityGame.DebugInfo.Color = Color.White;
            _player = new PlayerShip(this, "PlayerShip");

            //SpawnAsteroids(5);

            //TEST ASTEROIDS
            var a = new Asteroid(this, "Asteroid1");
            a.Body.Position = new Vector2(-10, 100);
            a.Physics.AddForce(-a.Physics.Force);

            var b = new Asteroid(this, "Asteroid2");
            b.Body.Position = new Vector2(100, 100);
            b.Physics.AddForce(-b.Physics.Force);
            b.Physics.AddForce(30, 0);

            Page p = new Page(this, "Page");
            p.Show();

            _statusLabel = new Label(p, "StatusLabel", new Point(0, 0));
            _statusLabel.Color = Color.White;
            _statusLabel.Visible = false;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (!_playerDied && !_asteroidsDied)
            {
                _playerDied = this.Count(c => c.GetType() == typeof(PlayerShip)) == 0;
                _asteroidsDied = this.Count(c => c.GetType() == typeof(Asteroid)) == 0;
            }

            if (_playerDied && _asteroidsDied) //Playership took out the last asteroid by ramming it
            {
                _statusLabel.Visible = true;
                _statusLabel.Text = "Good job dipshit.";
                _statusLabel.Body.X = EntityGame.Viewport.Width / 2f - _statusLabel.Body.Width / 2f;
                _statusLabel.Body.Y = 100;

            }
            else if (_playerDied)
            {
                _statusLabel.Visible = true;
                _statusLabel.Text = "You lose, stop sucking jajajaja.";
                _statusLabel.Body.X = EntityGame.Viewport.Width / 2f - _statusLabel.Body.Width / 2f;
                _statusLabel.Body.Y = 100;

            }
            else if (_asteroidsDied)
            {
                _statusLabel.Visible = true;
                _statusLabel.Text = "You win, bitches ain't shit.";
                _statusLabel.Body.X = EntityGame.Viewport.Width / 2f - _statusLabel.Body.Width / 2f;
                _statusLabel.Body.Y = 100;
            }
        }

        public override void Destroy(IComponent i = null)
        {
            base.Destroy(i);
            EntityGame.BackgroundColor = Color.White;
            EntityGame.DebugInfo.Color = Color.Black;
        }

        public void SpawnAsteroids(int num)
        {
            for (int i = 0; i < num; i++)
            {
                var a = new Asteroid(this, "Asteroid");
                while (GetService<CollisionHandler>().ReturnManifolds(a.Collision).Count > 0)
                {
                    a.Body.X = RandomHelper.GetFloat() * EntityGame.Viewport.Right;
                    a.Body.Y = RandomHelper.GetFloat() * EntityGame.Viewport.Bottom;
                }
            }
        }
    }
}