using Scramble.Properties;
using Scramble.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class SniperEnemy : Enemy
    {
        Brush ShootBrush = new SolidBrush(Color.Red);
        public override void Render(BufferedGraphics drawingSpace)
        {
            drawingSpace.Graphics.DrawImage(Resources.SniperEnemy, X, Y, width, height);

            // affiche la bar de vie au dessus de lui
            HealthBar.DisplayHealthBar(drawingSpace, X, Y, width, HealPoint, HEALPOINTMAX);

            // affiche le tir de l'ennemie sur AirSpace en se basant sut le rectangle créer lors du tir
            drawingSpace.Graphics.FillRectangle(ShootBrush, SniperShoot);            
        }
    }
}
