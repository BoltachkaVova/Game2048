using System;
using UnityEngine;

namespace Game2048
{
    public class InputController : MonoBehaviour
    {
        private BoardController _boardController;
        
        private Vector2 _downPosition;
        private Vector2 _upPosition;
        
        private bool _isActive = true;
        private float _minDistance = 20f;
        
        public event Action<TypeSwipe> Swipe;

        public void Construct(BoardController boardController)
        {
            _boardController = boardController;
            _boardController.FailGame += OnFailGame;
        }

        private void Update()
        {
            if(!_isActive) return;
            
            if (Input.GetMouseButtonDown(0))
            {
                _downPosition = Input.mousePosition;
                _upPosition = Input.mousePosition;
            }

            if (!Input.GetMouseButtonUp(0)) return;
            
            _upPosition = Input.mousePosition;
            DetectSwipe();
        }

        private void OnDestroy()
        {
            _boardController.FailGame -= OnFailGame;
        }

        private void DetectSwipe()
        {
            if (!IsCheckSwipe()) return;
            var type = TypeSwipe.None;

            if (IsHorizontalSwipe())
            {
                type = _downPosition.x - _upPosition.x > 0 ? TypeSwipe.Left : TypeSwipe.Right;
            }
            else
            {
                type = _downPosition.y - _upPosition.y > 0 ? TypeSwipe.Down : TypeSwipe.Up;
            }
            
            Swipe?.Invoke(type);
            _downPosition = _upPosition;
        }

        private bool IsCheckSwipe()
        {
            return Vector3.Distance(_downPosition, _upPosition) > _minDistance;
        }

        private bool IsHorizontalSwipe()
        {
            return Mathf.Abs(_downPosition.x - _upPosition.x) > Mathf.Abs(_downPosition.y - _upPosition.y);
        }
        
        
        private void OnFailGame()
        {
            _isActive = false;
        }
    }
}