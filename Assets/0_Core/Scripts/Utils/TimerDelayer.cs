using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utilities
{
    public class TimerDelayer : IDisposable
    {
        private interface IDelayItem
        {
            float ActionTime { get; }
            void SafeInvoke();
        }

        private class DelayItem : IDelayItem
        {
            public float ActionTime { get; private set; }

            private Action _action;

            public DelayItem(float delay, Action action)
            {
                ActionTime = Time.time + delay;
                _action = action;
            }

            public void SafeInvoke()
            {
                if (null != _action)
                {
                    _action.Invoke();
                }
            }
        }

        private class DelayItem<T> : IDelayItem
        {
            public float ActionTime { get; private set; }

            private Action<T> _action;
            private object _argument;

            public DelayItem(float delay, Action<T> action, object argument)
            {
                ActionTime = Time.time + delay;
                _action = action;
                _argument = argument;

            }
            public void SafeInvoke()
            {
                if (null != _action)
                {
                    _action.Invoke((T)_argument);
                }
            }
        }

        private class DelayItem<T0,T1> : IDelayItem
        {
            public float ActionTime { get; private set; }

            private Action<T0, T1> _action;
            private object _argument0;
            private object _argument1;

            public DelayItem(float delay, Action<T0, T1> action, object argument0, object argument1)
            {
                ActionTime = Time.time + delay;
                _action = action;
                _argument0 = argument0;
                _argument1 = argument1;

            }
            public void SafeInvoke()
            {
                if (null != _action)
                {
                    _action.Invoke((T0)_argument0, (T1)_argument1);
                }
            }
        }


        private readonly List<IDelayItem> _items;

        public TimerDelayer()
        {
            _items = new List<IDelayItem>();
        }

        public void Dispose()
        {
            Reset();
        }

        public void Reset() 
        {
            _items.Clear();
        }

        public void DelayAction(float delay, Action action)
        {
            _items.Add(new DelayItem(delay, action));
        }

        public void DelayAction<T>(float delay, Action<T> action, object arg0)
        {
            _items.Add(new DelayItem<T>(delay, action, arg0));
        }
        public void DelayAction<T0, T1>(float delay, Action<T0, T1> action, object arg0, object arg1)
        {
            _items.Add(new DelayItem<T0, T1>(delay, action, arg0, arg1));
        }

        public bool Tick()
        {
            IDelayItem item;

            for (int i = 0; i < _items.Count; i++)
            {
                item = _items[i];
                if (Time.time > item.ActionTime)
                {
                    _items.RemoveAt(i--);
                    item.SafeInvoke();
                }
            }

            return _items.Count > 0;
        }
    }
}