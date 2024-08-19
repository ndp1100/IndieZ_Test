using System;
using System.Collections.Generic;

namespace Injection
{
    public sealed class Context : IDisposable
    {
        private readonly Dictionary<Type, object> _objectsMap;

        public Context()
        {
            _objectsMap = new Dictionary<Type, object>(100);
            _objectsMap[typeof(Context)] = this;
        }

        public Context(Context parent)
        {
            _objectsMap = new Dictionary<Type, object>(parent._objectsMap);
            _objectsMap[typeof(Context)] = this;
        }

        public void Dispose()
        {
            foreach (var item in _objectsMap)
            {
                if (this == item.Value)
                    continue;

                if (item.Value is IDisposable)
                {
                    (item.Value as IDisposable).Dispose();
                }
            }
            _objectsMap.Clear();
        }

        public void Install(params object[] objects)
        {
            foreach (object obj in objects)
            {
                _objectsMap[obj.GetType()] = obj;
            }
        }

        public void InstallByType(object obj, Type type)
        {
            _objectsMap[type] = obj;
        }

        public void ApplyInstall()
        {
            var injector = Get<Injector>();
            foreach (object obj in _objectsMap.Values)
            {
                injector.Inject(obj);
            }
        }

        public void Uninstall(params object[] objects)
        {
            foreach (object obj in objects)
            {
                _objectsMap.Remove(obj.GetType());
            }
        }

        public T Get<T>() where T : class
        {
#if UNITY_EDITOR
            if (!_objectsMap.ContainsKey(typeof(T)))
            {
                throw new KeyNotFoundException("Not found " + typeof(T));
            }
#endif

            return (T)_objectsMap[typeof(T)];
        }

        public object Get(Type type)
        {
#if UNITY_EDITOR
            if (!_objectsMap.ContainsKey(type))
            {
                throw new KeyNotFoundException("Not found " + type);
            }
#endif
            return _objectsMap[type];
        }
    }
}