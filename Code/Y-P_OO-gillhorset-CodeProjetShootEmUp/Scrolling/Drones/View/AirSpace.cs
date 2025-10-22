using Scramble.Properties;
using System;
using System.Diagnostics;
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
        bool isHealItemShipCollision;

        


        List<Enemy> AllEnemysList = new List<Enemy>();

        // -- zone de test -- //
        BasicEnemy aEnemy = new BasicEnemy(400, 400);
        BasicEnemy anotherEnemy = new BasicEnemy(600, 600);
        HealItem aHealItem = new HealItem(100, 100);
        // -- fin zone de test -- //

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

            // -- zone de test -- //
            healItems.Add(aHealItem);
            AllEnemysList.Add(aEnemy);
            AllEnemysList.Add(anotherEnemy);
        // -- fin zone de test -- //

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
                    ship.ShipGround[i] = ground[i];
                }
            }

            // va déplacer les tirs du joueur
            Shoot.ShootMove(ship.playerShoots);

            for (int i = ship.playerShoots.Count - 1; i >= 0; i--)
            {
                ship.playerShoots[i].Render(airspace, ship.playerShoots);
            }


            if (healItems.Count > 0)
            {
                foreach (var aHealItem in healItems.ToList())
                {
                    aHealItem.Render(airspace);
                }
            }

            // affiche tous les enemies présent dans la liste AllEnemysList
            if (AllEnemysList.Count > 0)
            {
                foreach (var aEnemy in AllEnemysList.ToList())
                {
                    aEnemy.Render(airspace);
                }
            }

            // va verifier si le joueur entre en colision avec un enemie
            CheckEnemyCollision(ship, AllEnemysList);

            // va verifier si le joueur entre en colision avec un pack de soin
            CheckHealItemCollision();

            foreach (var aEnemy in AllEnemysList.ToList())
            {
                aEnemy.EnemyShoot();
                Shoot.ShootMove(aEnemy.enemyShoots);

                // Boucle pour afficher les tirs de l'ennemi
                
                foreach (var shoot in aEnemy.enemyShoots.ToList())
                {
                    shoot.Render(airspace, aEnemy.enemyShoots);

                    // vérifie si un tir enemy touche le jouer
                    CheckEnemyShootCollisionWhithPlayer(ship, aEnemy.enemyShoots);
                }
            }


            //foreach (var aEnemy in Enemys.ToList())
            //{
            //    aEnemy.EnemyShoot();
            //    Shoot.ShootMove(aEnemy.enemyShoots);
            //    foreach (var shoot in aEnemy.enemyShoots.ToList())
            //    {
            //        shoot.Render(airspace, aEnemy.enemyShoots);
            //    }
            //}


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
            }
        }

        public void CheckHealItemCollision()
        {
            foreach (var aHealItem in healItems.ToList())
            {
                isHealItemShipCollision = ship.shipRectCollision.IntersectsWith(aHealItem.healItemRectCollision);
                if (isHealItemShipCollision)
                {
                    break;
                }
            }
            if (isHealItemShipCollision)
            {
                if (ship.healPoint < 5)
                {
                    ship.healPoint++;
                    healItems.Remove(aHealItem);
                }
            }
            isHealItemShipCollision = false;
        }

        public void CheckEnemyCollision(Ship ship, List<Enemy> enemys)
        {
            foreach (Enemy enemy in enemys)
            {
                if (ship.shipRectCollision.IntersectsWith(enemy.enemyRectCollision) && ship.PlayerCanBeHit())
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
    }
}