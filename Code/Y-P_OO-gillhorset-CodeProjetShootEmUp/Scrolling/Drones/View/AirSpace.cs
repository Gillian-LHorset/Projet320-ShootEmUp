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
        private Ship ship;

        BufferedGraphicsContext currentContext;
        protected BufferedGraphics airspace;
        int[] ground = new int[WIDTH / 10+1];
        Brush groundBrush = new SolidBrush(Color.DarkGreen);
        Brush starBrush = new SolidBrush(Color.White);
        int scrollSmoother = 0;

        List<HealItem> healItems  = new List<HealItem>();

        List<MissileItem> missileItems = new List<MissileItem>();

        private bool _enemyDie;


        private int _timingExplosionPlayer;

        private int _explosionPositionX;
        private int _explosionPositionY;

        List<Enemy> AllEnemysList = new List<Enemy>();

        Brush fontBrush = new SolidBrush(Color.Black);

        private DateTime _lastEnemySpawned;
        private TimeSpan _enemySpawnCooldown;

        // emplacement d'apparition des �toiles
        //bool[,] stars = new bool[WIDTH/10, HEIGHT/10];


        // Initialisation de l'espace a�rien avec un certain nombre de ships
        public AirSpace(Ship ship)
        {
            InitializeComponent();
            // Gets a reference to the current BufferedGraphicsContext
            currentContext = BufferedGraphicsManager.Current;
            // Creates a BufferedGraphics instance associated with this form, and with
            // dimensions the same size as the drawing surface of the form.
            airspace = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);
            this.ship = ship;
            ground[0] = HEIGHT / 5;
            for (int i = 1; i < ground.Length; i++)
            {
                ground[i] = ground[i-1] + GlobalHelpers.alea.Next(0, 7)-3;
            }
            ClientSize = new Size(WIDTH, HEIGHT);

            //for (int i = 0; i < 100; i++)
            //{
            //    stars[GlobalHelpers.alea.Next(0, WIDTH/10), GlobalHelpers.alea.Next(0, HEIGHT/10)] = true;
            //}

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
                airspace.Graphics.FillRectangle(groundBrush, new Rectangle(i * 10 - scrollSmoother, HEIGHT - ground[i], 10, ground[i]));
            }
            

            if (ship.IsInLife)
            // si le joueur est encore en vie
            {


                // systeme de g�n�ration du sol
                scrollSmoother = (scrollSmoother + 5) % 10;
                if (scrollSmoother == 0)
                {
                    for (int i = 1; i < ground.Length; i++)
                    {
                        ground[i - 1] = ground[i];
                        ground[ground.Length - 1] = ground[ground.Length - 2] + GlobalHelpers.alea.Next(0, 7) - 3;
                        Ship.ShipGround[i] = ground[i];
                    }
                }

                // d�place les tirs du joueur
                foreach (var shoot in ship.playerShoots.ToList())
                {
                    shoot.ShootMove();
                    shoot.Render(airspace, ship.playerShoots);
                }

                // affiche tous les enemies pr�sent dans la liste AllEnemysList
                if (AllEnemysList.Count > 0)
                // si la liste d'ennemie est supperieur � 0
                {
                    foreach (var aEnemy in AllEnemysList.ToList())
                    {
                        aEnemy.EnemyMove();
                        aEnemy.Render(airspace);
                    }
                }

                // affiche les items missile
                foreach (var aMissileItem in missileItems)
                {
                    aMissileItem.Render(airspace);
                }


                if (healItems.Count > 0)
                // si il y a des items de heal sur AirSpace
                {
                    // affiche les items de heal
                    foreach (var aHealItem in healItems.ToList())
                    {
                        aHealItem.Render(airspace);
                    }
                }

                

                

                

                

                foreach (var aEnemy in AllEnemysList.ToList())
                {
                    aEnemy.EnemyShoot();

                    // Boucle pour afficher les tirs de l'ennemi

                    foreach (var shoot in aEnemy.enemyShoots.ToList())
                    {
                        shoot.Render(airspace, aEnemy.enemyShoots);
                        shoot.ShootMove();

                        // v�rifie si un tir enemy touche le jouer
                        CheckEnemyShootCollisionWhithPlayer(ship, aEnemy.enemyShoots);
                    }
                }
                CheckPlayerShootCollisionWhithEnemys(AllEnemysList, ship.playerShoots);


                // va afficher et faire bouger le joueur
                ship.Render(airspace);


            } else
            // si le joueur n'est plus en vie
            {
                // incr�ment la variable qui fait afficher les diff�rentes frames de l'explosion
                _timingExplosionPlayer++;
                
                // explosion du joueur
                if (_timingExplosionPlayer <= 4)
                {
                    airspace.Graphics.DrawImage(Resources.ShipExplosion1, ship.X, ship.Y, 100, 100);
                }
                else if (_timingExplosionPlayer <= 8)
                {
                airspace.Graphics.DrawImage(Resources.ShipExplosion2, ship.X, ship.Y, 100, 100);
                }
                else if (_timingExplosionPlayer <= 12)
                {
                airspace.Graphics.DrawImage(Resources.ShipExplosion3, ship.X, ship.Y, 100, 100);

                }
                else if (_timingExplosionPlayer <= 16)
                {
                airspace.Graphics.DrawImage(Resources.ShipExplosion4, ship.X, ship.Y, 100, 100);

                }

                // affiche le game over
                airspace.Graphics.DrawString(($"Game over"), new Font("Arial", 50), fontBrush, new PointF(WIDTH / 2 - 150, HEIGHT / 2 - ground[1] + 50));
            }

            GC.Collect();
        }

        // Calcul du nouvel �tat apr�s que 'interval' millisecondes se sont �coul�es
        private void Update(int interval)
        {
            // verifie les collision entre le joueur et le sol
            ship.CheckGroundCollisionPlayer();

            // va verifier si le joueur entre en colision avec un enemie
            CheckEnemyCollision(ship, AllEnemysList);

            // fait apparaitre les ennemies sur l'�cran
            SpawnRandomEnemy();

            // va verifier si le joueur entre en colision avec un pack de soin
            CheckHealItemCollision(healItems);

            // v�rifie la collision entre le joueur et les items r�cuperable de missiles
            CheckMissileItemCollision(missileItems);

            // verifie les collision entre le joueur et l'ennemie de type Sniper
            CheckSniperShootCollision();

            // affiche l'hud du joueur, donc sa reserve de missile
            Hud.Render(airspace, ship);
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
                    ship.GoUp = true;
                    break;
                case Keys.A:
                    ship.GoLeft = true;
                    break;
                case Keys.S:
                    ship.GoDown = true;
                    break;
                case Keys.D:
                    ship.GoRight = true;
                    break;
                case Keys.Space:
                    ship.isShooting = true;
                    break;
                case Keys.E:
                    ship.isShootingMissile = true;
                    break;
            }
        }

        // Read quel keys est mont�
        private void AirSpace_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    ship.GoUp = false;
                    break;
                case Keys.A:
                    ship.GoLeft = false;
                    break;
                case Keys.S:
                    ship.GoDown = false;
                    break;
                case Keys.D:
                    ship.GoRight = false;
                    break;
                case Keys.Space:
                    ship.isShooting = false;
                    break;
                case Keys.E:
                    ship.isShootingMissile = false;
                    break;
            }
        }

        /// <summary>
        /// cette m�thode v�rifie les collisions entre le joueur et les items de heal sur la map
        /// </summary>
        /// <param name="healItems"></param>
        public void CheckHealItemCollision(List<HealItem> healItems)
        {
            foreach (var aHealItem in healItems.ToList())
            {                
                if (ship.shipRectCollision.IntersectsWith(aHealItem.healItemRectCollision) && ship.healPoint < 5)
                {
                    ship.healPoint++;
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
            foreach (var aMissileItem in missileItems.ToList())
            {                
                if (ship.shipRectCollision.IntersectsWith(aMissileItem.missileItemRectCollision))
                // si le joueur entre en collision avec un item r�cuperable de missile
                {
                    // incr�mente sa r�serve de missile
                    ship.reserveMissile++;

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
                if (ship.shipRectCollision.IntersectsWith(enemy.EnemyRectCollision) && ship.PlayerCanBeHit())
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
                if (shoot.IsAPlayerShoot == false && ship.shipRectCollision.IntersectsWith(shoot.ShootRectCollision) && ship.PlayerCanBeHit())
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
                                if (enemy.healPoint > 3)
                                // si les points de vie de l'ennemie son supperieur � trois
                                {
                                    // retire 3 points de vie � l'ennemie
                                    enemy.healPoint -= 3;
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
                                    if (enemy.healPoint > 1)
                                    // si les point de vie de l'ennemie sont supperieur � un
                                    {
                                        // retire un point de vie � l'ennemie
                                        enemy.healPoint--;
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
                                    healItems.Add(enemyDropHealItem);
                                } else if (GlobalHelpers.alea.Next(4) == 0)
                                // � sa mort, un enemie � une chance sur 20 de lacher un item de soin
                                {
                                    // cr�e un item rammasable missile
                                    MissileItem enemyDropMissileItem = new MissileItem(enemy.X, enemy.Y);
                                    // met un item rammasable missile dans la liste pr�vu � cet effet
                                    missileItems.Add(enemyDropMissileItem);
                                }

                                // supprime l'ennemie du jeu
                                enemys.Remove(enemy);
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
            foreach (var aEnemy in AllEnemysList)
            // recherche dans toute la liste d'ennemies pr�sent dans AirSpace
            {
                if (aEnemy.GetType() == typeof(SniperEnemy))
                // si l'ennemie est de type SniperEnemy
                {
                    if (ship.shipRectCollision.IntersectsWith(aEnemy.SniperShoot) && ship.PlayerCanBeHit())
                    // si le rectangle de collision entre en contacte avec le rectangle de d�gat de SniperShoot
                    //      et que le joueur peut �tre touch�
                    {
                        // le joueur est touch�
                        ship.PlayerHitIsNow();
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
                        AllEnemysList.Add(aBasicEnemy);
                        break;
                    case 2:
                        FrontEnemy aFrontEnemy = new FrontEnemy(AirSpace.WIDTH, GlobalHelpers.alea.Next(0, 700));
                        AllEnemysList.Add(aFrontEnemy);
                        break;
                    case 3:
                        SniperEnemy aSniperEnemy = new SniperEnemy(AirSpace.WIDTH, GlobalHelpers.alea.Next(0, 700));
                        AllEnemysList.Add(aSniperEnemy);
                        break;
                    case 4:
                        TankEnemy aTankEnemy = new TankEnemy(ship, AirSpace.WIDTH, GlobalHelpers.alea.Next(0, 700));
                        AllEnemysList.Add(aTankEnemy);
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