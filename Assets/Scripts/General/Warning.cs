using System.Collections;
using UnityEngine;

namespace General
{
    public class Warning : MonoBehaviour {
        public static Warning Instance { get; private set; }

        [SerializeField]
        private Material defaultTextMaterial; // Default material is assigned in the Inspector
        private Vector2 _defaultPosition;
        private void Awake() {
            // Ensures a single instance (Singleton pattern)
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public static void ShowWarning(string text) {
            if (Instance == null) {
                Debug.LogWarning("Warning instance is not available in the scene.");
                return;
            }
            Instance.CreateWarning(text);
        }

        private void CreateWarning(string text) {
            GameObject obj = new GameObject("WarningText");
            obj.transform.position = _defaultPosition;

            TextMesh myText = obj.AddComponent<TextMesh>();
            myText.text = text;
            myText.characterSize = 0.1f;
            myText.fontSize = 64;
            myText.color = Color.red;

            // Assign default material if provided
            if (defaultTextMaterial != null) {
                MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                renderer.material = new Material(defaultTextMaterial);
            }

            StartCoroutine(DestroyWarning(obj));
        }

        private IEnumerator DestroyWarning(GameObject obj) {
            yield return new WaitForSeconds(3);

            // Fade out the text
            MeshRenderer m = obj.GetComponent<MeshRenderer>();
            Color c = m.material.color;
            while (c.a > 0.01f) {
                c.a -= 0.01f;
                m.material.color = c;
                obj.transform.position += Vector3.up * 0.02f;
                yield return null;
            }
            Destroy(obj);
        }
    }
}