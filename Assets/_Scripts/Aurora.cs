using UnityEngine;
using UnityEngine.UI;

namespace _Scripts {
    public class Aurora : MonoBehaviour {
        private Image _image;
        private float _rand1;
        private float _rand2;
        // Start is called before the first frame update
        void Start() {
            _image = GetComponent<Image>();
            _rand1 = Random.Range(0, 360f);
            _rand2 = Random.Range(0, 360f);
        }

        // Update is called once per frame
        void Update()
        {
            var scale = transform.localScale;
            scale.y = 0.35f * Mathf.Sin(Mathf.Deg2Rad * (Time.time * 18f + _rand1)) + 1f;
            transform.localScale = scale;
            var alpha = Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * (Time.time * 16f + _rand2))/2f) + 0.01f;
            var color = _image.color;
            color.a = alpha;
            _image.color = color;
        }
    }
}
