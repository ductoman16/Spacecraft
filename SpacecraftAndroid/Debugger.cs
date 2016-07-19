using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CocosSharp;

namespace SpacecraftAndroid
{
    public class Debugger
    {
        private const string DebugFont = "Arial";
        private CCLabel _velocityDebug;
        private CCLabel _actualAccelDebug;
        private CCLabel _initialAccelDebug;
        private CCLabel _offsetAccelDebug;

        public void InitDebugLabels(CCLayer layer)
        {
            _velocityDebug = new CCLabel("", DebugFont, 100f);
            layer.AddChild(_velocityDebug);

            _actualAccelDebug = new CCLabel("", DebugFont, 100f);
            _actualAccelDebug.UpdateDisplayedColor(CCColor3B.Green);
            layer.AddChild(_actualAccelDebug);

            _initialAccelDebug = new CCLabel("", DebugFont, 100f);
            _initialAccelDebug.UpdateDisplayedColor(CCColor3B.Blue);
            layer.AddChild(_initialAccelDebug);

            _offsetAccelDebug = new CCLabel("", DebugFont, 100f);
            _offsetAccelDebug.UpdateDisplayedColor(CCColor3B.Red);
            layer.AddChild(_offsetAccelDebug);
        }

        public void PositionDebugLabels(CCRect worldBounds)
        {
            _velocityDebug.AnchorPoint = new CCPoint(1, 0);
            _velocityDebug.Position = new CCPoint(worldBounds.MaxX - 10, 10);

            _actualAccelDebug.AnchorPoint = new CCPoint(0, 0);
            _actualAccelDebug.Position = new CCPoint(10, 10);

            _initialAccelDebug.AnchorPoint = new CCPoint(0, 0);
            _initialAccelDebug.Position = new CCPoint(10, 110);

            _offsetAccelDebug.AnchorPoint = new CCPoint(0, 0);
            _offsetAccelDebug.Position = new CCPoint(10, 60);
        }

        public void DebugAcceleration(CCAcceleration initialAccel, CCAcceleration acceleration, CCAcceleration offsetAccel)
        {
            var doDebug = DateTime.Now.Ticks % 5 == 0;
            if (doDebug)
            {
                DebugAcceleration(initialAccel, "Initial Accel: ", _initialAccelDebug);
                DebugAcceleration(acceleration, "Actual Accel: ", _actualAccelDebug);
                DebugAcceleration(offsetAccel, "Offset Accel: ", _offsetAccelDebug);
            }
        }

        public void DebugVelocity(float velocityX, float velocityY)
        {
            var text = $"Velocity: X:{velocityX.ToString("0.00000")} Y:{velocityY.ToString("0.00000")}";
            _velocityDebug.Text = text;
            //System.Diagnostics.Debug.WriteLine(text);
        }

        private static void DebugAcceleration(CCAcceleration acceleration, string title, CCLabel debugLabel)
        {
            if (acceleration == null)
            {
                System.Diagnostics.Debug.WriteLine(title + "null");
                return;
            }


            var text = title +
                          $"X:{(acceleration.X >= 0 ? " " : string.Empty)}{acceleration.X.ToString("0.00000")} " +
                          $"Y:{(acceleration.Y >= 0 ? " " : string.Empty)}{acceleration.Y.ToString("0.00000")} " +
                          $"Z:{(acceleration.Z >= 0 ? " " : string.Empty)}{acceleration.Z.ToString("0.00000")} ";

            if (debugLabel != null) debugLabel.Text = text;
            //System.Diagnostics.Debug.WriteLine(text);
        }
    }
}