using Scramble.Helpers;
using Scramble.Properties;

namespace Scramble
{
    // Cette partie de la classe Ship définit comment on peut voir un vaisseau

    public partial class Ship
    {
        public static readonly int WIDTH = 100;
        public static readonly int HEIGHT = 40;
        // De manière graphique
        public void Render(BufferedGraphics drawingSpace)
        {
            MoveShip();
            drawingSpace.Graphics.DrawImage(Resources.ship, X,Y,WIDTH, HEIGHT);
            drawingSpace.Graphics.DrawString($"{this}", TextHelpers.drawFont, TextHelpers.writingBrush, X + 5, Y - 25);
        }

        // De manière textuelle
        public override string ToString()
        {
            return $"{Name} ({((int)((double)_tanklevel / FULLTANK * 100)).ToString()}%)";
        }

    }
}
