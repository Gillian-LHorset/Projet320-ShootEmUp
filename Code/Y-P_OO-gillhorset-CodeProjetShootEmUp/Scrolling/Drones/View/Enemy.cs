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
        /// <summary>
        /// Affiche l'ennemie sur AirSpace, est redéfinie par les classes enfant
        /// </summary>
        /// <param name="drawingSpace"></param>
        public virtual void Render(BufferedGraphics drawingSpace)
        {
                drawingSpace.Graphics.DrawImage(Resources.ship, X, Y, width, height);
        }

    }
}
