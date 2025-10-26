using Scramble.Properties;
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


        List<Enemy> AllEnemysList = new List<Enemy>();

        private DateTime _lastEnemySpawned;
        private TimeSpan _enemySpawnCooldown;

        // emplacement d'apparition des étoiles
        //bool[,] stars = new bool[WIDTH/10, HEIGHT/10];


        // Initialisation de l'espace aérien avec un certain nombre de ships
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
            //  draw stars
            //for (int i = 1; i < 150; i++)
            //{
            //    for (int j = 1; j < 90; j++)
            //    {
            //        if (stars[i, j] == true)
            //            airspace.Graphics.FillRectangle(starBrush, new Rectangle(i * 10, j * 10, 10, 10));
            //    }
            //}

            // couleur de fond du niveau
            airspace.Graphics.Clear(Color.AliceBlue);

            // va afficher et faire bouger le joueur
            ship.Render(airspace);
            
            // systeme de génération du sol
            for (int i = 0; i < ground.Length; i++)
            {
                airspace.Graphics.FillRectangle(groundBrush, new Rectangle(i * 10-scrollSmoother, HEIGHT - ground[i], 10, ground[i]));
            }
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

            // déplace les tirs du joueur
            foreach (var shoot in ship.playerShoots.ToList())
            {
                shoot.ShootMove();
                shoot.Render(airspace, ship.playerShoots);
            }

            foreach (var aMissileItem in missileItems)
            {
                aMissileItem.Render(airspace);
            }


            if (healItems.Count > 0)
            {
                foreach (var aHealItem in healItems.ToList())
                {
                    aHealItem.Render(airspace);
                }
            }

            SpawnRandomEnemy();

            // affiche tous les enemies présent dans la liste AllEnemysList
            if (AllEnemysList.Count > 0)
            // si la liste d'ennemie est supperieur à 0
            {
                foreach (var aEnemy in AllEnemysList.ToList())
                {
                    aEnemy.EnemyMove();
                    aEnemy.Render(airspace);
                }
            }

            // va verifier si le joueur entre en colision avec un enemie
            CheckEnemyCollision(ship, AllEnemysList);

            // va verifier si le joueur entre en colision avec un pack de soin
            CheckHealItemCollision(healItems);

            // vérifie la collision entre le joueur et les items récuperable de missiles
            CheckMissileItemCollision(missileItems);

            //
            CheckSniperShootCollision();

            foreach (var aEnemy in AllEnemysList.ToList())
            {
                aEnemy.EnemyShoot();                

                // Boucle pour afficher les tirs de l'ennemi
                
                foreach (var shoot in aEnemy.enemyShoots.ToList())
                {
                    shoot.Render(airspace, aEnemy.enemyShoots);
                    shoot.ShootMove();

                    // vérifie si un tir enemy touche le jouer
                    CheckEnemyShootCollisionWhithPlayer(ship, aEnemy.enemyShoots);
                }
            }
            CheckPlayerShootCollisionWhithEnemys(AllEnemysList, ship.playerShoots);

            if (Ship.isInLife)
            {
                airspace.Render();
            }
            GC.Collect();

        }

        // Calcul du nouvel état après que 'interval' millisecondes se sont écoulées
        private void Update(int interval)
        {
            ship.CheckGroundCollisionPlayer();
        }

        // Méthode appelée à chaque frame
        private void NewFrame(object sender, EventArgs e)
        {
            this.Update(ticker.Interval);
            this.Render();
        }

        // Read key
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
        /// Cette méthode détecte si le joueur est en collision avec un item missile et icrémente sa reserve de missile si oui
        /// </summary>
        /// <param name="missileItems">Correspond à la liste de missile présent dans AirSpace</param>
        public void CheckMissileItemCollision(List<MissileItem> missileItems)
        {
            foreach (var aMissileItem in missileItems.ToList())
            {                
                if (ship.shipRectCollision.IntersectsWith(aMissileItem.missileItemRectCollision))
                // si le joueur entre en collision avec un item récuperable de missile
                {
                    // incrémente sa réserve de missile
                    ship.reserveMissile++;

                    // retire l'item du jeu
                    missileItems.Remove(aMissileItem);
                }
            }
        }

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
                                if (enemy.healPoint > 3)
                                // si les points de vie de l'ennemie son supperieur à trois
                                {
                                    // retire 3 points de vie à l'ennemie
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
                                // si la classe de l'ennemie est différent de TankEnemy
                                { 
                                    if (enemy.healPoint > 1)
                                    // si les point de vie de l'ennemie sont supperieur à un
                                    {
                                        // retire un point de vie à l'ennemie
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
                                // à sa mort, un enemie à une chance sur 20 de lacher un item de soin
                                {
                                    // crée un item de soin
                                    HealItem enemyDropHealItem = new HealItem(enemy.X, enemy.Y);
                                    // met un item de soin dans la liste prévu à cet effet
                                    healItems.Add(enemyDropHealItem);
                                } else if (GlobalHelpers.alea.Next(15) == 0)
                                // à sa mort, un enemie à une chance sur 20 de lacher un item de soin
                                {
                                    // crée un item rammasable missile
                                    MissileItem enemyDropMissileItem = new MissileItem(enemy.X, enemy.Y);
                                    // met un item rammasable missile dans la liste prévu à cet effet
                                    missileItems.Add(enemyDropMissileItem);
                                }

                                // supprime l'ennemie du jeu
                                enemys.Remove(enemy);
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
            foreach (var aEnemy in AllEnemysList)
            // recherche dans toute la liste d'ennemies présent dans AirSpace
            {
                if (aEnemy.GetType() == typeof(SniperEnemy))
                // si l'ennemie est de type SniperEnemy
                {
                    if (ship.shipRectCollision.IntersectsWith(aEnemy.SniperShoot) && ship.PlayerCanBeHit())
                    // si le rectangle de collision entre en contacte avec le rectangle de dégat de SniperShoot
                    //      et que le joueur peut être touché
                    {
                        // le joueur est touché
                        ship.PlayerHitIsNow();
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

                // défini la derniere fois que un ennemie a spawn
                _lastEnemySpawned = DateTime.Now;
                // génère un cooldown aléatoire pour le prochain spawn d'ennemie
                _enemySpawnCooldown = TimeSpan.FromSeconds(GlobalHelpers.alea.Next(6, 9));
            }
            

        }

    }
}