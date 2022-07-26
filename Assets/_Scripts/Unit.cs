using System;
using System.Security.Cryptography;
using _Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts {
    public class Unit : MonoBehaviour {
        [SerializeField] private Image itemImage;
        private RectTransform _rect;
        private Image _image;
        private ItemDataSo _item;

        public ItemDataSo Item {
            set {
                if (value != null) {
                    _item = value;
                    itemImage.enabled = true;
                    itemImage.sprite = Item.sprite;
                }
                else {
                    _item = value;
                    itemImage.enabled = false;
                }
            }
            get => _item;
        }
        public bool IsSelected { set; get; }
        private float _tarScale;
        private float _curScale;
        
        void Awake() {
            _rect = GetComponent<RectTransform>();
            _rect.localScale = Vector3.one;
            Item = null;
        }
        void Update() {
            var pos1 = Input.mousePosition;
            var pos2 = transform.position;
            pos1.z = 0;
            pos2.z = 0;
            if (Vector3.Distance(pos1, pos2) <= 1f) {
                _tarScale = 1f + 0.1f * Mathf.Sin(Time.time * 6f);
            }
            else {
                _tarScale = 1f;
            }
            _curScale = transform.localScale.x;
            _curScale = Calc.Approach(_curScale, _tarScale, 16f);
            transform.localScale = _curScale * Vector3.one;
        }
    }
}
