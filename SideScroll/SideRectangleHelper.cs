using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic
{
    static class SideRectangleHelper
    {
        public static bool TouchTopOf(this Rectangle r1, Rectangle r2)
        {
            return (r1.Bottom >= r2.Top - 2
                && r1.Bottom <= r2.Top + (r2.Height / 2)
                && r1.Right >= r2.Left /*+ (r2.Width / 4f)*/
                && r1.Left <= r2.Right /*- (r2.Width / 4f)*/);
        }
        public static bool TouchBottomOf(this Rectangle r1, Rectangle r2)
        {
            return (r1.Top <= r2.Bottom
                && r1.Top >= r2.Top + (r2.Height / 2)
                && r1.Right >= r2.Left
                && r1.Left <= r2.Right);
        }

        public static bool TouchLeftOf(this Rectangle r1, Rectangle r2)
        {
            return (r1.Right <= r2.Right &&
                r1.Right >= r2.Left &&
                r1.Top <= r2.Bottom - (r2.Width / 2) &&
                r1.Bottom >= r2.Top + 1);
        }

        public static bool TouchRightOf(this Rectangle r1, Rectangle r2)
        {
            return (r1.Left >= r2.Left &&
                r1.Left <= r2.Right &&
                r1.Top <= r2.Bottom - (r2.Width / 2) &&
                r1.Bottom >= r2.Top + 1);
        }
    }
}
