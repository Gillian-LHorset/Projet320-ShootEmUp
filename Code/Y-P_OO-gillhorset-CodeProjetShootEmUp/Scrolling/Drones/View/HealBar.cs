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
            // défini si l'entité est encore en vie
            bool isInLife;
            // la taille de la barre de vie dépend de la taille de l'entité fournie
            int barWidth = WIDTH;
            // hauteur de la bar de vie
            int barHeight = 5;

            // va remonter un peu la barre pour éviter qu'elle soit sur le modele de l'entité
            y -= 10;

            // couleur de l'interieur de la bar
            Brush healBarBrush = new SolidBrush(Color.Red);
            // couleur du tour de la barre de vie
            Pen aroundHealBarPen = new Pen(Color.Black);
            // couleur dessour la couleur rouge de la bar de vie
            Brush decressesHealBarBrush = new SolidBrush(Color.Gray);


            airspace.Graphics.FillRectangle(decressesHealBarBrush, new Rectangle(x, y, barWidth, barHeight));

            // zone rouge indiquant la vie du joueur
            // sa taille est relavite a healPoint du joueur pour donner une indication de manière graphique
            airspace.Graphics.FillRectangle(healBarBrush, new Rectangle(x, y, (barWidth / maxHealPoint) * entityHealPoint, barHeight));
            // affiche le tour de la healbar
            airspace.Graphics.DrawRectangle(aroundHealBarPen, new Rectangle(x, y, barWidth, barHeight));
            if (entityHealPoint < 1)
            // si les points de vie de l'entité sont plus petit que 1
            {
                // l'entité n'est plus en vie
                isInLife = false;
            }
        }
    }
}
