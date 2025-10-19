using System;

namespace Ezhtellar.Genesis
{
    public interface IEnemySensor
    {
        event Action<Unit> DidDetectEnemy;

        void SetUnitHost(Unit unit);
    }
}