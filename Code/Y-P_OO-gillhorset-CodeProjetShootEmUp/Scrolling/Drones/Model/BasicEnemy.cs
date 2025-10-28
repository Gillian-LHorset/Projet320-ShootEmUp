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
        public readonly int HEALPOINTMAX = 3;

        // variable utilisé pour définir à quel moment l'ennemie change de direction
        private int _moveTiming;

        // défini la direction verticale que l'ennemie prend
        private int _moveDirection;

        // défini une position aléatoire pour éviter que tous les ennemies soit visuellement endroit horizontalement
        private int _rndPositionX = GlobalHelpers.alea.Next(1000, (AirSpace.WIDTH - 100));

        public BasicEnemy(int x, int y) : base(x, y)
        {
            // défini les points de vie de l'ennemie à sa création
            HealPoint = HEALPOINTMAX;

            // défini la taille de l'ennemie
            width = 39;
            height = 54;

            // défini le cooldown de tir en modifiant la variable qui est présente dans la classe parent
            enemyShootCooldown = TimeSpan.FromSeconds(1.5);

            // défini le rectangle de collision à la position et aux proportions de l'ennemie
            EnemyRectCollision = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Permet à l'ennemie de se déplacer sur la carte
        /// </summary>
        public override void EnemyMove()
        {
            // une position aléatoire est donné pour éviter que tous les ennemies soit visuellement endroit horizontalement
            if (X < _rndPositionX)
            // si l'ennemie est trop à gauche
            {
                // le move speed de l'ennemie le ramenera à droite
                X += _moveSpeed;
            }

            if (X+width > _rndPositionX)
            // si l'ennemie est hors de l'écran à droite
            {
                // son move speed le ramenera à gauche
                X -= _moveSpeed;
            }

            if (_moveTiming % 50 == 0)
            // crée un situation ou l'enemie peut bouger toute les 50 itération de la méthode
            {
                if (GlobalHelpers.alea.Next(50) < 25)
                    // crée une situation ou l'enemie à une chance sur deux de monter
                {
                    if (Y - _moveSpeed > 20)
                    // si la position de l'ennemie ne va pas être en dehors de l'écran
                    {
                        // fait monter l'ennemie
                        _moveDirection = -5;
                    }
                }
                else if (Y + height + _moveSpeed < AirSpace.HEIGHT - Ship.ShipGround[X / 10])
                // si la poisition de l'ennemie n'est pas dans le sol après execution
                {
                    // il peut bouger
                    _moveDirection = 5;
                } else
                {
                    // il ne peut pas bouger
                    _moveDirection = 0;
                }
            }                      

            if (Y + height +10 > AirSpace.HEIGHT - Ship.ShipGround[X / 10])
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
            Y += _moveDirection;

            // compte le nombre d'itération de la méthode
            _moveTiming++;

            // déplace le rectange de détection de collision avec le modele de l'ennemie
            EnemyRectCollision.X = X;
            EnemyRectCollision.Y = Y;
        }
    }
}
