using Scramble.Properties;

namespace Scramble
{
    // display shoot
    public partial class Shoot
    {
        public virtual void Render(BufferedGraphics drawingSpace, List<Shoot> shoots)
        {
            foreach (Shoot shoot in shoots.ToList())
            // pour chaque objet de la liste shoots
            {
                if (shoot._x < 0 || shoot._x > AirSpace.WIDTH)
                // si le shoot est en dehors de AirSpace
                {
                    // supprime le tir 
                    shoots.Remove(shoot);
                }
                drawingSpace.Graphics.DrawImage(Resources.playerShoot, _x, _y, _WIDTH, _HEIGHT);
            }
        }

    }
}
