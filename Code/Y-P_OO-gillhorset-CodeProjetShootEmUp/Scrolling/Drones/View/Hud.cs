using Scramble.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    internal class Hud
    {
        public static void Render(BufferedGraphics drawingSpace, Ship ship)
        {
            // défini la couleur du texte
            Brush fontBrush = new SolidBrush(Color.Black);

            drawingSpace.Graphics.DrawImage(Resources.Missile, 10, 10, 22, 13);
            // affiche le nombre de missile en reserve
            drawingSpace.Graphics.DrawString(($"X {ship.ReserveMissile}"), new Font("Arial", 13), fontBrush, new PointF(35, 7));


            drawingSpace.Graphics.DrawString(($"Score : {AirSpace.ScorePoint}"), new Font("Arial", 13), fontBrush, new PointF(90, 7));
        }
    }
}
