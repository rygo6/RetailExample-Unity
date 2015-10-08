using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

namespace EC.Visualization
{
	public class ScrollItemEvent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
	{
		private VisualizationUI _parentPlannerUI;

		public ScrollRect _scrollRect;
	
		public RectTransform _scrollRectRectTransform;

		public int ItemArrayIndex { get; set; }

		private void Start()
		{
			_parentPlannerUI = GetComponentInParent<VisualizationUI>();

			_scrollRect = GetComponentInParent<ScrollRect>();

			_scrollRectRectTransform = _scrollRect.GetComponent<RectTransform>();		
		}

		public void OnPointerDown(PointerEventData data)
		{
			_inputMoved = Vector2.zero;
		}
	
		public void OnPointerUp(PointerEventData data)
		{

		}
	
		public void OnPointerClick(PointerEventData data)
		{
			_parentPlannerUI.ItemButtonClick(ItemArrayIndex);
		}
		
		public void OnBeginDrag(PointerEventData data)
		{
			_itemDragOut = false;		
			_scrollRect.OnBeginDrag(data);
		}

		public void OnDrag(PointerEventData data)
		{
			_inputMoved.x += Mathf.Abs(data.delta.x);
			_inputMoved.y += Mathf.Abs(data.delta.y);
		
			_scrollRectRectTransform.GetWorldCorners(_fourCornersArray);

			if (data.position.y < _fourCornersArray[0].y && _inputMoved.x < _dragThreshold)
			{
				_parentPlannerUI.SpawnItemFromMenuDrag(data, ItemArrayIndex);
				_itemDragOut = true;
			}
		
			_scrollRect.OnDrag(data);
		}
		private bool _itemDragOut;
		private float _dragThreshold = RetinaUtility.RFloat(50f);
		private Vector3[] _fourCornersArray = new Vector3[4];
		private Vector2 _inputMoved;

		public void OnEndDrag(PointerEventData data)
		{
			_scrollRect.OnEndDrag(data);
		}
	}
}