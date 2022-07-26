using System;
using _Scripts.ScriptableObjects;
using UnityEditor.Tilemaps;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts {
    public class LightHolder : MonoBehaviour {
        public Aurora aurora;

        public Star star;
        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i <= 200; i++) {
                var temp = Instantiate(star.gameObject);
                temp.transform.SetParent(this.transform);
                temp.transform.position = new Vector3(Random.Range(0f,1920f), 0, 0);
            }
            
            for (int i = 0; i <= 2000; i += 20) {
                var temp = Instantiate(aurora.gameObject);
                var offset = Random.Range(-20f, 0f);
                temp.transform.SetParent(this.transform);
                temp.transform.position = new Vector3(i, offset, 0);

            }
        }
    }
}
