using Scramble.Properties;
using Scramble.View;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace Scramble
{
    // La classe AirSpace repr�sente le territoire au dessus duquel les vaisseau peuvent voler
    // Il s'agit d'un formulaire (une fen�tre) qui montre une vue 2D depuis le cot�

    public partial class AirSpace : Form
    {
        public static readonly int WIDTH = 1500;        // Dimensions of the airspace
        public static readonly int HEIGHT = 900;

        // La flotte est l'ensemble des ships qui �voluent dans notre espace a�rien
        private Ship _ship;

        BufferedGraphicsContext currentContext;
        protected BufferedGraphics airspace;
        int[] ground = new int[WIDTH / 10+1];
        Brush groundBrush = new SolidBrush(Color.DarkGreen);
        private int _scrollSmoother = 0;

        private List<HealItem> _healItems = new List<HealItem>();

        private List<MissileItem> _missileItems = new List<MissileItem>();

        private bool _enemyDie;

        private int _timingExplosionPlayer;

        private List<Enemy> _allEnemysList = new List<Enemy>();

        private Brush _fontBrush = new SolidBrush(Color.Black);

        private DateTime _lastEnemySpawned;
        private TimeSpan _enemySpawnCooldown;

        // score n�cessaire pour la victoire
        public static int ScorePoint = 0;

        // Initialisation de l'espace a�rien avec un certain nombre de ships
        public AirSpace(Ship ship)
        {
            InitializeComponent();
            // Gets a reference to the current BufferedGraphicsContext
            currentContext = BufferedGraphicsManager.Current;
            // Creates a BufferedGraphics instance associated with this form, and with
            // dimensions the same size as the drawing surface of the form.
            airspace = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);
            this._ship = ship;
            ground[0] = HEIGHT / 5;
            for (int i = 1; i < ground.Length; i++)
            {
                ground[i] = ground[i-1] + GlobalHelpers.alea.Next(0, 7)-3;
            }
            ClientSize = new Size(WIDTH, HEIGHT);

            this.KeyPreview = true; // Ensures the form captures key events before child controls
            this.KeyDown += AirSpace_KeyDown;
            this.KeyUp += AirSpace_KeyUp;
        }

        // Affichage de la situation actuelle
        private void Render()
        {
            airspace.Render();

            // couleur de fond du niveau
            airspace.Graphics.Clear(Color.AliceBlue);

            // syst�me s'affichage du sol
            for (int i = 0; i < ground.Length; i++)
            {
                airspace.Graphics.FillRectangle(groundBrush, new Rectangle(i * 10 - _scrollSmoother, HEIGHT - ground[i], 10, ground[i]));
            }

            if (_ship.IsInLife)
            {
                // systeme de g�n�ration du sol
                _scrollSmoother = (_scrollSmoother + 5) % 10;
                if (_scrollSmoother == 0)
                {
                    for (int i = 1; i < ground.Length; i++)
                    {
                        ground[i - 1] = ground[i];
                        ground[ground.Length - 1] = ground[ground.Length - 2] + GlobalHelpers.alea.Next(0, 7) - 3;
                        Ship.ShipGround[i] = ground[i];
                    }
                }
            }

            // va affiche le vaisseau
            _ship.Render(airspace);

            // affiche l'hud du joueur, donc sa reserve de missile
            Hud.Render(airspace, _ship);

            if (_ship.IsInLife && ScorePoint < 2000)
            // si le joueur est encore en vie
            {
                // fait bouger le vaisseau
                _ship.MoveShip();

                // d�place les tirs du joueur
                foreach (Shoot shoot in _ship.PlayerShoots.ToList())
                {
                    shoot.ShootMove();
                    shoot.Render(airspace, _ship.PlayerShoots);
                }

                // affiche tous les enemies pr�sent dans la liste AllEnemysList
                if (_allEnemysList.Count > 0)
                // si la liste d'ennemie est supperieur � 0
                {
                    foreach (Enemy aEnemy in _allEnemysList.ToList())
                    {
                        aEnemy.EnemyMove();
                        aEnemy.Render(airspace);
                    }
                }

                // affiche les items missile
                foreach (MissileItem aMissileItem in _missileItems)
                {
                    aMissileItem.Render(airspace);
                }


                if (_healItems.Count > 0)
                // si il y a des items de heal sur AirSpace
                {
                    // affiche les items de heal
                    foreach (HealItem aHealItem in _healItems.ToList())
                    {
                        aHealItem.Render(airspace);
                    }
                }

                foreach (Enemy aEnemy in _allEnemysList.ToList())
                {
                    aEnemy.EnemyShoot();

                    // Boucle pour afficher les tirs de l'ennemi
                    foreach (Shoot shoot in aEnemy.EnemyShoots.ToList())
                    {
                        shoot.Render(airspace, aEnemy.EnemyShoots);
                        shoot.ShootMove();

                        // v�rifie si un tir enemy touche le jouer
                        CheckEnemyShootCollisionWhithPlayer(_ship, aEnemy.EnemyShoots);
                    }
                }
                CheckPlayerShootCollisionWhithEnemys(_allEnemysList, _ship.PlayerShoots);
            } else if (!_ship.IsInLife)
            // si le joueur n'est plus en vie
            {
                // incr�ment la variable qui fait afficher les diff�rentes frames de l'explosion
                _timingExplosionPlayer++;
                
                // explosion du joueur
                if (_timingExplosionPlayer <= 4)
                {
                    airspace.Graphics.DrawImage(Resources.ShipExplosion1, _ship.X, _ship.Y, 100, 100);
                }
                else if (_timingExplosionPlayer <= 8)
                {
                airspace.Graphics.DrawImage(Resources.ShipExplosion2, _ship.X, _ship.Y, 100, 100);
                }
                else if (_timingExplosionPlayer <= 12)
                {
                airspace.Graphics.DrawImage(Resources.ShipExplosion3, _ship.X, _ship.Y, 100, 100);

                }
                else if (_timingExplosionPlayer <= 16)
                {
                airspace.Graphics.DrawImage(Resources.ShipExplosion4, _ship.X, _ship.Y, 100, 100);

                }

                // affiche le game over
                airspace.Graphics.DrawString(($"Game over"), new Font("Arial", 50), _fontBrush, new PointF(WIDTH / 2 - 150, HEIGHT / 2 - ground[1] + 50));
            } 
            else if (ScorePoint >= 2000)
            {
                // affiche l'�cran de victoire
                airspace.Graphics.DrawString(($"Vous avez gagn� !"), new Font("Arial", 50), _fontBrush, new PointF(WIDTH / 2 - 250, HEIGHT / 2 - 100));
            }

            // utilise le garbadge collector pour �viter une surcharge de la m�moire
            GC.Collect();
        }

        // Calcul du nouvel �tat apr�s que 'interval' millisecondes se sont �coul�es
        private void Update(int interval)
        {
            if (_ship.IsInLife && ScorePoint < 2000)
            {
                // verifie les collision entre le joueur et le sol
                _ship.CheckGroundCollisionPlayer();

                // va verifier si le joueur entre en colision avec un enemie
                CheckEnemyCollision(_ship, _allEnemysList);

                // fait apparaitre les ennemies sur l'�cran
                SpawnRandomEnemy();

                // va verifier si le joueur entre en colision avec un pack de soin
                CheckHealItemCollision(_healItems);

                // v�rifie la collision entre le joueur et les items r�cuperable de missiles
                CheckMissileItemCollision(_missileItems);

                // verifie les collision entre le joueur et l'ennemie de type Sniper
                CheckSniperShootCollision();
            }
        }

        // M�thode appel�e � chaque frame
        private void NewFrame(object sender, EventArgs e)
        {
            this.Update(ticker.Interval);
            this.Render();
        }

        // Read quel keys est press�
        private void AirSpace_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    _ship.GoUp = true;
                    break;
                case Keys.A:
                    _ship.GoLeft = true;
                    break;
                case Keys.S:
                    _ship.GoDown = true;
                    break;
                case Keys.D:
                    _ship.GoRight = true;
                    break;
                case Keys.Space:
                    _ship.IsShooting = true;
                    break;
                case Keys.E:
                    _ship.IsShootingMissile = true;
                    break;
            }
        }

        // Read quel keys est mont�
        private void AirSpace_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    _ship.GoUp = false;
                    break;
                case Keys.A:
                    _ship.GoLeft = false;
                    break;
                case Keys.S:
                    _ship.GoDown = false;
                    break;
                case Keys.D:
                    _ship.GoRight = false;
                    break;
                case Keys.Space:
                    _ship.IsShooting = false;
                    break;
                case Keys.E:
                    _ship.IsShootingMissile = false;
                    break;
            }
        }

        /// <summary>
        /// cette m�thode v�rifie les collisions entre le joueur et les items de heal sur la map
        /// </summary>
        /// <param name="healItems"></param>
        public void CheckHealItemCollision(List<HealItem> healItems)
        {
            foreach (HealItem aHealItem in healItems.ToList())
            {                
                if (_ship.ShipRectCollision.IntersectsWith(aHealItem.HealItemRectCollision) && _ship.HealthPoint < 5)
                {
                    _ship.HealthPoint++;
                    healItems.Remove(aHealItem);
                }
            }
        }

        /// <summary>
        /// Cette m�thode d�tecte si le joueur est en collision avec un item missile et icr�mente sa reserve de missile si oui
        /// </summary>
        /// <param name="missileItems">Correspond � la liste de missile pr�sent dans AirSpace</param>
        public void CheckMissileItemCollision(List<MissileItem> missileItems)
        {
            foreach (MissileItem aMissileItem in missileItems.ToList())
            {                
                if (_ship.ShipRectCollision.IntersectsWith(aMissileItem.MissileItemRectCollision))
                // si le joueur entre en collision avec un item r�cuperable de missile
                {
                    // incr�mente sa r�serve de missile
                    _ship.ReserveMissile++;

                    // retire l'item du jeu
                    missileItems.Remove(aMissileItem);
                }
            }
        }

        /// <summary>
        /// cette m�thode verife la collision directe entre le joueur et un ennemie
        /// </summary>
        /// <param name="ship">l'objet du joueur</param>
        /// <param name="enemys">liste contenant tous les ennemies</param>
        public void CheckEnemyCollision(Ship ship, List<Enemy> enemys)
        {
            foreach (Enemy enemy in enemys)
            {
                if (ship.ShipRectCollision.IntersectsWith(enemy.EnemyRectCollision) && ship.PlayerCanBeHit())
                {
                    ship.PlayerHitIsNow();
                }
            }
        }

        /// <summary>
        /// verifie les collisions entre le joueur et les tirs ennemies
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="shoots"></param>
        public void CheckEnemyShootCollisionWhithPlayer(Ship ship, List<Shoot> shoots)
        {
            foreach (Shoot shoot in shoots.ToList())
            {
                if (shoot.IsAPlayerShoot == false && ship.ShipRectCollision.IntersectsWith(shoot.ShootRectCollision) && ship.PlayerCanBeHit())
                {
                    ship.PlayerHitIsNow();
                    shoots.Remove(shoot);
                }
            }
        }

        /// <summary>
        /// cette m�thode gere les collisions entre les ennemy et les tirs du joueur
        /// </summary>
        /// <param name="enemys"></param>
        /// <param name="shoots"></param>
        public void CheckPlayerShootCollisionWhithEnemys(List<Enemy> enemys, List<Shoot> shoots)
        {
            foreach (Shoot shoot in shoots.ToList())
            {
                if (shoot.IsAPlayerShoot == true)
                // si shoot � �t� tirer par le joueur
                {
                    foreach (Enemy enemy in enemys.ToList())
                    {
                        if (enemy.EnemyRectCollision.IntersectsWith(shoot.ShootRectCollision))
                        // si le rectangle de collision de l'ennemie rentre en contacte avec le rectangle de collision du shoot
                        {
                            // reset la variable qui dicte si l'ennemie est mort
                            _enemyDie = false;


                            if (shoot is Missile)
                            // si le shoot est de la classe missile
                            {
                                if (enemy.HealPoint > 3)
                                // si les points de vie de l'ennemie son supperieur � trois
                                {
                                    // retire 3 points de vie � l'ennemie
                                    enemy.HealPoint -= 3;
                                }
                                else
                                {
                                    // indique que l'ennemie est mort
                                    _enemyDie = true;
                                }
                            }
                            else if (shoot is Shoot)
                            // si le shoot est de la classe Shoot
                            {
                                if (!(enemy is TankEnemy))
                                // si la classe de l'ennemie est diff�rent de TankEnemy
                                { 
                                    if (enemy.HealPoint > 1)
                                    // si les point de vie de l'ennemie sont supperieur � un
                                    {
                                        // retire un point de vie � l'ennemie
                                        enemy.HealPoint--;
                                    }
                                    else
                                    {
                                        // indique que l'ennemie est mort
                                        _enemyDie = true;
                                    }
                                }
                            }

                            if (_enemyDie)
                            // si l'ennemie est mort
                            {
                                if (GlobalHelpers.alea.Next(19) == 0)
                                // � sa mort, un enemie � une chance sur 20 de lacher un item de soin
                                {
                                    // cr�e un item de soin
                                    HealItem enemyDropHealItem = new HealItem(enemy.X, enemy.Y);
                                    // met un item de soin dans la liste pr�vu � cet effet
                                    _healItems.Add(enemyDropHealItem);
                                } else if (GlobalHelpers.alea.Next(4) == 0)
                                // � sa mort, un enemie � une chance sur 20 de lacher un item de soin
                                {
                                    // cr�e un item rammasable missile
                                    MissileItem enemyDropMissileItem = new MissileItem(enemy.X, enemy.Y);
                                    // met un item rammasable missile dans la liste pr�vu � cet effet
                                    _missileItems.Add(enemyDropMissileItem);
                                }

                                // supprime l'ennemie du jeu
                                enemys.Remove(enemy);

                                // ajoute du score � la mort d'un ennemie
                                ScorePoint += 100;
                            }
                            // le tir ayant touch�, il est retirer du jeu
                            shoots.Remove(shoot);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// M�thode verifiant sp�cifiquement les tirs des ennemies de type SniperEnemy
        /// </summary>
        public void CheckSniperShootCollision()
        {            
            foreach (Enemy aEnemy in _allEnemysList)
            // recherche dans toute la liste d'ennemies pr�sent dans AirSpace
            {
                if (aEnemy.GetType() == typeof(SniperEnemy))
                // si l'ennemie est de type SniperEnemy
                {
                    if (_ship.ShipRectCollision.IntersectsWith(aEnemy.SniperShoot) && _ship.PlayerCanBeHit())
                    // si le rectangle de collision entre en contacte avec le rectangle de d�gat de SniperShoot
                    //      et que le joueur peut �tre touch�
                    {
                        // le joueur est touch�
                        _ship.PlayerHitIsNow();
                    }
                }
            }
        }

        /// <summary>
        /// fait apparaitre des ennemies al�atoir sur AirSpace � chaque fois que le temps donn�e est pass�
        /// </summary>
        public void SpawnRandomEnemy()
        {
            if (DateTime.Now - _lastEnemySpawned >= _enemySpawnCooldown)
            // si le temps acctuel moins la derni�re fois qu'un ennemie a spawn est supperieur au cooldown entre deux spawn
            {
                // fait spawn un ennemie au hazard
                switch (GlobalHelpers.alea.Next(1, 5))
                // g�n�re un nombre al�atoire entre 1 et 4
                {
                    case 1:
                        BasicEnemy aBasicEnemy = new BasicEnemy(AirSpace.WIDTH - 50, GlobalHelpers.alea.Next(0, 700));
                        _allEnemysList.Add(aBasicEnemy);
                        break;
                    case 2:
                        FrontEnemy aFrontEnemy = new FrontEnemy(AirSpace.WIDTH, GlobalHelpers.alea.Next(0, 700));
                        _allEnemysList.Add(aFrontEnemy);
                        break;
                    case 3:
                        SniperEnemy aSniperEnemy = new SniperEnemy(AirSpace.WIDTH, GlobalHelpers.alea.Next(0, 700));
                        _allEnemysList.Add(aSniperEnemy);
                        break;
                    case 4:
                        TankEnemy aTankEnemy = new TankEnemy(_ship, AirSpace.WIDTH, GlobalHelpers.alea.Next(0, 700));
                        _allEnemysList.Add(aTankEnemy);
                        break;
                }

                // d�fini la derniere fois que un ennemie a spawn
                _lastEnemySpawned = DateTime.Now;
                // g�n�re un cooldown al�atoire pour le prochain spawn d'ennemie
                _enemySpawnCooldown = TimeSpan.FromSeconds(GlobalHelpers.alea.Next(6, 9));
            }
            

        }

    }
}