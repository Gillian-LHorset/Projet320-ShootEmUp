using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble.View
{
    internal class HealthBar
    {
        public static void DisplayHealthBar(BufferedGraphics airspace, int x, int y, int WIDTH, int entityHealthPoint, int maxHealPoint)
        {
            // défini si l'entité est encore en vie
            bool isInLife;
            // la taille de la barre de vie dépend de la taille de l'entité fournie
            int barWidth = WIDTH;
            // hauteur de la bar de vie
            int barHeight = 5;

            // va remonter un peu la barre pour éviter qu'elle soit sur le modele de l'entité
            y -= 10;

            // couleur de l'interieur de la bar
            Brush healthBarBrush = new SolidBrush(Color.Red);
            // couleur du tour de la barre de vie
            Pen aroundHealthBarPen = new Pen(Color.Black);
            // couleur dessour la couleur rouge de la bar de vie
            Brush decressesHealthBarBrush = new SolidBrush(Color.Gray);


            airspace.Graphics.FillRectangle(decressesHealthBarBrush, new Rectangle(x, y, barWidth, barHeight));

            // zone rouge indiquant la vie du joueur
            // sa taille est relavite a healPoint du joueur pour donner une indication de manière graphique
            airspace.Graphics.FillRectangle(healthBarBrush, new Rectangle(x, y, (barWidth / maxHealPoint) * entityHealthPoint, barHeight));
            // affiche le tour de la healbar
            airspace.Graphics.DrawRectangle(aroundHealthBarPen, new Rectangle(x, y, barWidth, barHeight));
            if (entityHealthPoint < 1)
            // si les points de vie de l'entité sont plus petit que 1
            {
                // l'entité n'est plus en vie
                isInLife = false;
            }
        }
    }
}
