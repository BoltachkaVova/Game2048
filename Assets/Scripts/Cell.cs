using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game2048
{
    public class Cell
    {
        private readonly Vector2 _position;
        
        private bool _isFree = true;
        private Element _element;

        public bool IsFree => _isFree;
        public Vector2 Position => _position;
        public Element Element => _element;

        public Cell(Vector2 position)
        {
            _position = position;
        }

        public void SetElement(Element element, int data)
        {
            _isFree = false;
            
            _element = element;
            _element.Open(true);
            _element.Initilize(data, _position);
        }

        public async UniTask ChangeElement(Element element)
        {
            _isFree = false;
            
            _element = element;
            await _element.MoveTo(_position);
        }

        public async UniTask UpgradeElement(Element element)
        {
            _isFree = false;
            
            await element.MoveTo(_position);
            element.Open(false);
            
            _element.Upgrade().Forget();
        }

        public void RemoveElement()
        {
            _isFree = true;
            _element = null;
        }
    }
}