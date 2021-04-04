using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace BS.Manager.Cameras{
	public class CameraManager : BaseManager<CameraManager>
	{
		[ShowNonSerializedField]
		protected Camera camera;

		private void Awake(){
			camera = FindObjectOfType<Camera>();
		}

		[Button("카메라 진동 테스트")]
		public void ShakeCamera(){
			ShakeCamera(3, 1, 0.01f);
		}

		public void ShakeCamera(float t, float amp, float freq = 0.01f){
			StartCoroutine(Shake(t, amp, freq));
		}

		protected IEnumerator Shake(float t, float amp, float freq){
			float duration = t;
			int index = 0;

			while(duration > 0){
				duration -= freq;
				camera.transform.localPosition = new Vector3(Random.Range(-amp, amp), Random.Range(-amp, amp), camera.transform.position.z);
				yield return new WaitForSeconds(freq);
			}

			yield return null;
		}
	}
}
