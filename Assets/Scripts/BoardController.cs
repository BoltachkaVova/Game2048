using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game2048
{
    public class BoardController
    {
        private readonly InputController _inputController;
        private readonly Pool _pool;
        private readonly RectTransform _board;
        private readonly SettingsConfig _config;

        private Cell[,] _cells;
        
        private int _column;
        private int _row;
        private int _elementData = 2;

        private bool _isDebug = false;
        
        public event Action FailGame;
        
        public BoardController(InputController inputController, Pool pool, RectTransform board, SettingsConfig config)
        {
            _inputController = inputController;
            _pool = pool;
            _board = board;
            _config = config;
        }

        public void Initialise()
        {
            _inputController.Swipe += OnSwiped;
            
            SetSettingsBoard();
            FillingBoard();
        }
        
        public void Dispose()
        {
            _inputController.Swipe -= OnSwiped;
        }

        private void SetSettingsBoard()
        {
            _column = (int)_config.SizeBoard.x;
            _row = (int)_config.SizeBoard.y;
            _cells = new Cell[_column, _row];
        }

        private async void OnSwiped(TypeSwipe typeSwipe)
        {
            if (typeSwipe == TypeSwipe.Left || typeSwipe == TypeSwipe.Right)
            {
                await CheckBoardHorizontal(typeSwipe);
            }
            else
            {
                await CheckBoardVertical(typeSwipe);
            }

            SetElements(_config.CountSetElementsInOneSwipe);

            if (_isDebug)
            {
                DebugNotFree();
            }
        }
        
        private void FillingBoard()
        {
            var offsetVector2 = new Vector2(_config.SizeBoard.x * _config.OffsetBetweenCells, 
                _config.SizeBoard.y * _config.OffsetBetweenCells);
            var size = _config.SizeBoard * _config.SizeElement + offsetVector2;
            _board.sizeDelta = size;
            
            var offset = _config.OffsetBetweenCells;
            var startPosStep = _config.SizeElement * 0.5f + offset;
            var stepSize = _config.SizeElement + offset;
            
            var startPosition = new Vector2(size.x * -0.5f + startPosStep, size.y * -0.5f + startPosStep);

            for (var x = 0; x < _column; x++)
            {
                for (var y = 0; y < _row; y++)
                {
                    var position = startPosition + new Vector2(stepSize * x, stepSize * y);
                    var cell = new Cell(position);
                    
                    _cells[x, y] = cell;
                }
            }
            
            SetElements(_elementData); 
        }
        
        private void SetElements(int count)
        {
            while (count != 0)
            {
                var xRange = Random.Range(0, _column);
                var yRange = Random.Range(0, _row);
                
                if (!_cells[xRange, yRange].IsFree)
                {
                    if (!IsPlay())
                    {
                        FailGame?.Invoke();
                        return;
                    }
                    continue;
                }

                if (_pool.TryGetElement(out var element))
                {
                    _cells[xRange, yRange].SetElement(element, _elementData);
                }
                
                --count;
            }
        }
        
        private bool IsPlay()
        {
            for (var i = 0; i < _column; i++)
            {
                for (var j = 0; j < _row; j++)
                {
                    if (_cells[i, j].IsFree)
                        return true;
                }
            }
            return false;
        }

        private async UniTask CheckBoardHorizontal(TypeSwipe typeSwipe)
        {
            var tasks = new List<UniTask>(8);
            
            for (var i = 0; i < _row; i++)
            {
                var horizontalLine = GetHorizontalLine(i, typeSwipe);
                var countFree = horizontalLine.Where(o => o.IsFree).ToList().Count;
                
                if (countFree == _column) continue; // all cells free
                Cell prefNoFreeCell = null;
                
                for (var cell = horizontalLine.First; cell != null; cell = cell.Next)
                {
                    if (cell.Value.IsFree) continue;
                    
                    var element = cell.Value.Element;
                    cell.Value.RemoveElement(); 
                    
                    if (prefNoFreeCell != null && prefNoFreeCell.Element.Level == element.Level)
                    {
                        tasks.Add(prefNoFreeCell.UpgradeElement(element));
                        prefNoFreeCell = null;
                        continue;
                    }
                    
                    var firstFreeCell = horizontalLine.FirstOrDefault(o => o.IsFree);
                    if (firstFreeCell != null)
                    {
                        tasks.Add(firstFreeCell.ChangeElement(element));
                        prefNoFreeCell = firstFreeCell;
                    }
                }
            }
            
            await UniTask.WhenAll(tasks);
        }
        
        private async UniTask CheckBoardVertical(TypeSwipe typeSwipe)
        {
            var tasks = new List<UniTask>(8);
            
            for (var i = 0; i < _column; i++)
            {
                var verticalLine = GetVerticalLine(i, typeSwipe);
                var countFree = verticalLine.Where(o => o.IsFree).ToList().Count;
                
                if (countFree == _row) continue; 
                Cell prefNoFreeCell = null;

                for (var cell = verticalLine.Last; cell != null; cell = cell.Previous)
                {
                    if(cell.Value.IsFree) continue;
                    var element = cell.Value.Element;

                    cell.Value.RemoveElement();
                    
                    if (prefNoFreeCell != null && prefNoFreeCell.Element.Level == element.Level)
                    {
                        tasks.Add(prefNoFreeCell.UpgradeElement(element));
                        prefNoFreeCell = null;
                        continue;
                    }
                    
                    var lastFreeCell = verticalLine.LastOrDefault(o => o.IsFree);
                    if(lastFreeCell != null)
                    {
                        tasks.Add(lastFreeCell.ChangeElement(element));
                        prefNoFreeCell = lastFreeCell;
                    }
                }
            }
            await UniTask.WhenAll(tasks);
        }
        
        
        private LinkedList<Cell> GetHorizontalLine(int y, TypeSwipe typeSwipe)
        {
            var line = new LinkedList<Cell>();
            for (var i = 0; i < _column; i++)
            {
                var cell = _cells[i, y];

                if (typeSwipe == TypeSwipe.Right)
                {
                    line.AddFirst(cell);
                }
                else
                {
                    line.AddLast(cell);
                }
            }
            return line;
        }
        
        private LinkedList<Cell> GetVerticalLine(int x, TypeSwipe typeSwipe)
        {
            var line = new LinkedList<Cell>();
            for (var i = 0; i < _row; i++)
            {
                var cell = _cells[x, i];
                if (typeSwipe == TypeSwipe.Down)
                {
                    line.AddFirst(cell);
                }
                else
                {
                    line.AddLast(cell);
                }
            }
            return line;
        }

        
        private void DebugNotFree()
        {
            for (var x = 0; x < _column; x++)
            {
                for (var y = 0; y < _row; y++)
                {
                    if(_cells[x, y].IsFree) continue;
                    Debug.Log($"{_cells[x, y].IsFree} in {_cells[x, y].Position}");
                }
            }
        }
    }
}