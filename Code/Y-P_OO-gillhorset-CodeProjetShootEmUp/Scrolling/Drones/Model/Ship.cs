namespace Scramble
{
    // Cette partie de la classe ship définit ce qu'est un ship par un modèle numérique
    public partial class Ship
    {
        public static readonly int FULLTANK = 1000;   // Charge maximale de la batterie
        private int _tanklevel;                            // La charge actuelle de la batterie
        private string _name;                           // Un nom
        private int _x;                                 // Position en X depuis la gauche de l'espace aérien
        private int _y;                                 // Position en Y depuis le haut de l'espace aérien

        // Constructeur
        public Ship(int x, int y, string name)
        {
            _x = x;
            _y = y;
            _name = name;
        }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y;} set { _y = value; } }
        public string Name { get { return _name;} }

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

        public bool test = false;

        public int[] ShipGround = new int[AirSpace.WIDTH / 10 + 1];

        public void MoveShip()
        {

            if (_goUpBool)
            {
                if (_y > 10) {
                    _y -= 15;
                }
            }
            if (_goLeftBool)
            {
                if (_x > 10)
                {
                    _x -= 15;
                }

            }
            if (_goDownBool)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (Ship.HEIGHT + _y + 30 < AirSpace.HEIGHT - ShipGround[_x / 10])  // position x du vaisseau entre crochet
                                                                             // si la hauteur de la position du vaisseau et sa taille sont superieur à
                                                                             // la hauteur de l'écran moins la hauteur du sol
                    {
                        test = true;
                    }
                    else { test = false; break; }

                }
                if (test && _y < AirSpace.HEIGHT - Ship.HEIGHT)
                {
                    _y += 15;
                }

            }
            if (_goRightBool)
            {
                if (_x < AirSpace.WIDTH - Ship.WIDTH) {
                    _x += 15;
                }
            }

        }
        public void Update(int interval)
        {
                                         // Il a dépensé de l'énergie
        }


    }
}