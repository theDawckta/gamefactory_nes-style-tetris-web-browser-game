using UnityEngine;

namespace Game.Gameplay
{
    public abstract class PlayfieldRenderer : MonoBehaviour
    {
        public abstract void SetTileColor(int col, int row, Color color);
        public abstract void ResetTileColor(int col, int row);
    }
}
