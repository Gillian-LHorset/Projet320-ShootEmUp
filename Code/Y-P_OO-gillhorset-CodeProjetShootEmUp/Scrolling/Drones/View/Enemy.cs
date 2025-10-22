using Scramble.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// classe Inutile et devant être détruite a terme
namespace Scramble
{
    public partial class Enemy
    {
        public virtual void Render(BufferedGraphics drawingSpace)
        {
            try
            {
                drawingSpace.Graphics.DrawImage(Resources.ship, X, Y, WIDTH, HEIGHT);

            }
            catch (Exception ex) { 
            Console.WriteLine("L'erreur est : ",ex.Message);
            }
        }

    }
}
