using UnityEngine;

namespace _Scripts {
    public class Background : MonoBehaviour
    {
        void Update() {
            var scale = transform.localScale;
            scale.y = 0.2f * Mathf.Sin(Mathf.Deg2Rad * Time.time * 8f) + 0.8f;
            transform.localScale = scale;
        }
    }
}
