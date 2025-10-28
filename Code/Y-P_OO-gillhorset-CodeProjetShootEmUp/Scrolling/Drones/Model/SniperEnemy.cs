using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class SniperEnemy : Enemy
    {
        // définie la vie maximum de l'ennemie
        public readonly int HEALPOINTMAX = 3;        

        // défini une position aléatoire pour éviter que tous les ennemies soit visuellement endroit horizontalement
        private int _rndPositionY;

        // défini la dernière fois que l'ennemie à tiré
        private DateTime _lastEnemyBulletShoot;

        // temps que va rester le tir de l'ennemie à l'écran
        private TimeSpan _shootDisplayCooldown = TimeSpan.FromSeconds(0.6);

        // taille verticale du tir de l'ennemie
        private int _laserHeight = 10;
        // position verticale du tir de l'ennemie
        protected int _laserYPosition = 100;

        // défini si l'ennemie peut tirer
        private bool _enemyCanShoot = false;

        // défini si l'ennemie est en position pour tirer
        private bool _isPositionValid = false;

        // défini quand l'ennemie est en position pour tirer
        private DateTime _whenIsInPosition = DateTime.Now;
        // défini l'espace de temps entre le moment ou l'ennemie est en position pour tirer et le moment ou il tir
        private TimeSpan _cooldownAfterMove = TimeSpan.FromSeconds(0.5);


        public SniperEnemy(int x, int y) : base(x, y)
        {
            // défini les points de vie de l'ennemie à sa création
            healPoint = HEALPOINTMAX;

            // défini la taille de l'ennemie
            WIDTH = 50;
            HEIGHT = 50;

            // défini le cooldown de tir en modifiant la variable qui est présente dans la classe parent
            _enemyShootCooldown = TimeSpan.FromSeconds(2);

            // défini le rectangle de collision à la position et aux proportions de l'ennemie
            EnemyRectCollision = new Rectangle(x, y, WIDTH, HEIGHT);


            // défini une position verticale aléatoire pour l'ennemie
            _rndPositionY = GlobalHelpers.alea.Next(20, AirSpace.HEIGHT - Ship.ShipGround[X / 10] + HEIGHT);
        }

        /// <summary>
        /// Fait tirer l'ennemie sur AirSpace avec un temps entre 2 tirs
        /// </summary>
        public override void EnemyShoot()
        {
            if (_enemyCanShoot)
            {
                // la position de l'ennemie ne corresponds plus à la position défini
                _isPositionValid = false;

                if (DateTime.Now - _lastEnemyBulletShoot >= _enemyShootCooldown)
                // si le temps acctuel moins la dernière fois que l'ennemie à tiré est supperieur au temps entre deux tirs
                {
                    // défini la taille verticale du tir
                    _laserHeight = 10;
                    // défini la position verticale du tir
                    //      en se basant sur la position de l'ennemie, sa taille et la taille du laser pour centrer visuellement le tir sur l'ennemie
                    _laserYPosition = Y + HEIGHT / 2 - _laserHeight / 2;

                    // défini que le dernier tir viens d'avoir lieu
                    _lastEnemyBulletShoot = DateTime.Now;
                }

                if (_laserHeight > 0)
                // si la taille du laser est plus grande que 0
                {
                    // le rectangle de collision est redessiné
                    SniperShoot = new Rectangle(0, _laserYPosition, X, _laserHeight);
                    // la position du laser est incrémenter pour rester centrer sur l'ennemie
                    _laserYPosition++;
                    // réduit la taille du tir de l'ennemie
                    _laserHeight -= 2;
                }
                else if (_laserHeight < 1)
                // si la hauteur du laser est plus petite que 1
                {
                    // supprime techniquement le rectangle de collision du tir de l'ennemie
                    SniperShoot = Rectangle.Empty;

                    //Console.WriteLine(_rndPositionY);
                    _enemyCanShoot = false;
                }
            }
        }

        /// <summary>
        /// Méthode faisant bouger l'ennemie de manière verticale aléatoirement et l'arrete avant de pouvoire tirer
        /// </summary>
        public override void EnemyMove()
        {
            if (X + WIDTH > AirSpace.WIDTH)
            // si l'ennemie est hors de l'écran à droite
            {
                // il se déplacera vers la gauche
                X -= 10;
            }

            if (X < AirSpace.WIDTH - WIDTH - 10)
            // si l'ennemie est trop à gauche
            {
                // il se déplacera vers la droite
                X += 10;
            }

            if (Y + HEIGHT > AirSpace.HEIGHT - Ship.ShipGround[X / 10])
            // si la position de l'ennemie est dans le le sol
            {
                // défini une position verticale aléatoire pour l'ennemie
                _rndPositionY = GlobalHelpers.alea.Next(20, AirSpace.HEIGHT - Ship.ShipGround[X / 10] + HEIGHT);
                // l'ennemie n'est pas en position pour tirer
                _isPositionValid = false;
            }

            if (!_enemyCanShoot && !_isPositionValid)
            // si l'ennemie n'est est dans sa nouvelle position pour tirer et que l'ennemie peut tirer
            {
                if (Y + 5 < _rndPositionY)
                // si la position de l'ennemie est plus bas que la position défini
                {
                    // l'ennemie monte
                    Y += 5;
                    // l'ennemie n'est pas en position pour tirer
                    _isPositionValid = false;
                }
                else if (Y - 5 > _rndPositionY)
                // si la position de l'ennemie est plus haut que la position dénini
                {
                    // l'ennemie descend
                    Y -= 5;
                    // l'ennemie n'est pas en position pour tirer
                    _isPositionValid = false;
                }
                else
                {
                    // l'ennemie est en position pour tirer
                    _isPositionValid = true;
                    // cet instant est défini comme étant le moment ou il est en position
                    _whenIsInPosition = DateTime.Now;
                }
            }
            else if(DateTime.Now - _whenIsInPosition >= _cooldownAfterMove)
            // si le moment présent moins le moment ou il a été en position est superieur au temps entre le moment ou il est en position et il peut tirer
            {
                // autorise l'ennemie à tirer
                _enemyCanShoot = true;

                // défini une nouvelle position verticale aléatoire pour l'ennemie
                _rndPositionY = GlobalHelpers.alea.Next(20, AirSpace.HEIGHT - Ship.ShipGround[X / 10] + HEIGHT);
            }

            // déplace le rectange de détection de collision avec le modele de l'ennemie
            EnemyRectCollision.X = X;
            EnemyRectCollision.Y = Y;
        }

    }
}
