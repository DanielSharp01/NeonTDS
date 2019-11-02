using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeonTDS
{
    public interface IParticleEffect
    {
        void Spawn(EntityManager entityManager);
    }
}
