using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using _Scripts.ScriptableObjects;
using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace _Scripts {
    public enum ItemName {
        Default = 0,
        BlankCard = 501,
        Needle = 201,
        Laser = 101,
        Sun = 301,
        Life = 601,
        Spell = 602,
    }
    
    public enum Type {
        Default = 0,
        FastWeapon = 1,
        SlowWeapon = 2,
        SpellCard = 3,
        Equipment = 4,
        Effect = 5,
        Consumable = 6,
    }

    public class BackpackManager : MonoBehaviour {
        struct ItemCondition {
            public int Number;
            public int Quantity;
            public ItemDataSo Data;
        }

        public static BackpackManager Manager;
        private const float Length = 64f * 1.5f + 16f;
        [SerializeField] private Vector3 oriPackPoint;
        [SerializeField] private int xNum;
        [SerializeField] private int yNum;
        [SerializeField] private int wallet;
        [SerializeField] private Unit unitPrefab;
        [SerializeField] private List<ItemDataSo> itemDataList;
        [SerializeField] private List<int> itemCount;
        [SerializeField] private List<Toggle> toggleList;
        [SerializeField] private List<Unit> equippedList;
        [SerializeField] private TMP_Text displayTMP; //TODO: note this.
        [SerializeField] private TMP_Text interactTMP;
        [SerializeField] private TMP_InputField searchTMP;
        [SerializeField] private TMP_Text equipmentTMP;
        [SerializeField] private Image displayImage;
        private Unit[,] _backpack;
        private ItemDataSo[,] _condition;
        private Type _tag;
        private ItemDataSo _display;

        private void Awake() {
            if (Manager == null) {
                Manager = this;
            }
            else {
                Destroy(this.gameObject);
            }
            _backpack = new Unit[9,9];
            _condition = new ItemDataSo[7,10];
            _tag = Type.Default;
        }

        void Start() {
            _display = itemDataList[0];
            oriPackPoint = transform.position;
            Initialization();
            GenerateGrid();
            GenerateEquipmentGrid();
            FillInGrid(_tag);
            RefreshDisplay();
        }

        public Vector2Int CheckPosition(Vector3 pos) {
            var valid = ((pos.x - oriPackPoint.x) <= Length * xNum) && 
                        ((oriPackPoint.y - pos.y) <= Length * yNum) &&
                        ((pos.x - oriPackPoint.x) >= 0f) &&
                        ((oriPackPoint.y - pos.y) >= 0f);
            if (!valid) {
                return new Vector2Int(-1, -1);
            }
            else {
                int x = (int)((pos.x - oriPackPoint.x) / Length);
                int y = (int)((oriPackPoint.y - pos.y) / Length);
                return new Vector2Int(x, y);
            }
        }

        public bool CheckEquipmentPosition(Vector3 pos) {
            var tempPos = equipmentTMP.gameObject.transform.position;
            var validEquipment = ((pos.x - tempPos.x) <= Length * 7f) &&
                                 ((pos.y - tempPos.y) <= 1f * Length) &&
                                 ((pos.x - tempPos.x) >= Length * 1f) &&
                                 ((pos.y - tempPos.y) >= 0f * Length);
            return validEquipment;
        }

        public Vector3 GetGridPosition(Vector2Int point) {
            var pos = new Vector3(point.x * Length, -point.y * Length, 0f);
            var offset = new Vector3(Length / 2f, -Length / 2f, 0f);
            return pos + offset + oriPackPoint;
        }
        public Unit GetGrid(Vector2Int pos) {
            return _backpack[pos.y, pos.x];
        }

        private void Initialization() {
            int pData = 0;
            for (int t = 1; t <= 6; t++) {
                for (int n = 1; n <= itemCount[t]; n++) {
                    pData++;
                    var temp = itemDataList[pData];
                    _condition[t,n] = temp;
                }
            }
        }
        
        private void GenerateGrid() {
            for (int y = 0; y < yNum; y++) {
                for (int x = 0; x < xNum; x++) {
                    var temp = Instantiate(unitPrefab);
                    var tempTransform = temp.transform;
                    var pos = new Vector3(x * Length, -y * Length, 0f);
                    var offset = new Vector3(Length / 2f, -Length / 2f, 0f);
                    tempTransform.position = pos + offset + oriPackPoint;
                    tempTransform.SetParent(this.transform);
                    _backpack[y,x] = temp.GetComponent<Unit>();
                }
            }
        }
        
        private void GenerateEquipmentGrid() {
            for (int i = 1; i <= 6; i++) {
                equippedList[i].transform.localPosition =
                    new Vector3((i + 1f) * Length, 0f, 0f);
            }
        }
        
        private void RefreshGrid() {
            for (int y = 0; y < yNum; y++) {
                for (int x = 0; x < xNum; x++) {
                    _backpack[y, x].Item = null;
                }
            }
        }

        private void FillInGrid(Type typeTag) {
            int pData = 0;
            if (typeTag == Type.Default) {
                for (int t = 1; t <= 6; t++) {
                    for (int n = 1; n <= itemCount[t]; n++) {
                        var y = (int) (pData / xNum);
                        var x = pData % xNum;
                        if(_condition[t, n].quantity == 0) continue;
                        _backpack[y, x].Item = _condition[t, n];
                        pData++;
                    }
                }
            }
            else {
                int t = (int) typeTag;
                for (int n = 1; n <= itemCount[t]; n++) {
                    var y = (int) (pData / xNum);
                    var x = pData % xNum;
                    if(_condition[t, n].quantity == 0) continue;
                    _backpack[y, x].Item = _condition[t, n];
                    pData++;
                }
            }
        }

        public void FillInSingleGrid(Vector2Int pos, ItemDataSo item) {
            _backpack[pos.y, pos.x].Item = item;
        }

        public void FillInEquipmentGrid() {
            var t = (int) _display.type;
            if (t == 0) return;
            if (equippedList[t].Item != null && equippedList[t].Item.number == _display.number) {
                equippedList[t].Item = null;
                return;
            }
            equippedList[t].Item = _display;
        }
        
        public void OnToggleValueChanged() {
            for (int i = 0; i <= 6; i++) {
                if (toggleList[i].isOn) {
                    _tag = (Type) i;
                    RefreshGrid();
                    FillInGrid(_tag);
                    return;
                }
            }
        }

        public void SearchGridForItem() {
            for (int y = 0; y < yNum; y++) {
                for (int x = 0; x < xNum; x++) {
                    if(_backpack[y,x].Item == null) continue;
                    var itemName = _backpack[y, x].Item.name;
                    var itemNum = _backpack[y, x].Item.number.ToString();
                    if (searchTMP.text == itemName || searchTMP.text == itemNum) {
                        MouseMain.Mouse.State = MouseState.Selected;
                        MouseMain.Mouse.SelectTarget = _backpack[y, x].gameObject;
                        _display = _backpack[y, x].Item;
                        RefreshDisplay();
                    }
                }
            }
        }

        public void SellItem() {
            if (_display.number == 0 || _display.quantity <= 0) {
                equipmentTMP.text =
                    "Equipments:                                                                  Wallet: " +
                    wallet + " D";
                return;
            }
            wallet += _display.price;
            var t = _display.number / 100;
            var n = _display.number % 100;
            for (int y = 0; y < yNum; y++) {
                for (int x = 0; x < xNum; x++) {
                    if (_backpack[y, x].Item == null) continue;
                    if (_backpack[y, x].Item.number == _condition[t, n].number) {
                        _backpack[y, x].Item.quantity -= 1;
                        if (_backpack[y, x].Item.quantity <= 0) {
                            for (int i = 1; i <= 6; i++) {
                                if (equippedList[i].Item == null) continue;
                                if (equippedList[i].Item.number == _backpack[y, x].Item.number)
                                    equippedList[i].Item = null;
                            }
                            _backpack[y, x].Item = null;
                        }
                        SetDisplay(_backpack[y, x].Item);
                    }
                }
            }

            equipmentTMP.text =
                "Equipments:                                                                  Wallet: " +
                wallet + " D";
        }

        public void BuyItem() {
            if (_display.number == 0 || _display.quantity <= 0) {
                equipmentTMP.text =
                    "Equipments:                                                                  Wallet: " +
                    wallet + " D";
                return;
            }
            if (wallet < _display.price) return;
            wallet -= _display.price;
            var t = _display.number / 100;
            var n = _display.number % 100;
            for (int y = 0; y < yNum; y++) {
                for (int x = 0; x < xNum; x++) {
                    if (_backpack[y, x].Item == null) continue;
                    if (_backpack[y, x].Item.number == _condition[t, n].number) {
                        _backpack[y, x].Item.quantity += 1;
                        SetDisplay(_backpack[y, x].Item);
                    }
                }
            }

            equipmentTMP.text =
                "Equipments:                                                                  Wallet: " +
                wallet + " D";
        }

        public void SetDisplay(ItemDataSo item) {
            _display = item;
            RefreshDisplay();
        }
        private void RefreshDisplay() {
            if (_display == null)
                _display = itemDataList[0];
            displayTMP.text = "<b>Name:</b>\n" +
                       _display.itemName + "\n\n" +
                       "<b>Owner:</b>\n" +
                       _display.owner + "\n\n" +
                       "<b>Description:</b>\n" +
                       _display.description + "\n\n" +
                       "<b>Ability:</b>\n" +
                       _display.ability + "\n\n" +
                       "<b>Special:</b>\n" +
                       _display.special + "\n\n";
            displayImage.sprite = _display.sprite;

            interactTMP.text = "<b>Number:</b>\n "+
                               _display.number + "\n\n" +
                               "<b>Type:</b>\n"+
                               _display.type + "\n\n" +
                               "<b>Now Have:</b>\n " +
                               _display.quantity + "\n\n" +
                               "<b>Sell For:</b>\n " +
                               _display.price + "  D";
            
            equipmentTMP.text =
                "Equipments:                                                                  Wallet: " +
                wallet + " D";

        }
    }
}