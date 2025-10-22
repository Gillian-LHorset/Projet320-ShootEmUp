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
        public new readonly int WIDTH = 39;
        public new readonly int HEIGHT = 54;

        // définie la vitesse de déplacement de l'enemie
        private int _moveSpeed = 5;

        private DateTime _lastEnemyBulletShoot;
        private TimeSpan _enemyShootCooldown = TimeSpan.FromSeconds(1.5);

        // définie la vie maximum de l'ennemie
        readonly int HEALPOINTMAX = 3;

        int moveTiming;
        int moveDirection;

        // défini une position aléatoire pour éviter que tous les ennemies soit visuellement endroit horizontalement
        int rndPositionX = GlobalHelpers.alea.Next(1300, (AirSpace.WIDTH - 30));

        public BasicEnemy(int x, int y) : base(x, y)
        {
            healPoint = 3;
        }
        public override void EnemyShoot()
        {
            if (DateTime.Now - _lastEnemyBulletShoot >= _enemyShootCooldown)
            {
                Shoot aEnemyShoot = new Shoot(X, Y + HEIGHT / 2, false);
                enemyShoots.Add(aEnemyShoot);
                this._lastEnemyBulletShoot = DateTime.Now;
            }
        }

        public override void EnemyMove()
        {
            // défini une position aléatoire pour éviter que tous les ennemies soit visuellement endroit horizontalement
            // si l'ennemie est trop à gauche, il est ramené à droite de l'écran
            if (X < rndPositionX)
            {
                X += _moveSpeed;
            }

            // si l'ennemie est hors de l'écran horizontalement, il sera ramené dedans
            if (X+WIDTH > AirSpace.WIDTH)
            {
                X -= _moveSpeed;
            }

            if (moveTiming % 50 == 0)
            // crée un situation ou l'enemie peut bouger toute les 50 itération de la méthode
            {
                if (GlobalHelpers.alea.Next(50) < 25)
                    // crée une situation ou l'enemie à une chance sur deux de monter
                {
                    if (Y - _moveSpeed > 20)
                    // si la position de l'ennemie ne va pas être en dehors de l'écran, on le fait monté
                    {
                        moveDirection = -5;
                    }
                }
                else if (Y + HEIGHT + _moveSpeed < AirSpace.HEIGHT - Ship.ShipGround[X / 10])
                // si la poisition de l'ennemie n'est pas dans le sol après execution, alors on execute
                {
                    moveDirection = 5;
                } else
                {
                    moveDirection = 0;
                }
            }                      

            if (Y + HEIGHT +10 > AirSpace.HEIGHT - Ship.ShipGround[X / 10])
            // si l'ennemie est dans le sol, il sera remonter
            {
                Y -= _moveSpeed;
            }
            if (Y < 0)
            // si l'ennemie est hors de l'écran car il est trop haut, il sera ramené sur l'écran
            {
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
