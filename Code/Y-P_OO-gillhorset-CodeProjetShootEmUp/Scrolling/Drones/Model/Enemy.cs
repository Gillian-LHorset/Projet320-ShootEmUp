using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// model enemy
namespace Scramble
{    
    public partial class Enemy
    {
        public int X;
        public int Y;

        public readonly int WIDTH = 39;
        public readonly int HEIGHT = 54;

        public int healPoint;

        public Rectangle enemyRectCollision;

        private DateTime _lastEnemyBulletShoot;
        private TimeSpan _enemyShootCooldown = TimeSpan.FromSeconds(1.5);

        public List<Shoot> enemyShoots = new List<Shoot>();

        public Enemy(int x, int y) { 
            X = x;
            Y = y;

            enemyRectCollision = new Rectangle(x, y, WIDTH, HEIGHT);
        }

        public void EnemyShoot()
        {
            if (DateTime.Now - _lastEnemyBulletShoot >= _enemyShootCooldown)
            {
                Shoot aEnemyShoot = new Shoot(X, Y + HEIGHT / 2, false);
                enemyShoots.Add(aEnemyShoot);
                this._lastEnemyBulletShoot = DateTime.Now;
            }
        }

    }
}
