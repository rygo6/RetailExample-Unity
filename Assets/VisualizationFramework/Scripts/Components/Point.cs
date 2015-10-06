using UnityEngine;
using System.Collections;

namespace EC.Visualization
{
	public class Point : MonoBehaviour
	{
		private void OnDrawGizmos()
		{	
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(this.transform.position, .01f);
		}
	}
}