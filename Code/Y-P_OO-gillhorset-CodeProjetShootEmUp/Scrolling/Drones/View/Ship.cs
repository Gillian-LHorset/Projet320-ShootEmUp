using Scramble.Helpers;
using Scramble.Properties;
using Scramble.View;

namespace Scramble
{
    // Cette partie de la classe Ship définit comment on peut voir un vaisseau

    public partial class Ship
    {
        public static readonly int WIDTH = 100;
        public static readonly int HEIGHT = 40;

        // variable qui a pour but de permetre au ship de s'afficher une fois sur deux lors qu'il est touché
        int countInCollision = 0;

        public void Render(BufferedGraphics drawingSpace)
        {
            if (!PlayerCanBeHit())
            // si le joueur ne peut pas être touché
            {
                countInCollision++;
                if (countInCollision % 2 == 0)
                // l'affiche du joueur se fera un fois sur 2
                {
                    drawingSpace.Graphics.DrawImage(Resources.playerShip, X, Y, WIDTH, HEIGHT);
                }
            } else
            {
                drawingSpace.Graphics.DrawImage(Resources.playerShip, X, Y, WIDTH, HEIGHT);
            }

            // affiche la bar de vie du joueur
            HealthBar.DisplayHealthBar(drawingSpace, _x, _y, WIDTH, HealthPoint, MaxHealthPoint);
        }
    }
}
