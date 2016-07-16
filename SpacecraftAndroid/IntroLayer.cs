using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;
using System.Linq;

namespace SpacecraftAndroid
{
    public sealed class IntroLayer : CCLayerColor
    {
        private static int _accelMultiplier = 17;

        private readonly CCSprite _ship;

        private CCAcceleration _initialAccel;

        public IntroLayer() : base(CCColor4B.Gray)
        {

            //Init ship
            _ship = new CCSprite("ship")
            {
                Rotation = 90,
                Scale = 2
            };
            AddChild(_ship);

            Schedule(GameLoop);
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            //The ship's acceleration is inverted if you're holding the device upside-down.
            _accelMultiplier *= Application.CurrentOrientation == CCDisplayOrientation.LandscapeRight ? -1 : 1; 

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;

            // position the label on the center of the screen
            _ship.Position = bounds.Center;

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
            var acceleration = accelerateEvent.Acceleration;
            System.Diagnostics.Debug.WriteLine(
                $"X: {(acceleration.X >= 0 ? " " : string.Empty)}{acceleration.X.ToString("0.00000")} " +
                $"Y: {(acceleration.Y >= 0 ? " " : string.Empty)}{acceleration.Y.ToString("0.00000")} " +
                $"Z: {(acceleration.Z >= 0 ? " " : string.Empty)}{acceleration.Z.ToString("0.00000")} ");

            if (_initialAccel == null) _initialAccel = acceleration;

            _ship.PositionX -= _accelMultiplier * Convert.ToSingle(acceleration.Y);
            _ship.PositionY += _accelMultiplier * Convert.ToSingle(acceleration.X);

        }

    }
}

