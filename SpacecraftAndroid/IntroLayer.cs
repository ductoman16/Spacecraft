using System;
using CocosSharp;

namespace SpacecraftAndroid
{
    public sealed class IntroLayer : CCLayerColor
    {
        private static int _accelMultiplierY = 32;
        private static int _accelMultiplierX = 17;
        private static int _orientationAccelModifier;

        private readonly CCSprite _ship;

        private CCAcceleration _initialAccel;
        private CCAcceleration _currentAcceleration;

        private readonly Debugger _debugger;

        public IntroLayer() : base(CCColor4B.Gray)
        {
            //Init ship
            _ship = new CCSprite("ship")
            {
                Rotation = 90,
                Scale = 2
            };
            AddChild(_ship);

            _debugger = new Debugger();
#if(DEBUG)
            _debugger.InitDebugLabels(this);
#endif

            Schedule(GameLoop);
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            //The ship's acceleration is inverted if you're holding the device upside-down.
            _orientationAccelModifier = Application.CurrentOrientation == CCDisplayOrientation.LandscapeRight ? -1 : 1;

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;

            // position the label on the center of the screen
            _ship.Position = bounds.Center;

            //Debug labels
#if(DEBUG)
            _debugger.PositionDebugLabels(bounds);
#endif

            Window.Accelerometer.Enabled = true;
            var accelerometer = new CCEventListenerAccelerometer
            {
                OnAccelerate = OnAccelerate
            };
            AddEventListener(accelerometer);
        }

        private void GameLoop(float obj)
        {
            RestrainShipY();
            RestrainShipX();

            HandleAcceleration(_currentAcceleration);
        }

        private void RestrainShipX()
        {
            if (_ship.PositionX > VisibleBoundsWorldspace.MaxX) _ship.PositionX = VisibleBoundsWorldspace.MaxX;
            if (_ship.PositionX < VisibleBoundsWorldspace.MinX) _ship.PositionX = VisibleBoundsWorldspace.MinX;
        }

        private void RestrainShipY()
        {
            if (_ship.PositionY > VisibleBoundsWorldspace.MaxY) _ship.PositionY = VisibleBoundsWorldspace.MaxY;
            if (_ship.PositionY < VisibleBoundsWorldspace.MinY) _ship.PositionY = VisibleBoundsWorldspace.MinY;
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
            _ship.PositionX -= velocityX;
            var velocityY = _orientationAccelModifier * _accelMultiplierY * Convert.ToSingle(-offsetAccel.Z);
            _ship.PositionY += velocityY;

#if(DEBUG)
            _debugger.DebugAcceleration(_initialAccel, acceleration, offsetAccel);
            _debugger.DebugVelocity(velocityX, velocityY);
#endif
        }
    }
}

