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
        public bool goLeft { get { return goLeftBool; } set { goLeftBool = value; } }
        public bool goDown { get { return goDownBool; } set { goDownBool = value; } }
        public bool goRight { get { return goRightBool; } set { goRightBool = value; } }
        
        private bool _goUpBool;
        private bool goLeftBool;
        private bool goDownBool;
        private bool goRightBool;

        public void MoveShip()
        {

            if (_goUpBool)
            {
                if (_y > 10) {
                    _y -= 15;
                }
            }
            if (goLeftBool)
            {
                if (_x > 10)
                {
                    _x -= 15;
                }

            }
            if (goDownBool)
            {
                if (_y < AirSpace.HEIGHT - Ship.HEIGHT)
                {
                    _y += 15;
                }
            }
            if (goRightBool)
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
