using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace WizardRogueLike
{
    partial class Game1
    {
        public bool CircleOnBox(Vector2 circlePos, float circleR, Vector2 boxPos, Vector2 boxSize)
        {
            Vector2 circleDistance;
            
            circleDistance.X = Math.Abs(circlePos.X - boxPos.X);
            circleDistance.Y = Math.Abs(circlePos.Y - boxPos.Y);

            if (circleDistance.X > (boxSize.X / 2 + circleR)) { return false; }
            if (circleDistance.Y > (boxSize.Y / 2 + circleR)) { return false; }

            if (circleDistance.X <= (boxSize.X / 2)) { return true; }
            if (circleDistance.Y <= (boxSize.Y / 2)) { return true; }

            float cornerDistance_sq = (float)(Math.Pow((circleDistance.X - boxSize.X / 2), 2) + Math.Pow((circleDistance.Y - boxSize.Y / 2), 2));

            return (cornerDistance_sq <= (Math.Pow(circleR,2)));
        }

        public bool CircleCollision(Vector2 circle1, float circleR1, Vector2 circle2, float circleR2)
        {
            float dX = circle2.X - circle1.X;
            float dY = circle2.Y - circle1.Y;
            float Radius = circleR1 + circleR2;
            return (Math.Sqrt((dX * dX) + (dY * dY)) <= Math.Sqrt(Radius * Radius));
        }
    }
}
