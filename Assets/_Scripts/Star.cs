using System.Buffers.Text;
using UnityEngine;

namespace _Scripts {
    public class Star : MonoBehaviour
    {
        private float _speed;

        private float _oriSpeed;

        private float _rand;
        // Start is called before the first frame update
        void Start() {
            _speed = Random.Range(25f, 40f);
            _rand = Random.Range(1f, 2f);
            _oriSpeed = _speed;
        }

        // Update is called once per frame
        void Update() {
            _speed -= Time.deltaTime * _rand;
            transform.localScale = 0.2f * _speed / _oriSpeed * Vector3.one;
            var pos1 = transform.position;
            pos1.y += _speed * Time.deltaTime;
            transform.position = pos1;
            if(_speed <= 0f) {
                _speed = Random.Range(25f, 40f);
                _speed = _oriSpeed;
                var pos = transform.position;
                pos.y = 0;
                transform.position = pos;
            }
            
        }
    }
}
