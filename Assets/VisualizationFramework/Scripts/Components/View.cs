using UnityEngine;
using System.Collections;

namespace EC
{
	public class View : MonoBehaviour
	{
		[Header("View to start with")]
		[SerializeField]
		private View _attachToView;

		private void Update()
		{
			if (_attachToView != null)
			{
				transform.position = _attachToView.transform.position;
				transform.rotation = _attachToView.transform.rotation;
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawSphere(this.transform.position, 1.0f);
		}
	}
}