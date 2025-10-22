using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Scramble
{
    public partial class BasicEnemy : Enemy
    {
        readonly int healPointMax = 3;
        int healPoint = 3;
            public BasicEnemy(int x, int y) : base(x, y)
        {
                    
        }

    }
}
