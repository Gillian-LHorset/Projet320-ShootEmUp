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
        BufferedGraphics airspace;
        int[] ground = new int[WIDTH / 10+1];
        Brush groundBrush = new SolidBrush(Color.DarkGreen);
        int scrollSmoother = 0;

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


            this.KeyPreview = true; // Ensures the form captures key events before child controls
            this.KeyDown += AirSpace_KeyDown;
        }

        

        // Affichage de la situation actuelle
        private void Render()
        {
            airspace.Graphics.Clear(Color.AliceBlue);

            // draw ships
            
            ship.Render(airspace);
            
            for (int i = 0; i < ground.Length; i++)
            {
                airspace.Graphics.FillRectangle(groundBrush, new Rectangle(i * 10-scrollSmoother, HEIGHT - ground[i], 10, ground[i]));
            }
            scrollSmoother = (scrollSmoother + 5) % 10;
            if (scrollSmoother == 0)
            {
                for (int i = 1;i < ground.Length; i++) ground[i-1] = ground[i];
                ground[ground.Length - 1] = ground[ground.Length - 2] + GlobalHelpers.alea.Next(0, 7) - 3;
            }

            airspace.Render();
        }

        // Calcul du nouvel état après que 'interval' millisecondes se sont écoulées
        private void Update(int interval)
        {
            
            ship.Update(interval);
            
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
                    if (ship.Y > 1)
                    {
                        ship.Y -= 15;
                    }
                    break;
                case Keys.A:
                    if (ship.X > 1)
                    {
                        ship.X -= 15;
                    }
                    break;
                case Keys.S:
                    if (ship.Y + 77 < HEIGHT)
                    {
                        ship.Y += 15;
                    }
                    break;
                case Keys.D:
                    if (ship.X + 110 < WIDTH)
                    {
                        ship.X += 15;
                    }
                    break;
            }
        }
    }
}