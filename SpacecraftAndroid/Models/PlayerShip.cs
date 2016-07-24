using System;
using CocosSharp;

namespace SpacecraftAndroid.Models
{
    /// <summary>
    ///     The player's ship.
    /// </summary>
    public sealed class PlayerShip
    {
        private readonly double _maxVelocityX;
        private readonly double _maxVelocityY;

        public double VelocityX { get; private set; }
        public double VelocityY { get; private set; }

        public Facing Facing { get; private set; }

        public PlayerShip(double maxVelocityX, double maxVelocityY)
        {
            _maxVelocityX = maxVelocityX;
            _maxVelocityY = maxVelocityY;
            Facing = Facing.Right;
        }

        public void ApplyVelocity(double x, double y)
        {
            if (x > 0) VelocityX = Math.Min(x, _maxVelocityX);
            if (x < 0) VelocityX = Math.Max(x, -1 * _maxVelocityX);

            if (y > 0) VelocityY = Math.Min(y, _maxVelocityY);
            if (y < 0) VelocityY = Math.Max(y, -1 * _maxVelocityY);
        }

        public void Turn()
        {
            Facing = Facing == Facing.Left ? Facing.Right : Facing.Left;
        }

    }
}