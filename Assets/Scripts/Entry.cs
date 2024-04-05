using UnityEngine;

namespace Game2048
{
    public class Entry : MonoBehaviour
    {
        [SerializeField] private InputController inputController;
        [SerializeField] private Element elementPrefab;
        [SerializeField] private RectTransform board;
        [SerializeField] private UIController uiController;
        [SerializeField] private SettingsConfig config;
        
        private Pool _pool;
        private BoardController _boardController;
        
        private void Start()
        {
            InitPool();
            
            _boardController = new BoardController(inputController, _pool, board, config);
            _boardController.Initialise();
            
            uiController.Construct(_boardController);
            inputController.Construct(_boardController);
        }

        private void OnDestroy()
        {
            _boardController.Dispose();
        }

        private void InitPool()
        {
            _pool = new Pool(board, elementPrefab);
            var count = (int)(config.SizeBoard.x * config.SizeBoard.y);
            _pool.Initialise(count);
        }
    }
}