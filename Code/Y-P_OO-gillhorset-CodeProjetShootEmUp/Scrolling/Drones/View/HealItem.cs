using Scramble.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class HealItem
    {
        ////////////////////Brush testBrush = new SolidBrush(Color.Red);
        public void Render(BufferedGraphics drawingSpace)
        {
            drawingSpace.Graphics.DrawImage(Resources.HealItem, _x, _y, WIDTH, HEIGHT);
            ////////////////////drawingSpace.Graphics.FillRectangle(testBrush, healItemRectCollision);
        }
    }
}
