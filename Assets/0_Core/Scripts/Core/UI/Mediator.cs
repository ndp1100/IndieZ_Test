using System;
using Game.UI.Hud;

namespace Game.Core.UI
{
    public abstract class Mediator
    {
        public abstract Type ViewType { get; }

        public abstract void Mediate(object view);
        public abstract void Unmediate();

        public abstract void InternalShow();
        public abstract void InternalHide();
    }

    public abstract class Mediator<T> : Mediator where T : IHud
    {
        private bool _isShowed;
        protected T _view;

        public override Type ViewType => typeof(T);
        public T View => _view;

        public sealed override void Mediate(object view)
        {
            _view = (T) view;
            _isShowed = false;
        }

        public sealed override void Unmediate()
        {
            if (_isShowed)
            {
                Hide();
            }
            _view = default(T);
        }

        public sealed override void InternalShow()
        {
            _view.IsActive = true;
            Show();
        }

        public sealed override void InternalHide()
        {
            _view.IsActive = false;
            Hide();
        }

        protected abstract void Show();
        protected abstract void Hide();
    }
}