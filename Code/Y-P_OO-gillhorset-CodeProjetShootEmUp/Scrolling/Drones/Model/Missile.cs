using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class Missile : Shoot
    {

        public new readonly int WIDTH = 44;
        public readonly int HEIGHT = 26;
        public Missile(int X, int Y, bool isAPlayerShoot) : base(X, Y, isAPlayerShoot)
        {
            _x = X;
            _y = Y;
            IsAPlayerShoot = isAPlayerShoot;

            ShootRectCollision = new Rectangle(_x, _y, WIDTH, HEIGHT);
        }
        public override void ShootMove()
        {
            if (IsAPlayerShoot)
            // si le tir est tiré par le joueur
            {
                // le tir va se déplacer de gauche a droite
                _x += 50;
            }
            ShootRectCollision.X = _x;
        }
    }
}
