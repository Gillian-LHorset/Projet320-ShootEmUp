using Scramble.Properties;
using Scramble.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class FrontEnemy : Enemy
    {
        public override void Render(BufferedGraphics drawingSpace)
        {
            drawingSpace.Graphics.DrawImage(Resources.FrontEnemy, X, Y, WIDTH, HEIGHT);
            HealBar.DisplayHealBar(drawingSpace, X, Y, WIDTH, healPoint, HEALPOINTMAX);
        }
    }
}
