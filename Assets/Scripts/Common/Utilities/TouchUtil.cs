using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dasverse.Aleo
{
    public class TouchUtil : MonoBehaviour
    {
        public static void OnMultiTouch()
        {
            Input.multiTouchEnabled = true;
        }

        public static void OffMultiTouch()
        {
            Input.multiTouchEnabled = false;
        }
    }
}
