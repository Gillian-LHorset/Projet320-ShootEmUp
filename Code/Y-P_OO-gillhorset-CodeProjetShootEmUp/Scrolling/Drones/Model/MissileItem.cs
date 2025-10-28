using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class MissileItem
    {
        // position du missile item
        private int _x;
        private int _y;

        // taille du missile item
        private readonly int _WIDTH = 30;
        private readonly int _HEIGHT = 30;

        // rectangle pour detecter les collision avec le joueur
        public Rectangle MissileItemRectCollision;
        public MissileItem(int X, int Y)
        {
            _x = X;
            _y = Y;

            MissileItemRectCollision = new Rectangle(_x, _y, _WIDTH, _HEIGHT);
        }
    }
}
