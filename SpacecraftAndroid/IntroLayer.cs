using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private readonly CCSprite _fireButton;
        private bool _fireButtonHeld;

        private CCAcceleration _initialAccel;
        private CCAcceleration _currentAcceleration;

        private readonly Debugger _debugger;

        public IntroLayer() : base(CCColor4B.Gray)
        {
            //Init ship
            _shipSprite = new CCSprite("ship")
            {
                Scale = 2
            };
            AddChild(_shipSprite);
            _ship = new PlayerShip(ShipMaxVelocityX, ShipMaxVelocityY);

            //Init button
            _fireButton = new CCSprite("button")
            {
                Scale = 3
            };
            AddChild(_fireButton);

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
            _shipController = new ShipController(_shipSprite, _ship, bounds, _debugger,
                node => AddChild(node),
                node => RemoveChild(node));

            _fireButton.Position = new CCPoint(100, 100);

            //Debug labels
            _debugger.PositionDebugLabels(bounds);

            //Init accelerometer
            Window.Accelerometer.Enabled = true;
            var accelerometer = new CCEventListenerAccelerometer
            {
                OnAccelerate = OnAccelerate
            };
            AddEventListener(accelerometer);

            //Init touch listener
            var touchListener = new CCEventListenerTouchAllAtOnce
            {
                OnTouchesBegan = OnTouches,
                OnTouchesMoved = OnTouches,
                OnTouchesEnded = OnTouchesEnded
            };
            AddEventListener(touchListener, this);
        }

        private void GameLoop(float obj)
        {
            HandleAcceleration(_currentAcceleration);

            HandleFireButtonPress();

            _shipController.Update();
        }

        private void OnAccelerate(CCEventAccelerate accelerateEvent)
        {
            _currentAcceleration = accelerateEvent.Acceleration;
        }

        private void OnTouches(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Any())
            {
                if (_fireButton.BoundingBoxTransformedToWorld.ContainsPoint(touches[0].Location))
                {
                    _fireButtonHeld = true;
                    Debug.WriteLine("Firing");
                }
            }
        }

        private void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Any())
            {
                _fireButtonHeld = false;
            }
        }

        private void HandleFireButtonPress()
        {
            if (_fireButtonHeld)
            {
                _shipController.Fire();
            }
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

