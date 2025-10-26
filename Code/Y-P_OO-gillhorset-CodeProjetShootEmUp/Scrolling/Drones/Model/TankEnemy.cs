using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class TankEnemy : Enemy
    {
        // vie maximale de l'ennemie
        public readonly int HEALPOINTMAX = 1;

        // variable stockant les informations du joueur
        private Ship _player;

        // définie la vitesse de déplacement de l'enemie
        private int _moveSpeed = 2;

        // défini la direction verticale que l'ennemie prend
        private int _moveDirection;

        public TankEnemy(Ship player, int x, int y) : base(x, y)
        {
            // défini les points de vie de l'ennemie à sa création
            healPoint = HEALPOINTMAX;

            // défini la taille de l'ennemie
            WIDTH = 45;
            HEIGHT = 45;

            // défini le rectangle de collision à la position et aux proportions de l'ennemie
            EnemyRectCollision = new Rectangle(x, y, WIDTH, HEIGHT);

            // importe les informations du joueur
            _player = player;

        }

        /// <summary>
        /// l'ennemie ne tirant aucun projectile, la méthode de shoot est vide
        /// </summary>
        public override void EnemyShoot() {}

        public override void EnemyMove() {

            if (X + Ship.WIDTH < _player.X)
            // si la position horizontale plus la largeur de ship est inferieur à la position du joueur
            {
                // fait avancer l'ennemie vers la droite
                X += _moveSpeed;
            } else if (X - Ship.WIDTH > _player.X)
            // si la position horizontale moins la largeur de ship est supperieur à la position du joueur
            {
                // fait bouger l'ennemie vers la gauche
                X -= _moveSpeed;
            }

            if (Y + Ship.HEIGHT < _player.Y)
            // si la position verticale de l'ennemie plus la hauteur du vaisseau est inferieur à la position verticale du joueur
            {
                // fait déscendre l'ennemie
                Y += _moveSpeed;
            }
            else if (Y - Ship.HEIGHT > _player.Y)
            // si la position verticale de l'ennemie moins la hauteur du vaisseau est supperieur à la position verticale du joueur
            {
                // fait monter l'ennemie
                Y -= _moveSpeed;
            }



            EnemyRectCollision.X = X;
            EnemyRectCollision.Y = Y;
        }



    }
}
