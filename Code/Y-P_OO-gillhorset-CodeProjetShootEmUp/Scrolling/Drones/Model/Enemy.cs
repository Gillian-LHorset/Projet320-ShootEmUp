using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// modèle des ennemies
namespace Scramble
{    
    public partial class Enemy
    {
        // positions de l'ennemie sur AirSpace
        public int X;
        public int Y;

        // Valeur de taille d'un ennemie par défaut
        protected int width;
        protected int height;

        // rectangle de collision du tir de l'ennemie
        public Rectangle SniperShoot;

        // Valeur, devant être redéfini, de la vie d'un ennemie
        public int HealPoint;

        // défini la dernière fois que l'ennemie à tiré
        private DateTime _lastEnemyBulletShoot;
        // défini le temps entre deux tir d'un ennemie
        protected TimeSpan enemyShootCooldown;

        // rectangle servant à détecter les collisions
        public Rectangle EnemyRectCollision;

        // liste des tirs de l'ennmie
        public List<Shoot> EnemyShoots = new List<Shoot>();

        public Enemy(int x, int y) { 
            // Défini la position de l'ennemie lors de la création de l'objet
            X = x;
            Y = y;
        }

        /// <summary>
        /// Fait tirer l'ennemie sur AirSpace avec un temps entre 2 tirs
        /// </summary>
        public virtual void EnemyShoot()
        {
            if (DateTime.Now - _lastEnemyBulletShoot >= enemyShootCooldown)
            // si le temps acctuel moins la dernière fois que l'ennemie à tiré est supperieur au temps entre deux tirs
            {
                // crée un projectile propre à l'ennemie
                Shoot aEnemyShoot = new Shoot(X, Y + height / 2, false);
                // ajoute le tir dans la liste de tirs propre à l'ennemie
                EnemyShoots.Add(aEnemyShoot);

                // défini que le dernier tir viens d'avoir lieu
                _lastEnemyBulletShoot = DateTime.Now;
            }
        }

        /// <summary>
        /// Permet à l'ennemie de se déplacer sur AirSpace
        /// </summary>
        public virtual void EnemyMove()
        {

        }

    }
}
