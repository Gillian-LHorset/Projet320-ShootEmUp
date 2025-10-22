using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble.View
{
    internal class HealBar
    {
        public static void DisplayHealBar(BufferedGraphics airspace, int x, int y, int WIDTH, int entityHealPoint, int maxHealPoint)
        {
            bool isInLife;
            int barWidth = WIDTH;
            int barHeight = 5;

            // va remonter un peu la barre pour éviter qu'elle soit sur le modele de l'entité
            y -= 10;

            Brush healBarBrush = new SolidBrush(Color.Red);
            Pen aroundHealBarPen = new Pen(Color.Black);
            Brush decressesHealBarBrush = new SolidBrush(Color.Gray);

            airspace.Graphics.FillRectangle(decressesHealBarBrush, new Rectangle(x, y, barWidth, barHeight));

            // zone rouge indiquant la vie du joueur
            // sa taille est relavite a healPoint du joueur pour donner une indication de manière graphique
            airspace.Graphics.FillRectangle(healBarBrush, new Rectangle(x, y, (barWidth / maxHealPoint) * entityHealPoint, barHeight));
            airspace.Graphics.DrawRectangle(aroundHealBarPen, new Rectangle(x, y, barWidth, barHeight));
            if (entityHealPoint < 1)
            {
                isInLife = false;
            }
        }
    }
}
