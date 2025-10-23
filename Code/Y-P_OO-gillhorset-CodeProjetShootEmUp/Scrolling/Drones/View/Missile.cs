using Scramble.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class Missile : Shoot
    {
        public override void Render(BufferedGraphics drawingSpace, List<Shoot> shoots)
        {
            
            drawingSpace.Graphics.DrawImage(Resources.Missile, _x, _y, WIDTH, HEIGHT);
            
        }
    }
}
