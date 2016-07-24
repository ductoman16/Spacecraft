namespace SpacecraftAndroid.Models
{
    /// <summary>
    ///     A projectile fired from a ship.
    /// </summary>
    public class Bullet
    {
        public double VelocityX { get; }
        public double VelocityY { get; }

        public Facing Facing { get; private set; }

        public Bullet(double velocityX, double velocityY)
        {
            VelocityX = velocityX;
            VelocityY = velocityY;

            if (VelocityX < 0) Facing = Facing.Left;
            if (VelocityX > 0) Facing = Facing.Right;
        }
    }
}