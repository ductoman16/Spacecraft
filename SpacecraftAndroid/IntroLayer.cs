using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;
using System.Linq;

namespace SpacecraftAndroid
{
    public class IntroLayer : CCLayerColor
    {
        private const int ShipSpeed = 2;

        private readonly CCSprite _ship;
        private readonly CCSprite _leftButton;
        private readonly CCSprite _rightButton;

        private bool _leftHeld;
        private bool _rightHeld;

        public IntroLayer() : base(CCColor4B.Gray)
        {
            //Init ship
            _ship = new CCSprite("ship")
            {
                PositionX = 100,
                PositionY = 100
            };
            AddChild(_ship);

            //Init Buttons
            _leftButton = new CCSprite("button")
            {
                PositionX = 100,
                PositionY = 100
            };
            AddChild(_leftButton);

            _rightButton = new CCSprite("button")
            {
                PositionY = 100
            };
            AddChild(_rightButton);

            Schedule(GameLoop);
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;

            // position the label on the center of the screen
            _ship.Position = bounds.Center;

            _rightButton.PositionX = VisibleBoundsWorldspace.MaxX - 100;

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce
            {
                OnTouchesBegan = OnTouchesMoved,
                OnTouchesEnded = OnTouchesEnded
            };
            AddEventListener(touchListener, this);
        }

        private void GameLoop(float obj)
        {
            if (_leftHeld)
            {
                _ship.PositionX -= ShipSpeed;
            }
            if (_rightHeld)
            {
                _ship.PositionX += ShipSpeed;
            }
        }

        private void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Any())
            {
                if (_leftButton.BoundingBoxTransformedToWorld.ContainsPoint(touches[0].Location))
                {
                    _leftHeld = true;
                }
                if (_rightButton.BoundingBoxTransformedToWorld.ContainsPoint(touches[0].Location))
                {
                    _rightHeld = true;
                }
            }
        }

        private void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Any())
            {
                _leftHeld = false;
                _rightHeld = false;
            }
        }
    }
}

