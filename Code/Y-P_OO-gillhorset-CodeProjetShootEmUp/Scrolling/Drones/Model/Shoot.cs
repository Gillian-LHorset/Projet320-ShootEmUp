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
        // position du tir
        private int _x;
        private int _y;

        // défini si le tir viens du joueur
        public bool IsAPlayerShoot;

        // rectangle ayant pour but de détecter les collision
        public Rectangle ShootRectCollision;

        // taille du tir
        private readonly int _WIDTH = 64;
        private readonly int _HEIGHT = 8;

        public Shoot (int X, int Y, bool isAPlayerShoot)
        {
            _x = X;
            _y = Y;
            IsAPlayerShoot = isAPlayerShoot;

            ShootRectCollision = new Rectangle(_x, _y, _WIDTH, _HEIGHT);
        }

        public int shootX { get { return _x; } set { _x = value; } }
        public int shootY { get { return _y; } set { _y = value; } }


        public virtual void ShootMove()
        {
            if (IsAPlayerShoot)
            // si le tir est tiré par le joueur
            {
                // le tir va se déplacer de gauche a droite
                _x += 30;
            } else
            // si le tir est tiré par un ennemy
            {
                // il se déplacera de droite à gauche
                _x -= 30;
            }
            ShootRectCollision.X = _x;
                
            
        }

    }
}
