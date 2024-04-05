using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048
{
    public class Element : MonoBehaviour
    {
        [Header("Colors")]
        [SerializeField] private List<Color> levelColors;
        [SerializeField] private Image image;
        
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI valueText;
       
        [Header("Animation")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float durationScale = 0.4f;
        [SerializeField] private float durationMove = 0.4f;
        [SerializeField] private float durationColor = 0.4f;

        private int _value = 0;
        private int _level = 0;
        private bool _isActive = false;
        public bool IsActive => _isActive;
        
        public int Level => _level;

        public void Initilize(int value, Vector2 position)
        {
            _value = value;
            _level = 0;
            
            valueText.text = _value.ToString();
            rectTransform.localScale = Vector3.zero;
            image.color = levelColors[_level];
            transform.localPosition = position;
        }
        
        public void Open(bool isOn)
        {
            _isActive = isOn;
            gameObject.SetActive(isOn);
            
            rectTransform.DOScale(isOn ? Vector3.one : Vector3.zero, durationScale).SetEase(Ease.Linear);
        }

        public async UniTaskVoid Upgrade()
        {
            _value += _value;
            ++_level;
            
            await image.DOColor(levelColors[_level], durationColor).SetEase(Ease.Linear);
            valueText.text = _value.ToString();
        }
        
        public async UniTask MoveTo(Vector2 newPos)
        {
            await rectTransform.DOLocalMove(newPos, durationMove).SetEase(Ease.Linear);
        }
    }
}