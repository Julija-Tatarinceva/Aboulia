using UnityEngine;

namespace Prefabs._2D.IrregularCircleUI.Scripts
{
	public class CircleAnimation : MonoBehaviour {

		public GameObject[] animObjects;

		// Use this for initialization
		private void Start () {
		
		}
	
		// Update is called once per frame
		private void Update () {
			foreach(GameObject go in animObjects)
			{
				Vector3 angle = go.transform.eulerAngles;

				angle.z += Time.deltaTime * 50f;

				go.transform.eulerAngles = angle;
			}
		}
	}
}
