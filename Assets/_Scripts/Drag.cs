using System;
using System.Security.Cryptography;
using _Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts {
    public class Drag : MonoBehaviour {
        private Vector3 _curPos;
        private Image _image;
        private ItemDataSo _item;
        private float _tarScale;
        private float _curScale;
        public Vector3 TarPos { set; get; }
        public Vector2Int DesInGrid { set; get; }
        public bool IsDragging { private set; get; }
        public bool IsRetrieving { private set; get; }

        public void ResetDrag(Vector3 pos, Unit unit) {
            IsDragging = true;
            transform.position = pos;
            _curPos = pos;
            _tarScale = 1.5f;
            _item = unit.Item;
            DesInGrid = BackpackManager.Manager.CheckPosition(unit.transform.position);
            _image.sprite = unit.Item.sprite;
            _image.enabled = true;
        }

        public void Release() {
            var pos = BackpackManager.Manager.CheckPosition(transform.position);
            if (pos.x != -1) {
                if(BackpackManager.Manager.GetGrid(pos).Item == null)
                    DesInGrid = pos;
            }
            if (BackpackManager.Manager.CheckEquipmentPosition(transform.position)) {
                BackpackManager.Manager.FillInEquipmentGrid();
            }
            IsDragging = false;
            TarPos = BackpackManager.Manager.GetGridPosition(DesInGrid);
            IsRetrieving = true;
            _tarScale = 1f;
            
        }

        public void IsArrived() {
            if ((TarPos - _curPos).magnitude <= 0.1f) {
                IsRetrieving = false;
                BackpackManager.Manager.FillInSingleGrid(DesInGrid, _item);
                MouseMain.Mouse.SelectTarget = BackpackManager.Manager.GetGrid(DesInGrid).gameObject;
                _image.sprite = null;
                _image.enabled = false;
            }
        }

        private void Awake() {
            _image = GetComponent<Image>();
        }
        
        void Start() {
            _curPos = transform.position;
            TarPos = Vector3.zero;
        }
        
        void Update()
        {
            if (IsDragging) {
                TarPos = MouseMain.Mouse.transform.position;
            }

            if (IsRetrieving) {
                IsArrived();
            }
            _curPos = transform.position;
            _curPos.x = Calc.Approach(_curPos.x, TarPos.x, 8f);
            _curPos.y = Calc.Approach(_curPos.y, TarPos.y, 8f);
            transform.position = _curPos;

            _curScale = transform.localScale.x;
            _curScale = Calc.Approach(_curScale, _tarScale, 4f);
            transform.localScale = _curScale * Vector3.one;
        }
    }
}
