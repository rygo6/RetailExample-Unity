using UnityEngine;
using System.Collections;

namespace EC
{
	public class ItemSettings : MonoBehaviour 
	{
		public Color DownHighlightItemColor { get { return _downHighlightItemColor; } }
		[SerializeField]	
		private Color _downHighlightItemColor = Color.blue;

		public Color HighlightItemColor { get { return _highlightItemColor; } }
		[SerializeField]	
		private Color _highlightItemColor = Color.blue;

		public Color DropOutlineColor { get { return _dropOutlineColor; } }
		[SerializeField]	
		private Color _dropOutlineColor = Color.green;

		public Color InstantiateOutlineColor { get { return _instantiateOutlineColor; } }
		[SerializeField]
		private Color _instantiateOutlineColor = Color.cyan;

		public Color DeleteOutlineColor { get { return _deleteOutlineColor; } }
		[SerializeField]
		private Color _deleteOutlineColor = Color.red;

		public float OutlineSize { get { return _outlineSize; } }
		[SerializeField]
		private float _outlineSize = .003f;
	}
}