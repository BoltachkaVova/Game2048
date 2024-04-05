using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game2048
{
    public class Pool
    {
        private readonly Transform _container;
        private readonly Element _elementPrefabs;

        private List<Element> _pool = new List<Element>(124);
    
        public Pool(Transform container, Element elementPrefabs)
        {
            _container = container;
            _elementPrefabs = elementPrefabs;
        }

        public void Initialise(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var element = Object.Instantiate(_elementPrefabs, _container);
                element.gameObject.SetActive(false);
                _pool.Add(element);
            }
        }

        public bool TryGetElement(out Element element)
        {
            element = _pool.FirstOrDefault(o => !o.IsActive);

            if (element == null)
            {
                element  = Object.Instantiate(_elementPrefabs, _container);
                element.gameObject.SetActive(false);
                _pool.Add(element);
            }

            return element != null;
        }
    }
}