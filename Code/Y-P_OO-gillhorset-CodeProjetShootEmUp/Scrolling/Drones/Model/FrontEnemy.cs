using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class FrontEnemy : Enemy
    {
        // défini le maximum de point de vie qu'un ennemie peut avoir
        public readonly int HEALPOINTMAX = 4;

        // variable qui servent à définire si l'ennemie avance ou recule
        private bool _isEnemyAdvencing = false;
        private bool _isEnemyRetreating = false;

        // sert à définire si l'ennemie monte ou déscend
        private bool _isEnemyGoingDown = true;

        // définie la vitesse de déplacement de l'enemie
        private int _advenceMoveSpeed = 5;
        private int _retreatMoveSpeed = 10;
        private int _yMoveSpeed = 1;

        public FrontEnemy(int x, int y) : base(x, y)
        {
            // point de vie de l'ennemie à sa création
            healPoint = HEALPOINTMAX;

            // défini la taille de l'ennemie
            WIDTH = 60;
            HEIGHT = 60;

            // défini le cooldown de tir en modifiant la variable qui est présente dans la classe parent
            _enemyShootCooldown = TimeSpan.FromSeconds(1.3);

            // défini le rectangle de collision à la position et aux proportions de l'ennemie
            EnemyRectCollision = new Rectangle(x, y, WIDTH, HEIGHT);
        }

        /// <summary>
        /// Permet à l'ennemie de se déplacer sur la carte
        /// </summary>
        public override void EnemyMove()
        {
            // éviter que l'ennemie sorte verticalement de l'écran
            if (Y < 10)
            // si la position de l'ennemie est trop haut
            {
                // fait redéscendre l'ennemie
                Y += 10;

                // lui donne l'ordre de commencer à déscendre
                _isEnemyGoingDown = true;
            }
            if (Y + HEIGHT + _yMoveSpeed > AirSpace.HEIGHT - Ship.ShipGround[X / 10])
            // si la position de l'ennemie est au niveau du sol
            {
                // remonte l'ennemie
                Y -= 10;

                // lui donne l'ordre de commencer à monter
                _isEnemyGoingDown = false;
            }

            // évite que l'ennemie sorte de l'écran horizontalement
            if (X > 200)
            // si la position de l'ennemie est à droite de l'écran
            {
                // il a pour ordre d'avancer
                _isEnemyAdvencing = true;
            } else
            {
                // il n'a plus l'ordre d'avancer
                _isEnemyAdvencing = false;

                // il a l'ordre de reculé
                _isEnemyRetreating = true;
            }
            

            if (X + WIDTH + 30 > AirSpace.WIDTH)
            // si la position de l'ennemie est à droite de l'écran
            {
                // il à l'ordre d'avancer
                _isEnemyAdvencing = true;

                // il n'a pas l'ordre de reculer
                _isEnemyRetreating = false;
            }

            if (_isEnemyAdvencing && !_isEnemyRetreating)
            // si l'ennemie a l'ordre d'avancer
            {
                // on décrémente sa position vers la gauche
                X -= _advenceMoveSpeed;
            }
            else if (_isEnemyRetreating)
            // si l'ennemie a l'ordre de reculer
            {
                // on incrémente sa position vers la droite
                X += _retreatMoveSpeed;
            }

            if (_isEnemyGoingDown)
            // si l'ennemie a pour ordre de déscendre
            {
                // on incrémente sa position vers le bas
                Y += _yMoveSpeed;
            }
            else {
                // on incrémente sa position vers le haut
                Y -= _yMoveSpeed;
            }

            // déplace le rectange qui sert a faire les collision avec le modèle de l'ennemie
            EnemyRectCollision.X = X;
            EnemyRectCollision.Y = Y;
        }

    }
}
