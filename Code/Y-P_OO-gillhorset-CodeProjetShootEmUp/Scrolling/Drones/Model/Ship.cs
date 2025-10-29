using Scramble.View;
using System.Runtime.InteropServices.JavaScript;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace Scramble
{
    // Cette partie de la classe ship définit ce qu'est un ship par un modèle numérique
    public partial class Ship
    {
        // Position en X depuis la gauche de l'espace aérien
        private int _x;
        private int _y;

        // rectangle ayant pour but d'être utiliser lors de la verification des collisions
        public Rectangle ShipRectCollision;

        // Constructeur
        public Ship(int x, int y)
        {
            _x = x;
            _y = y;

            ShipRectCollision = new Rectangle(_x, _y, WIDTH, HEIGHT);
        }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y;} set { _y = value; } }

        // Cette méthode calcule le nouvel état dans lequel le ship se trouve après
        // que 'interval' millisecondes se sont écoulées
        public bool GoUp { get { return _goUpBool; } set { _goUpBool = value; } }
        public bool GoLeft { get { return _goLeftBool; } set { _goLeftBool = value; } }
        public bool GoDown { get { return _goDownBool; } set { _goDownBool = value; } }
        public bool GoRight { get { return _goRightBool; } set { _goRightBool = value; } }
        
        private bool _goUpBool;
        private bool _goLeftBool;
        private bool _goDownBool;
        private bool _goRightBool;

        // vie maximal du joueur
        public readonly int MaxHealthPoint = 5;

        // vie acctuel du joueur
        public int HealthPoint = 5;

        // défini le nombre de missile que le joueur peut encore tirer
        public int ReserveMissile = 3;

        // défini quand le joueur tir
        public bool IsShooting = false;

        // défini quand le joueur tir un missile
        public bool IsShootingMissile = false;

        // liste des tirs du joueur
        public List<Shoot> PlayerShoots = new List<Shoot>();

        public bool IsOkey = false;
        public bool HaveShoot = false;

        // défini si le joueur est en vie
        public bool IsInLife = true;

        // tableau contenant les emplacement du sol
        public static int[] ShipGround = new int[AirSpace.WIDTH / 10 + 1];

        // mise en place du cooldown de check de collision avec le sol
        private DateTime _lastCollisionCheck;
        private TimeSpan _collisionCooldown = TimeSpan.FromSeconds(2);

        // mise en place du cooldown de tirs
        private DateTime _lastBulletShoot;
        private TimeSpan _shootCooldown = TimeSpan.FromSeconds(0.4);

        // mise en place du cooldown de tirs de missile
        private DateTime _lastMissileShoot;
        private TimeSpan _missileCooldown = TimeSpan.FromSeconds(1);

        public void MoveShip()
        {            
            if (_goUpBool)
            {
                if (_y > 10)
                {
                    _y -= 15;
                }
            }
            if (_goLeftBool)
            {
                if (_x > 15)
                {
                    _x -= 15;
                }

            }
            if (_goDownBool)
            {
                if (Ship.HEIGHT + _y + 30 < AirSpace.HEIGHT - ShipGround[_x / 10])  
                    // position _x du vaisseau entre crochet
                    // si la hauteur de la position du vaisseau et sa taille sont superieur à la hauteur de l'écran moins la hauteur du sol
                {
                    IsOkey = true;
                }
                else
                {
                    IsOkey = false;
                }
                if (IsOkey && _y < AirSpace.HEIGHT - Ship.HEIGHT)
                {
                    _y += 15;
                }

            }
            if (_goRightBool)
            {
                if (_x < AirSpace.WIDTH - Ship.WIDTH - 10)
                {
                    _x += 15;
                }
            }

            if (IsShooting)
            {
                if (DateTime.Now - _lastBulletShoot >= _shootCooldown)
                // si le temps acctuel moins la dernière fois que le joueur à tiré est supperieur au temps entre deux tirs
                {
                    // ajoute un tir, à la position du joueur, dans sa liste de tirs
                    Shoot aShoot = new Shoot(_x + Ship.WIDTH, _y + Ship.HEIGHT / 2, true);
                    PlayerShoots.Add(aShoot);

                    // défini que la dernière fois qu'il a tirer est maintenant
                    _lastBulletShoot = DateTime.Now;
                }
            }
            if (IsShootingMissile)
            {
                PlayerShootMissile();
            }
            
            // défini le rectangle de collision aux positions du vaisseau
            ShipRectCollision.X = _x;
            ShipRectCollision.Y = _y;
        }

        /// <summary>
        /// La méthode détecte si le joueur est en collision avec le sol
        /// </summary>
        /// <returns>
        /// s'il est en collision --> oui
        /// s'il n'est pas en collision --> non
        /// </returns>
        public bool CheckGroundCollisionPlayer()
        {
            if (Ship.HEIGHT + _y > AirSpace.HEIGHT - ShipGround[_x / 10])
            // si la position verticale du joueur est superieur à la valeur du tableau ShipGround qui correspond à sa position horisontale
            // la valeur du tableau ShipGround définie la hauteur du sol
            {

                if (!PlayerCanBeHit())
                // si le joueur ne peut pas être touché
                {
                    return false; // encore en cooldown, on quitte sans exécuter
                }
                PlayerHitIsNow();
            }
            if (!PlayerCanBeHit())
            // si le joueur ne peut pas être touché
            {
                return false;

            } else
            {
                return true;
            }
        }

        /// <summary>
        /// La méthode va définir la dernière fois que le joueur à été touché et lui retirer
        /// </summary>
        public void PlayerHitIsNow()
        {
            _lastCollisionCheck = DateTime.Now;
            // retire 1 point de vie au joueur   
            HealthPoint--;

            if (HealthPoint < 1)
            // si les points de vie sont inferieur à 1
            {
                // défini la mort du joueur
                IsInLife = false;
            }
        }

        /// <summary>
        /// Cette méthode va checker si le joueur peut resevoir des dégats ou si il est encore en cooldown de dégat
        /// </summary>
        /// <returns>
        /// S'il peut en resevoir --> oui
        /// S'il ne peut pas en resevoir --> non
        /// </returns>
        public bool PlayerCanBeHit()
        {
            bool canBeHit;
            if (DateTime.Now - _lastCollisionCheck > _collisionCooldown)
            // si la dernière fois qu'il a été tappé est supperieur au cooldown de dégat,
            {
                // il peut a nouveau être tappé
                canBeHit = true;
            } else
            {
                // sinon le cooldown n'est pas fini donc il ne peux pas l'être
                canBeHit = false;
            }
            // va retourner si le joueur peut être touché ou non
            return canBeHit;
        }
        
        /// <summary>
        /// Le joueur tir un missile
        /// </summary>
        public void PlayerShootMissile()
        {
            if (IsShootingMissile == true && DateTime.Now - _lastMissileShoot > _missileCooldown && ReserveMissile > 0)
            // si le joueur est en train de tirer,
            //      que la date du dernier tir est supperieur au cooldown
            //      et que le joueur a encore un missile
            {
                // ajoute un missile dans la liste des tirs du joueur
                Missile aMissile = new Missile(_x + WIDTH, _y, true);
                PlayerShoots.Add(aMissile);

                // on retire de la reserve du joueur le missile qu'il vient de tirer
                ReserveMissile--;

                // on définie que le dernier tir est maintenant
                _lastMissileShoot = DateTime.Now;
            }
        }

    }
}