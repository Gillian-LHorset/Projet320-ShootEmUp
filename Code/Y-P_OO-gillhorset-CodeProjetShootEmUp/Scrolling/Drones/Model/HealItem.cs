using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class HealItem
    {

        public int _x;
        public int _y;

        public readonly int WIDTH = 30;
        public readonly int HEIGHT = 30;

        public Rectangle healItemRectCollision;
        public HealItem(int X, int Y) { 
            _x = X; 
            _y = Y;

            healItemRectCollision = new Rectangle(_x, _y, WIDTH, HEIGHT);
        }
    }
}
