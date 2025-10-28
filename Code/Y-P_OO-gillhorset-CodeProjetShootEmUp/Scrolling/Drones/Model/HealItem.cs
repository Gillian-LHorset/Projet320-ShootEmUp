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
        // position de l'item
        private int _x;
        private int _y;

        // taille de l'item
        public readonly int WIDTH = 30;
        public readonly int HEIGHT = 30;

        // rectangle ayant pour but de détecter les collisions
        public Rectangle HealItemRectCollision;

        public HealItem(int X, int Y) { 
            _x = X; 
            _y = Y;

            HealItemRectCollision = new Rectangle(_x, _y, WIDTH, HEIGHT);
        }
    }
}
