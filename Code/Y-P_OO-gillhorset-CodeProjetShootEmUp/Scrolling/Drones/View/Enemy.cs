using Scramble.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class Enemy
    {
        public void Render(BufferedGraphics drawingSpace)
        {
            try
            {
                drawingSpace.Graphics.DrawImage(Resources.BasicEnemy, _x, _y, WIDTH, HEIGHT);

            }
            catch (Exception ex) { 
            Console.WriteLine("L'erreur est : ",ex.Message);
            }
        }
    }
}
