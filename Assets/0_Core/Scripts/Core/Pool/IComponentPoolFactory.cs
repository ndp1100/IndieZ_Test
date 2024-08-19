using UnityEngine;

namespace Game.UI.Pool
{
    public interface IComponentPoolFactory
    {
        Transform Content { get; }

        int CountInstances { get; }

        T Get<T>() where T : Component;

        T Get<T>(int sublingIndex) where T : Component;

        void Release<T>(T component) where T : Component;

        void ReleaseAllInstances();

        void Dispose();
    }
}
