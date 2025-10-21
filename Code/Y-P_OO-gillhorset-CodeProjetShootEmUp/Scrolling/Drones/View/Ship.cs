using Scramble.Helpers;
using Scramble.Properties;

namespace Scramble
{
    // Cette partie de la classe Ship définit comment on peut voir un vaisseau

    public partial class Ship
    {
        public static readonly int WIDTH = 100;
        public static readonly int HEIGHT = 40;
        int countInCollision = 0;

        public void Render(BufferedGraphics drawingSpace)
        {
            MoveShip();

            if (!PlayerCanBeHit())
            {
                countInCollision++;
                if (countInCollision % 2 == 0)
                {
                    drawingSpace.Graphics.DrawImage(Resources.playerShip, X, Y, WIDTH, HEIGHT);
                }
            } else
            {
                drawingSpace.Graphics.DrawImage(Resources.playerShip, X, Y, WIDTH, HEIGHT);
            }

        }
    }
}
