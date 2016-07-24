using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly IDictionary<Bullet, CCSprite> _bullets = new Dictionary<Bullet, CCSprite>();
        private readonly Action<CCNode> _addChild;
        private readonly Action<CCNode> _removeChild;

        public ShipController(CCSprite shipSprite, PlayerShip shipModel, CCRect visibleBoundsWorldspace, Debugger debugger, Action<CCNode> addChild, Action<CCNode> removeChild)
        {
            _visibleBoundsWorldspace = visibleBoundsWorldspace;
            _debugger = debugger;
            _shipSprite = shipSprite;
            _shipModel = shipModel;
            _addChild = addChild;
            _removeChild = removeChild;
        }

        public void Update()
        {
            //Update ship position
            _shipSprite.PositionX += Convert.ToSingle(_shipModel.VelocityX);
            _shipSprite.PositionY += Convert.ToSingle(_shipModel.VelocityY);

            _shipSprite.Rotation = _shipModel.Facing == Facing.Left ? 90 : -90;

            RestrainShipY();
            RestrainShipX();

            //Update bullet positions
            foreach (var pair in _bullets.ToList())
            {
                var sprite = pair.Value;
                var bullet = pair.Key;

                sprite.PositionX += Convert.ToSingle(bullet.VelocityX);
                sprite.PositionY += Convert.ToSingle(bullet.VelocityY);

                RestrainBullet(bullet, sprite);
            }

            _debugger.DebugVelocity(_shipModel.VelocityX, _shipModel.VelocityY);
        }

        private void RestrainBullet(Bullet bullet, CCSprite sprite)
        {
            if (!_visibleBoundsWorldspace.ContainsPoint(sprite.Position))
            {
                _bullets.Remove(bullet);
                _removeChild(sprite);
            }
        }

        public void Fire()
        {
            var bullet = _shipModel.Fire();

            var sprite = new CCSprite("button")
            {
                Position = _shipSprite.Position,
                Scale = .5f
            };
            _bullets.Add(bullet, sprite);
            _addChild(sprite);
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