using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class Missile : Shoot
    {
        // écrase la taille définie par la classe partent
        private readonly int _WIDTH = 44;
        private readonly int _HEIGHT = 26;
        public Missile(int X, int Y, bool isAPlayerShoot) : base(X, Y, isAPlayerShoot)
        {
            shootX = X;
            shootY = Y;
            IsAPlayerShoot = isAPlayerShoot;

            ShootRectCollision = new Rectangle(shootX, shootY, _WIDTH, _HEIGHT);
        }

        /// <summary>
        /// fait déplacer le missile horizontalement sur AirSpace
        /// </summary>
        public override void ShootMove()
        {
            if (IsAPlayerShoot)
            // si le tir est tiré par le joueur
            {
                // le tir va se déplacer de gauche a droite
                shootX += 50;
            }
            // déplace le rectangle de collision en à l'endroit ou est le tir
            ShootRectCollision.X = shootX;
        }
    }
}
