using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class BasicEnemy : Enemy
    {
        // définie la vitesse de déplacement de l'enemie
        private int _moveSpeed = 5;

        // définie la vie maximum de l'ennemie
        readonly int HEALPOINTMAX = 3;

        // variable utilisé pour définir à quel moment l'ennemie change de direction
        int moveTiming;

        // défini la direction verticale que l'ennemie prend
        int moveDirection;

        // défini une position aléatoire pour éviter que tous les ennemies soit visuellement endroit horizontalement
        int rndPositionX = GlobalHelpers.alea.Next(1000, (AirSpace.WIDTH - 100));

        public BasicEnemy(int x, int y) : base(x, y)
        {
            // défini les points de vie de l'ennemie à sa création
            healPoint = HEALPOINTMAX;

            // défini la taille de l'ennemie
            WIDTH = 39;
            HEIGHT = 54;

            // défini le cooldown de tir en modifiant la variable qui est présente dans la classe parent
            _enemyShootCooldown = TimeSpan.FromSeconds(1.5);

            // défini le rectangle de collision à la position et aux proportions de l'ennemie
            enemyRectCollision = new Rectangle(x, y, WIDTH, HEIGHT);
        }

        /// <summary>
        /// Permet à l'ennemie de se déplacer sur la carte
        /// </summary>
        public override void EnemyMove()
        {
            // une position aléatoire est donné pour éviter que tous les ennemies soit visuellement endroit horizontalement
            if (X < rndPositionX)
            // si l'ennemie est trop à gauche
            {
                // le move speed de l'ennemie le ramenera à droite
                X += _moveSpeed;
            }

            if (X+WIDTH > AirSpace.WIDTH)
            // si l'ennemie est hors de l'écran à droite
            {
                // son move speed le ramenera à gauche
                X -= _moveSpeed;
            }

            if (moveTiming % 50 == 0)
            // crée un situation ou l'enemie peut bouger toute les 50 itération de la méthode
            {
                if (GlobalHelpers.alea.Next(50) < 25)
                    // crée une situation ou l'enemie à une chance sur deux de monter
                {
                    if (Y - _moveSpeed > 20)
                    // si la position de l'ennemie ne va pas être en dehors de l'écran
                    {
                        // fait monter l'ennemie
                        moveDirection = -5;
                    }
                }
                else if (Y + HEIGHT + _moveSpeed < AirSpace.HEIGHT - Ship.ShipGround[X / 10])
                // si la poisition de l'ennemie n'est pas dans le sol après execution
                {
                    // il peut bouger
                    moveDirection = 5;
                } else
                {
                    // il ne peut pas bouger
                    moveDirection = 0;
                }
            }                      

            if (Y + HEIGHT +10 > AirSpace.HEIGHT - Ship.ShipGround[X / 10])
            // si l'ennemie est dans le sol
            {
                // l'ennemie remonte
                Y -= _moveSpeed;
            }
            if (Y < 0)
            // si l'ennemie est hors de l'écran car il est trop haut
            {
                // il redescend
                Y += _moveSpeed;
            }

            // fait bouger l'ennemie
            Y += moveDirection;

            // compte le nombre d'itération de la méthode
            moveTiming++;

            // déplace le rectange de détection de collision avec le modele de l'ennemie
            enemyRectCollision.X = X;
            enemyRectCollision.Y = Y;
        }
    }
}
