using UnityEngine;

namespace Dasverse.Aleo.System
{
    [CreateAssetMenu(menuName = "Datas/InputData")]
    public class InputData : ScriptableObject
    {
        public float AngleSpeedX = 20f;
        public float AngleSpeedY = 20f;
        public float CameraZoomSpeed = 20f;
        public float MinZoom = 10f;
        public float MaxZoom = 80f;
    }
}
