using System;

namespace Game.Core
{
    public abstract class State : IDisposable
    {
        public abstract void Initialize();
        public abstract void Dispose();
    }
}