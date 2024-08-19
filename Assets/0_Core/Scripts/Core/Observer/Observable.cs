using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Core
{
    public interface IObservable
    {
        void SetChanged();
        void AddObserver(IObserver observer);
        void RemoveObserver(IObserver observer);
    }

    [Serializable]
    public abstract class Observable : IObservable
    {
        [NonSerialized]
        private readonly List<IObserver> _observers;
        private int _count;

        protected Observable()
        {
            _observers = new List<IObserver>();
        }

        [XmlIgnore]
        [JsonIgnore]
        public bool IsChanged
        {
            get;
            private set;
        }

        public void SetChanged()
        {
            IsChanged = true;

            if (_count == 0)
                return;

            int length = _observers.Count;
            for (int i = 0; i < Math.Min(length, _observers.Count); i++)
            {
                var current = _observers[i];
                if (current != null)
                {
                    current.OnObjectChanged(this);
                }
            }

            if (_count == _observers.Count)
                return;

            for (int i = _observers.Count - 1; i >= 0; i--)
            {
                if (null == _observers[i])
                {
                    _observers.RemoveAt(i);
                }
            }
        }

        public void Commit()
        {
            IsChanged = false;
        }

        public void SetChangedAndCommit()
        {
            SetChanged();
            Commit();
        }

        public void AddObserver(IObserver observer)
        {
            var index = _observers.IndexOf(observer);
            if (index == -1)
            {
                _observers.Add(observer);
                _count++;
                OnObserversChanged(_count);
            }
            else
            {
                if (_count == 1) return;
                _observers[index] = null;
                _observers.Add(observer);
            }
        }

        public void RemoveObserver(IObserver observer)
        {
            var index = _observers.IndexOf(observer);
            if (index != -1)
            {
                _observers[index] = null;
                _count--;
                OnObserversChanged(_count);
            }
        }

        public void Clear()
        {
            _observers.Clear();
            _count = 0;
            OnObserversChanged(_count);
        }

        protected virtual void OnObserversChanged(int count)
        {            
        }
    }       
}