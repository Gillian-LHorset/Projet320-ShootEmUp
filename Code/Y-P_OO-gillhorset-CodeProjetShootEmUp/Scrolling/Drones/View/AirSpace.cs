using Scramble.Properties;
using Scramble.View;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace Scramble
{
    // La classe AirSpace représente le territoire au dessus duquel les vaisseau peuvent voler
    // Il s'agit d'un formulaire (une fenêtre) qui montre une vue 2D depuis le coté

    public partial class AirSpace : Form
    {
        public static readonly int WIDTH = 1500;        // Dimensions of the airspace
        public static readonly int HEIGHT = 900;

        // La flotte est l'ensemble des ships qui évoluent dans notre espace aérien
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

        // score nécessaire pour la victoire
        public static int ScorePoint = 0;

        // Initialisation de l'espace aérien avec un certain nombre de ships
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

            // système s'affichage du sol
            for (int i = 0; i < ground.Length; i++)
            {
                airspace.Graphics.FillRectangle(groundBrush, new Rectangle(i * 10 - _scrollSmoother, HEIGHT - ground[i], 10, ground[i]));
            }

            if (_ship.IsInLife)
            {
                // systeme de génération du sol
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

                // déplace les tirs du joueur
                foreach (Shoot shoot in _ship.PlayerShoots.ToList())
                {
                    shoot.ShootMove();
                    shoot.Render(airspace, _ship.PlayerShoots);
                }

                // affiche tous les enemies présent dans la liste AllEnemysList
                if (_allEnemysList.Count > 0)
                // si la liste d'ennemie est supperieur à 0
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

                        // vérifie si un tir enemy touche le jouer
                        CheckEnemyShootCollisionWhithPlayer(_ship, aEnemy.EnemyShoots);
                    }
                }
                CheckPlayerShootCollisionWhithEnemys(_allEnemysList, _ship.PlayerShoots);
            } else if (!_ship.IsInLife)
            // si le joueur n'est plus en vie
            {
                // incrément la variable qui fait afficher les différentes frames de l'explosion
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
                // affiche l'écran de victoire
                airspace.Graphics.DrawString(($"Vous avez gagné !"), new Font("Arial", 50), _fontBrush, new PointF(WIDTH / 2 - 250, HEIGHT / 2 - 100));
            }

            // utilise le garbadge collector pour éviter une surcharge de la mémoire
            GC.Collect();
        }

        // Calcul du nouvel état après que 'interval' millisecondes se sont écoulées
        private void Update(int interval)
        {
            if (_ship.IsInLife && ScorePoint < 2000)
            {
                // verifie les collision entre le joueur et le sol
                _ship.CheckGroundCollisionPlayer();

                // va verifier si le joueur entre en colision avec un enemie
                CheckEnemyCollision(_ship, _allEnemysList);

                // fait apparaitre les ennemies sur l'écran
                SpawnRandomEnemy();

                // va verifier si le joueur entre en colision avec un pack de soin
                CheckHealItemCollision(_healItems);

                // vérifie la collision entre le joueur et les items récuperable de missiles
                CheckMissileItemCollision(_missileItems);

                // verifie les collision entre le joueur et l'ennemie de type Sniper
                CheckSniperShootCollision();
            }
        }

        // Méthode appelée à chaque frame
        private void NewFrame(object sender, EventArgs e)
        {
            this.Update(ticker.Interval);
            this.Render();
        }

        // Read quel keys est pressé
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

        // Read quel keys est monté
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
        /// cette méthode vérifie les collisions entre le joueur et les items de heal sur la map
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
        /// Cette méthode détecte si le joueur est en collision avec un item missile et icrémente sa reserve de missile si oui
        /// </summary>
        /// <param name="missileItems">Correspond à la liste de missile présent dans AirSpace</param>
        public void CheckMissileItemCollision(List<MissileItem> missileItems)
        {
            foreach (MissileItem aMissileItem in missileItems.ToList())
            {                
                if (_ship.ShipRectCollision.IntersectsWith(aMissileItem.MissileItemRectCollision))
                // si le joueur entre en collision avec un item récuperable de missile
                {
                    // incrémente sa réserve de missile
                    _ship.ReserveMissile++;

                    // retire l'item du jeu
                    missileItems.Remove(aMissileItem);
                }
            }
        }

        /// <summary>
        /// cette méthode verife la collision directe entre le joueur et un ennemie
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
        /// cette méthode gere les collisions entre les ennemy et les tirs du joueur
        /// </summary>
        /// <param name="enemys"></param>
        /// <param name="shoots"></param>
        public void CheckPlayerShootCollisionWhithEnemys(List<Enemy> enemys, List<Shoot> shoots)
        {
            foreach (Shoot shoot in shoots.ToList())
            {
                if (shoot.IsAPlayerShoot == true)
                // si shoot à été tirer par le joueur
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
                                // si les points de vie de l'ennemie son supperieur à trois
                                {
                                    // retire 3 points de vie à l'ennemie
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
                                // si la classe de l'ennemie est différent de TankEnemy
                                { 
                                    if (enemy.HealPoint > 1)
                                    // si les point de vie de l'ennemie sont supperieur à un
                                    {
                                        // retire un point de vie à l'ennemie
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
                                // à sa mort, un enemie à une chance sur 20 de lacher un item de soin
                                {
                                    // crée un item de soin
                                    HealItem enemyDropHealItem = new HealItem(enemy.X, enemy.Y);
                                    // met un item de soin dans la liste prévu à cet effet
                                    _healItems.Add(enemyDropHealItem);
                                } else if (GlobalHelpers.alea.Next(4) == 0)
                                // à sa mort, un enemie à une chance sur 20 de lacher un item de soin
                                {
                                    // crée un item rammasable missile
                                    MissileItem enemyDropMissileItem = new MissileItem(enemy.X, enemy.Y);
                                    // met un item rammasable missile dans la liste prévu à cet effet
                                    _missileItems.Add(enemyDropMissileItem);
                                }

                                // supprime l'ennemie du jeu
                                enemys.Remove(enemy);

                                // ajoute du score à la mort d'un ennemie
                                ScorePoint += 100;
                            }
                            // le tir ayant touché, il est retirer du jeu
                            shoots.Remove(shoot);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Méthode verifiant spécifiquement les tirs des ennemies de type SniperEnemy
        /// </summary>
        public void CheckSniperShootCollision()
        {            
            foreach (Enemy aEnemy in _allEnemysList)
            // recherche dans toute la liste d'ennemies présent dans AirSpace
            {
                if (aEnemy.GetType() == typeof(SniperEnemy))
                // si l'ennemie est de type SniperEnemy
                {
                    if (_ship.ShipRectCollision.IntersectsWith(aEnemy.SniperShoot) && _ship.PlayerCanBeHit())
                    // si le rectangle de collision entre en contacte avec le rectangle de dégat de SniperShoot
                    //      et que le joueur peut être touché
                    {
                        // le joueur est touché
                        _ship.PlayerHitIsNow();
                    }
                }
            }
        }

        /// <summary>
        /// fait apparaitre des ennemies aléatoir sur AirSpace à chaque fois que le temps donnée est passé
        /// </summary>
        public void SpawnRandomEnemy()
        {
            if (DateTime.Now - _lastEnemySpawned >= _enemySpawnCooldown)
            // si le temps acctuel moins la dernière fois qu'un ennemie a spawn est supperieur au cooldown entre deux spawn
            {
                // fait spawn un ennemie au hazard
                switch (GlobalHelpers.alea.Next(1, 5))
                // génère un nombre aléatoire entre 1 et 4
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

                // défini la derniere fois que un ennemie a spawn
                _lastEnemySpawned = DateTime.Now;
                // génère un cooldown aléatoire pour le prochain spawn d'ennemie
                _enemySpawnCooldown = TimeSpan.FromSeconds(GlobalHelpers.alea.Next(6, 9));
            }
            

        }

    }
}