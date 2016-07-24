using System;
using CocosSharp;
using SpacecraftAndroid.Models;

namespace SpacecraftAndroid.Controllers
{
    public class ShipController
    {
        private readonly CCRect _visibleBoundsWorldspace;
        private readonly Debugger _debugger;
        private readonly CCSprite _shipSprite;
        private readonly PlayerShip _shipModel;

        public ShipController(CCSprite shipSprite, PlayerShip shipModel, CCRect visibleBoundsWorldspace, Debugger debugger)
        {
            _visibleBoundsWorldspace = visibleBoundsWorldspace;
            _debugger = debugger;
            _shipSprite = shipSprite;
            _shipModel = shipModel;
        }

        public void Update()
        {
            _shipSprite.PositionX += Convert.ToSingle(_shipModel.VelocityX);
            _shipSprite.PositionY += Convert.ToSingle(_shipModel.VelocityY);

            RestrainShipY();
            RestrainShipX();

            _debugger.DebugVelocity(_shipModel.VelocityX, _shipModel.VelocityY);
        }

        private void RestrainShipX()
        {
            if (_shipSprite.PositionX > _visibleBoundsWorldspace.MaxX) _shipSprite.PositionX = _visibleBoundsWorldspace.MaxX;
            if (_shipSprite.PositionX < _visibleBoundsWorldspace.MinX) _shipSprite.PositionX = _visibleBoundsWorldspace.MinX;
        }

        private void RestrainShipY()
        {
            if (_shipSprite.PositionY > _visibleBoundsWorldspace.MaxY) _shipSprite.PositionY = _visibleBoundsWorldspace.MaxY;
            if (_shipSprite.PositionY < _visibleBoundsWorldspace.MinY) _shipSprite.PositionY = _visibleBoundsWorldspace.MinY;
        }

    }
}