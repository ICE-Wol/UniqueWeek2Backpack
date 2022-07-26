using UnityEngine;

namespace _Scripts {
    public class Calc : MonoBehaviour {
        private static float _epsilon = 0.0001f;

        /// <summary>
        /// A function which approach the current value to the target value, the closer the slower.
        /// </summary>
        /// <param name="current">Value type, the current value which is approaching the target value.</param>
        /// <param name="target">the final destination of the approach process.</param>
        /// <param name="rate">the rate of approach process, the bigger the slower.</param>
        /// <returns></returns>
        public static float Approach(float current, float target, float rate) {
            if (Mathf.Abs(current - target) >= _epsilon) {
                current -= (current - target) / rate;
            }
            else {
                current = target;
            }

            return current;
        }
    }
}
