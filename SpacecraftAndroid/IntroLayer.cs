using System;
using CocosSharp;
using SpacecraftAndroid.Controllers;
using SpacecraftAndroid.Models;

namespace SpacecraftAndroid
{
    public sealed class IntroLayer : CCLayerColor
    {
        private const int ShipMaxVelocityX = 20;
        private const int ShipMaxVelocityY = 15;

        private static int _accelMultiplierX = 37;
        private static int _accelMultiplierY = 32;
        private static int _orientationAccelModifier;

        private readonly CCSprite _shipSprite;
        private readonly PlayerShip _ship;
        private ShipController _shipController;

        private CCAcceleration _initialAccel;
        private CCAcceleration _currentAcceleration;

        private readonly Debugger _debugger;

        public IntroLayer() : base(CCColor4B.Gray)
        {
            //Init ship
            _shipSprite = new CCSprite("ship")
            {
                //Rotation = 90,
                Scale = 2
            };
            AddChild(_shipSprite);
            _ship = new PlayerShip(ShipMaxVelocityX, ShipMaxVelocityY);

            _debugger = new Debugger();
            _debugger.InitDebugLabels(this);

            Schedule(GameLoop);
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            //The acceleration is inverted if you're holding the device upside-down.
            _orientationAccelModifier = Application.CurrentOrientation == CCDisplayOrientation.LandscapeRight ? 1 : -1;

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;

            // position the ship on the center of the screen
            _shipSprite.Position = bounds.Center;
            _shipController = new ShipController(_shipSprite, _ship, bounds, _debugger);

            //Debug labels
            _debugger.PositionDebugLabels(bounds);

            Window.Accelerometer.Enabled = true;
            var accelerometer = new CCEventListenerAccelerometer
            {
                OnAccelerate = OnAccelerate
            };
            AddEventListener(accelerometer);
        }

        private void GameLoop(float obj)
        {
            HandleAcceleration(_currentAcceleration);

            _shipController.Update();
        }

        private void OnAccelerate(CCEventAccelerate accelerateEvent)
        {
            _currentAcceleration = accelerateEvent.Acceleration;
        }

        private void HandleAcceleration(CCAcceleration acceleration)
        {
            if (acceleration.TimeStamp <= 0) return;

            //Record initial acceleration as our calibration value
            if (_initialAccel == null)
            {
                _initialAccel = new CCAcceleration
                {
                    X = acceleration.X,
                    Y = acceleration.Y,
                    Z = acceleration.Z
                };
            }

            //Calculate the acceleration relative to the initial acceleration
            var offsetAccel = new CCAcceleration
            {
                X = acceleration.X - _initialAccel.X,
                Y = acceleration.Y - _initialAccel.Y,
                Z = acceleration.Z - _initialAccel.Z
            };

            //Move the ship
            var velocityX = _orientationAccelModifier * _accelMultiplierX * Convert.ToSingle(offsetAccel.Y);
            var velocityY = _orientationAccelModifier * _accelMultiplierY * Convert.ToSingle(offsetAccel.Z);

            _ship.ApplyVelocity(velocityX, velocityY);

            _debugger.DebugAcceleration(_initialAccel, acceleration, offsetAccel);
            _debugger.DebugVelocity(velocityX, velocityY);
        }
    }
}

