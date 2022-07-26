using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace _Scripts {
    public class MouseSub : MonoBehaviour {
        [SerializeField] private float radius;
        [SerializeField] private float scale;
        [SerializeField] private float rate;
        [SerializeField] private GameObject parent;
        private MouseMain _main;
        private GameObject _pre;
        private GameObject _follow;
        private Transform _transform;
        private int _order;
        private Vector3 _targetPos;
        private Vector3 _currentPos;
        private Vector3 _targetScale;
        private Vector3 _currentScale;
        private float _targetAngle;
        private Image _image;
        private int _timer;

        /// <summary>
        /// the function to generate sub mouse without revealing the private properties.
        /// It is static because it should be accessed without a instance of this class.
        /// </summary>
        /// <param name="main">The main mouse to follow.</param>
        /// <param name="order">The order of follow link of sub mouses.</param>
        /// <param name="pre">The previous sub mouse(if the order is zero,its the main mouse).</param>
        /// <returns>The reference of the generated MouseSub Script component.</returns>
        public static MouseSub GenerateMouseSub(int order, GameObject pre, MouseSub prefab, GameObject canvas) {
            MouseSub temp = Instantiate(prefab.gameObject).GetComponent<MouseSub>();
            temp._main = MouseMain.Mouse;
            temp._order = order;
            temp._pre = pre;
            var tempTransform = temp.transform;
            tempTransform.SetParent(canvas.transform);
            tempTransform.position = MouseMain.Mouse.transform.position;
            tempTransform.localScale = 0.1f * Vector3.one;

            return temp;
        }

        private void Start() {
            _image = GetComponent<Image>();
            _targetScale = scale * Vector3.one;
            _timer = 0;
        }

        void Update() {
            _timer++;
            _transform = transform;
            _currentPos = _transform.position;
            _currentScale = _transform.localScale;
            switch (_main.State) {
                case MouseState.Free:
                    _image.color = Color.white;
                    _follow = _pre;
                    _targetAngle = 0;
                    _targetScale = (12 - _order) / 12f * scale * Vector3.one;
                    _targetPos = _follow.transform.position;
                    break;
                case MouseState.Hold:
                    _follow = _main.gameObject;
                    _targetScale = scale * Vector3.one;
                    if (_order % 3 == (_timer / 210 % 3)) {
                        _targetScale.y *= 4f;
                        _targetScale.x *= 0.4f;
                    }
                    else {
                        _targetScale.x *= 0.4f;
                        _targetScale.y *= 0.6f;
                    }
                    _targetPos = Vector3.zero;
                    var center = _follow.transform.position;
                    _targetPos.x = center.x + Mathf.Cos(Mathf.Deg2Rad * (Time.time * 10f + _order / 12f * 360f)) * radius;
                    _targetPos.y = center.y + Mathf.Sin(Mathf.Deg2Rad * (Time.time * 10f + _order / 12f * 360f)) * radius;
                    _targetAngle = (Time.time * 10f + _order / 12f * 360f + 270f);
                    break;
                case MouseState.Selected:
                    _follow = _main.SelectTarget;
                    _targetScale = scale * Vector3.one;
                    if (_order % 3 == (_timer / 210 % 3)) {
                        _targetScale.y *= 4f;
                        _targetScale.x *= 0.4f;
                    }
                    else {
                        _targetScale.x *= 0.4f;
                        _targetScale.y *= 0.6f;
                    }
                    _targetPos = Vector3.zero;
                    var center2 = _follow.transform.position;
                    _targetPos.x = center2.x + Mathf.Cos(Mathf.Deg2Rad * (Time.time * 10f + _order / 12f * 360f)) * radius;
                    _targetPos.y = center2.y + Mathf.Sin(Mathf.Deg2Rad * (Time.time * 10f + _order / 12f * 360f)) * radius;
                    _targetAngle = (Time.time * 10f + _order / 12f * 360f + 270f);
                    break;
            }
            
            _currentPos.x = Calc.Approach(_currentPos.x, _targetPos.x, rate);
            _currentPos.y = Calc.Approach(_currentPos.y, _targetPos.y, rate);
            _currentPos.z = _targetPos.z;
            
            _currentScale.x = Calc.Approach(_currentScale.x, _targetScale.x, rate * 16);
            _currentScale.y = Calc.Approach(_currentScale.y, _targetScale.y, rate * 16);
            _currentScale.z = _targetScale.z;
            
            _transform.position = _currentPos;
            _transform.localScale = _currentScale;
            
            //TODO: figure it out 
            this.transform.rotation = Quaternion.Slerp(_transform.rotation, Quaternion.Euler(0,0,_targetAngle), 1);

        }
    }
}
