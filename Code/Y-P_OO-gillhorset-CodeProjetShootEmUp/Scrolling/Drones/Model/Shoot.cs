using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class Shoot
    {
        public int _x;
        public int _y;

        private bool _isAPlayerShoot;
        
        public readonly int WIDTH = 64;
        private readonly int HEIGHT = 8;

        public Shoot (int X, int Y, bool isAPlayerShoot)
        {
            _x = X;
            _y = Y;
            _isAPlayerShoot = isAPlayerShoot;
        }

        public int shootX { get { return _x; } set { _x = value; } }
        public int shootY { get { return _y; } set { _y = value; } }

        public static void ShootMove(List<Shoot> shoots)
        {
            if (shoots.Count > 0)
            {
                foreach (var shoot in shoots.ToList())
                {
                    if (shoot._isAPlayerShoot)
                    // si le tir est tiré par le joueur
                    {
                        // le tir va se déplacer de gauche a droite
                        shoot._x += 30;
                    } else
                    // si le tir est tiré par un ennemy
                    {
                        // il se déplacera de droite à gauche
                        shoot._x -= 30;
                    }
                }
            }
        }

    }
}
