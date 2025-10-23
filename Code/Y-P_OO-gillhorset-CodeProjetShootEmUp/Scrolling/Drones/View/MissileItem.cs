using Scramble.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class MissileItem
    {
        public void Render(BufferedGraphics drawingSpace)
        {

            drawingSpace.Graphics.DrawImage(Resources.MissileItem, _x, _y, WIDTH, HEIGHT);

        }
    }
}
