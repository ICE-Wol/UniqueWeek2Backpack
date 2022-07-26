using UnityEngine;
using UnityEngine.UI;

namespace _Scripts {
    public enum MouseState {
        Free = 0,
        Hold = 1,
        Selected = 2,
    }
    public class MouseMain : MonoBehaviour {
        public static MouseMain Mouse;
        
        [SerializeField] private MouseSub mouseSubPrefab;
        [SerializeField] private GameObject canvas;
        [SerializeField] private Drag drag;
        [SerializeField] private float scale;
        private MouseSub[] _subs;
        private RectTransform _rect;
        private Transform _transform;
        private float _currentScale;
        private float _targetScale;

        public MouseState State { get; set; }
        
        public GameObject SelectTarget { set; get; }

        void Start() {
            if (Mouse == null) {
                Mouse = this;
            }
            else {
                Destroy(this.gameObject);
            }
            _rect = GetComponent<RectTransform>();
            _subs = new MouseSub[12];
            State = MouseState.Free;
            _subs[0] = MouseSub.GenerateMouseSub(0, this.gameObject, mouseSubPrefab, canvas);
            for (int i = 1; i < 12; i++)
                _subs[i] = MouseSub.GenerateMouseSub(i, _subs[i - 1].gameObject, mouseSubPrefab, canvas);
            this.transform.localScale = 0.3f * Vector3.one;
        }
        
        void Update() {
            _transform = transform;
            _transform.position = Input.mousePosition;
            //only when the render mode of the Canvas is overlay can u use this directly.
            _currentScale = _transform.localScale.x;

            if (Input.GetMouseButtonDown(0)) {
                var pos = BackpackManager.Manager.CheckPosition(_transform.position);
                if (pos.x != -1) {
                    var grid = BackpackManager.Manager.GetGrid(pos);
                    if (grid.Item != null) {
                        State = MouseState.Selected;
                        if (!drag.IsDragging && !drag.IsRetrieving) {
                            SelectTarget = drag.gameObject;
                            BackpackManager.Manager.SetDisplay(grid.Item);
                            drag.ResetDrag(transform.position, grid);
                            grid.Item = null;
                        }
                        else {
                            SelectTarget = grid.gameObject;
                        }
                    }
                }
                else {
                    State = MouseState.Hold;
                    _targetScale = 0.2f;
                }
            }

            if (State == MouseState.Selected && Input.GetMouseButtonUp(0) && drag.IsDragging) {
                drag.Release();
            }
            if (State != MouseState.Selected && Input.GetMouseButtonUp(0)) {
                State = MouseState.Free;
                _targetScale = 0.3f;
            } 
            
            _currentScale = Calc.Approach(_currentScale, _targetScale, 256f);
            _transform.localScale = _currentScale * Vector3.one;
        }

    }
}
