using System;
using UnityEngine;

namespace Game2048
{
    public class UIController : MonoBehaviour
    {
        private BoardController _boardController;
        
        public void Construct(BoardController boardController)
        {
            _boardController = boardController;
            _boardController.FailGame += OnFailGame;
        }


        private void OnDestroy()
        {
            _boardController.FailGame -= OnFailGame;
        }

        private void OnFailGame()
        {
            Debug.Log("Faill");
        }
    }
}