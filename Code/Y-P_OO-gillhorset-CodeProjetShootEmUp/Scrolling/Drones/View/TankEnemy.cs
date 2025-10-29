using Scramble.Properties;
using Scramble.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class TankEnemy : Enemy
    {
        public override void Render(BufferedGraphics drawingSpace)
        {
            drawingSpace.Graphics.DrawImage(Resources.TankEnemy, X, Y, width, height);
            // affiche la bar de vie de l'ennemie au dessus de lui
            HealthBar.DisplayHealthBar(drawingSpace, X, Y, width, HealPoint, HEALPOINTMAX);
        }
    }
}
