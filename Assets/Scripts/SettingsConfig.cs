using UnityEngine;

namespace Game2048
{
    [CreateAssetMenu(fileName = "SettingsConfig", menuName = "Configs/SettingsConfig", order = 0)]
    public class SettingsConfig : ScriptableObject
    {
        [Header("GameSettings")] 
        public int CountSetElementsInOneSwipe;
        
        [Header("BoardSettings")]
        public Vector2 SizeBoard;
        public float OffsetBetweenCells;
        
        [Header("ElementSettings")]
        public int SizeElement;
    }
}