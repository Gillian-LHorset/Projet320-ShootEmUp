using Scramble.Properties;

namespace Scramble
{

    // display shoot
    public partial class Shoot
    {

        public virtual void Render(BufferedGraphics drawingSpace, List<Shoot> shoots)
        {
            foreach (var shoot in shoots.ToList())
            {
                if (shoot._x < 0 || shoot._x > AirSpace.WIDTH)
                {
                    shoots.Remove(shoot);
                }
                drawingSpace.Graphics.DrawImage(Resources.playerShoot, _x, _y, WIDTH, HEIGHT);
            }
        }

    }
}
