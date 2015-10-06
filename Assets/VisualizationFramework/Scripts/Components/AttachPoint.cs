using UnityEngine;
using System.Collections;

namespace EC.Visualization
{
	public class AttachPoint : MonoBehaviour
	{
		public string[] TagArray { get { return tagArray; } set { tagArray = value; } }
		[SerializeField]
		private string[] tagArray;
	}
}