using System;
using System.Collections.Generic;
using System.Linq;
using Game.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI.Pool
{

    [Serializable]
    public class MultiplePrefabPool
    {
        public GameObject Prefab;
        public int InitCount;
    }


    public class ComponentPoolFactory : MonoBehaviour, IComponentPoolFactory
    {
        [SerializeField] private bool _isMultiplePrefab;
        [SerializeField, HideIf("_isMultiplePrefab")] private GameObject _prefab;
        [SerializeField, HideIf("_isMultiplePrefab")] private int _count;
        [SerializeField, ShowIf("_isMultiplePrefab")] private List<MultiplePrefabPool> _prefabs;
        [SerializeField] private Transform _content;
        [SerializeField] private Transform _poolStorage;

        private readonly HashSet<GameObject> _instances;
        private Queue<GameObject> _pool;

        public Transform Content
        {
            get { return _content; }
        }

        public List<MultiplePrefabPool> Prefabs
        {
            get { return _prefabs; }
        }

        public void AddPrefab(GameObject prefab, int initCount)
        {
            _prefabs.Add(new MultiplePrefabPool {Prefab = prefab, InitCount = initCount});
        }

        public ComponentPoolFactory()
        {
            _instances = new HashSet<GameObject>();
            _pool = new Queue<GameObject>();
        }

        public void SetPrefab(GameObject prefab)
        {
            _prefab = prefab;
        }

        public void SetCount(int count)
        {
            _count = count;
        }

        public void Setup(GameObject prefab, int count, Transform content, Transform poolStorage)
        {
            _prefab = prefab;
            _count = count;
            _content = content;
            _poolStorage = poolStorage;
        }

        public int CountInstances
        {
            get { return _instances.Count; }
        }

        private void Awake()
        {
            if (_instances.Count > 0)
                return;

            if (_isMultiplePrefab == false)
            {
                for (int i = 0; i < _count; i++)
                {
                    Get<Transform>();
                }
            }
            else
            {
                List<int> randomIndexes = new List<int>();
                for (int i = 0; i < _prefabs.Count; i++)
                {
                    randomIndexes.Add(i);
                }

                randomIndexes.Shuffle();
                for (int i = 0; i < randomIndexes.Count; i++)
                {
                    var prefab = _prefabs[randomIndexes[i]];
                    GameObject result = Instantiate(prefab.Prefab);
                    if (result == null)
                        continue;

                    _pool.Enqueue(result);
                    var t = result.transform;
                    t.SetParent(_content, false);
                    result.SetActive(false);
                }


                // foreach (var prefab in _prefabs)
                // {
                //     for (int i = 0; i < prefab.InitCount; i++)
                //     {
                //         GameObject result = Instantiate(prefab.Prefab);
                //         if(result == null)
                //             continue;
                //
                //         _pool.Enqueue(result);
                //         var t = result.transform;
                //         t.SetParent(_content, false);
                //         result.SetActive(false);
                //     }
                // }
            }

            ReleaseAllInstances();
        }

        public T Get<T>() where T : Component
        {
            return Get<T>(_instances.Count);
        }

        public T Get<T>(int sublingIndex) where T : Component
        {
            bool isNewInstance = false;
            if (_pool.Count == 0)
            {
                if (_isMultiplePrefab == false)
                {
                    GameObject result = Instantiate(_prefab);
                    YOLogger.LogTemporaryChannel($"Pool", $"Create instance of {_prefab.name}");

                    if (null == result)
                        return null;

                    _pool.Enqueue(result);
                    isNewInstance = true;
                }
                else
                {
                    int randomIndex = UnityEngine.Random.Range(0, _prefabs.Count);
                    GameObject result = Instantiate(_prefabs[randomIndex].Prefab);
                    YOLogger.LogTemporaryChannel($"Pool", $"Create instance of {_prefabs[randomIndex].Prefab.name}");
                    if (null == result) return null;
                    _pool.Enqueue(result);
                    isNewInstance = true;
                }
            }

            T resultComponent = _pool.Dequeue().GetComponent<T>();
            if (null == resultComponent)
            {
                return resultComponent;
            }

            var go = resultComponent.gameObject;
            var t = resultComponent.transform;
            if (isNewInstance || (_poolStorage != null && _poolStorage != _content))
            {
                t.SetParent(_content, false);
            }

            _instances.Add(go);

            if (!go.activeSelf)
            {
                go.SetActive(true);
            }

            if (t.GetSiblingIndex() != sublingIndex)
            {
                t.SetSiblingIndex(sublingIndex);
            }

            return resultComponent;
        }

        public void Release<T>(T component) where T : Component
        {
            var go = component.gameObject;
            if (_instances.Contains(go))
            {
                go.SetActive(false);
                if (_poolStorage)
                {
                    go.transform.SetParent(_poolStorage, false);
                }

                _pool.Enqueue(go);
                _instances.Remove(go);
            }
        }

        public void ReleaseAllInstances()
        {
            foreach (GameObject instance in _instances)
            {
                instance.SetActive(false);
                if (_poolStorage)
                {
                    instance.transform.SetParent(_poolStorage, false);
                }

                _pool.Enqueue(instance);
            }

            _instances.Clear();
        }

        public void PutInstancesToPool()
        {
            _pool = new Queue<GameObject>(_instances.Union(_pool));
            _instances.Clear();
        }

        public void HideUnusedInstances()
        {
            foreach (GameObject instance in _pool)
            {
                instance.SetActive(false);
            }
        }

        public void Dispose()
        {
            ReleaseAllInstances();

            foreach (GameObject gameObject in _pool)
            {
                GameObject.Destroy(gameObject);
            }

            _pool.Clear();
        }
    }
}