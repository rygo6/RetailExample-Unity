//#define LOG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EC.Visualization
{
	[RequireComponent(typeof(Item))]
	public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		/// <summary>
		/// Gets the transform that object SmoothDamps too.
		/// </summary>
		/// <value>The target transform.</value>
		public Transform TargetTransform { get; private set; }

		public AttachPoint[] AttachPointArray { get; private set; }

		public ItemSnap ParentItemSnap
		{ 
			get
			{ 
				return _parentItemSnap; 
			} 
			set
			{ 
				if (_parentItemSnap != null)
				{
					_parentItemSnap.RemoveItem(GetComponent<Item>());
					_parentItemSnap = null;
				}
				_parentItemSnap = value; 
				if (_parentItemSnap != null)
				{	
					_parentItemSnap.AddItem(GetComponent<Item>());
				}
			} 
		}
		private ItemSnap _parentItemSnap;
	
		public ItemDrop ParentItemDrop
		{ 
			get
			{ 
				return _parentItemDrop; 
			} 
			set
			{ 
				if (_parentItemDrop != null)
				{
					_parentItemDrop.ChildItemDragList.Remove(this);
					_parentItemDrop = null;
					ParentItemSnap = null;
				}		
				_parentItemDrop = value; 
				if (_parentItemDrop != null)
				{
					_parentItemDrop.ChildItemDragList.Add(this);
				}
			} 
		}
		private ItemDrop _parentItemDrop;
	
		public bool AccessoryRendererState
		{
			get
			{
				return _accessoryRendererState;
			}
			set
			{
				if (_accessoryRendererState != value)
				{
					for (int i = 0; i < _accessoryRendererArray.Length; ++i)
					{
						_accessoryRendererArray[i].enabled = value;
					}
					_accessoryRendererState = value;
				}				
			}
		}
		private bool _accessoryRendererState = true;
	
		[SerializeField]
		private Renderer[] _accessoryRendererArray;
	
		public ItemDrop ThisEnteredDropItem
		{ 
			get
			{ 
				return _thisEnteredDropItem; 
			} 
			set
			{ 	
				//this be some clever stuff to make it so EnterIntoItemDrop
				//and ItemDragEntered can set each others setters back and forth
				//without causing a stack overflow, recursively calling each other
				//back and forth. Is there a smarter way to do this?	
				if (value == null && _thisEnteredDropItem != null)
				{
					if (_thisEnteredDropItem != null)
					{
						_thisEnteredDropItem.ItemDragEnteredThis = null;
					}
					_thisEnteredDropItem = null;
				}
				else if (_thisEnteredDropItem != value)
				{
					if (_thisEnteredDropItem != null)
					{
						_thisEnteredDropItem.ItemDragEnteredThis = null;
					}
					_thisEnteredDropItem = value;
					_thisEnteredDropItem.ItemDragEnteredThis = this;
				}			
			} 
		}
		private ItemDrop _thisEnteredDropItem;

		private void Awake()
		{
			GameObject gameObject = new GameObject(this.name + "Target");
			TargetTransform = gameObject.GetComponent<Transform>();
			TargetTransform.position = GetComponent<Item>().transform.position;
			TargetTransform.forward = GetComponent<Item>().transform.forward;	

			AttachPointArray = GetComponentsInChildren<AttachPoint>();

		}

		private void Update()
		{
			switch (GetComponent<Item>().State)
			{
			case ItemState.Attached:
				SmoothToTargetPositionRotation();
				break;
			case ItemState.AttachedHighlighted:
				SmoothToTargetPositionRotation();
				break;				
			case ItemState.Dragging:
				SmoothToTargetPositionRotation();
				break;		
			case ItemState.Floating:
				SmoothToTargetPositionRotation();
				break;					
			case ItemState.Instantiate:
				SmoothToTargetPositionRotation();
				break;	
			case ItemState.NoInstantiate:
				SmoothToTargetPositionRotation();
				break;							
			}				
		}

		public ItemSnap NearestItemSnap(PointerEventData data)
		{
			if (ThisEnteredDropItem != null)
			{
				List<ItemSnap> sortedItemSnapList = ThisEnteredDropItem.SortedItemSnapList(data);
				for (int i = 0; i < sortedItemSnapList.Count; ++i)
				{
					if ((sortedItemSnapList[i].TagMatch(GetComponent<Item>().TagArray) && sortedItemSnapList[i].AvailableSnap()) ||
					   sortedItemSnapList[i].ContainsItem(GetComponent<Item>()))
					{
						return sortedItemSnapList[i];
					}	
				}	
			}
			return null;	
		}
	
		public void FloatObjectInMainCamera(PointerEventData data)
		{
			Ray ray = ViewSingleton.Instance.mainView.GetComponent<Camera>().ScreenPointToRay(data.position);
			SetTargetPositionRotation(ray.GetPoint(3.5f), Vector3.forward);			
		}
	
		/// <summary>
		/// Smooths to target position rotation.
		/// </summary>
		/// <returns><c>true</c>, if position and target are equal, <c>false</c> otherwise.</returns>
		private bool SmoothToTargetPositionRotation()
		{
			Item item = GetComponent<Item>();
			if (TargetTransform.position != item.transform.position || TargetTransform.eulerAngles != item.transform.eulerAngles)
			{
				SmoothToPointAndDirection(TargetTransform.position, _posDamp, TargetTransform.forward, _rotDamp);	
				return false;
			}
			else
			{
				return true;
			}
		}
		private const float _posDamp = .1f;
		private const float _rotDamp = .2f;
	
		/// <summary>
		/// Sets the target position rotation.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="direction">Direction. Tansform.Forward.</param>
		public void SetTargetPositionRotation(Vector3 position, Vector3 direction)
		{
			TranslateTargetPositionRotationRecursive(position - TargetTransform.position, direction - TargetTransform.forward);
		}
	
		public void TranslateTargetPositionRotationRecursive(Vector3 deltaPosition, Vector3 deltaDirection)
		{
			TargetTransform.Translate(deltaPosition, Space.World);
			TargetTransform.forward = TargetTransform.forward + deltaDirection;
			ItemDrop dropItemMod = GetComponent<ItemDrop>();
			if (dropItemMod != null)
			{
				for (int i = 0; i < dropItemMod.ChildItemDragList.Count; ++i)
				{
					dropItemMod.ChildItemDragList[i].TranslateTargetPositionRotationRecursive(deltaPosition, deltaDirection);
				}
			}
		}
	
		/// <summary>
		/// Sets the actual position to target.
		/// Immediately set transform equal to the targetPosition and Rotation.
		///	This is done in instances where object may switch to attached before fully done smoothing to target.
		/// </summary>
		public void SetActualPositionRotationToTarget()
		{
			GetComponent<Item>().transform.position = TargetTransform.position;
			_smoothVelocity = Vector3.zero;
			_smoothAngleVelocity = Vector3.zero;
		}
	
		/// <summary>
		/// Sets the target to acual position direction.
		/// </summary>
		public void SetTargetToAcualPositionDirection()
		{
			Item item = GetComponent<Item>();
			SetTargetPositionRotation(item.transform.position, item.transform.forward);		
		}
	
		/// <summary>
		/// Smooths to point and direction.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <param name="moveSmooth">Move smooth.</param>
		/// <param name="direction">Direction.</param>
		/// <param name="rotSmooth">Rot smooth.</param>
		private void SmoothToPointAndDirection(Vector3 point, float moveSmooth, Vector3 direction, float rotSmooth)
		{
			Item item = GetComponent<Item>();
			item.transform.position = Vector3.SmoothDamp(item.transform.position, point, ref _smoothVelocity, moveSmooth);	
			item.transform.forward = Vector3.SmoothDamp(item.transform.forward, direction, ref _smoothAngleVelocity, rotSmooth);	
		}
		private Vector3 _smoothVelocity;
		private Vector3 _smoothAngleVelocity;
	
		public void OnBeginDrag(PointerEventData data)
		{
			#if LOG
		Debug.Log( "OnBeginDrag " + this.name );	
			#endif	
		
			ItemUtility.StateSwitch(data, GetComponent<Item>().State,
				OnBeginDragAttached,
				OnBeginDragAttachedHighlighted,
				null,
				null,
				OnBeginDragInstantiate,
				OnBeginDragNoInstantiate 
			);			
		}
	
		private void OnBeginDragAttached(PointerEventData data)
		{
//			Orbit.Instance.InputNoOrbit[data.pointerId.NoNegative()] = false;	
			GetComponent<Item>().SetShaderNormal();		
		}
	
		private void OnBeginDragAttachedHighlighted(PointerEventData data)
		{
			GetComponent<Item>().SetShaderOutline(Persistent.Get<ItemSettings>().HighlightItemColor);			
			SwitchAttachedToDragging(data);	
		}
	
		private void OnBeginDragInstantiate(PointerEventData data)
		{
			GetComponent<Item>().SetShaderOutline(Persistent.Get<ItemSettings>().InstantiateOutlineColor);
		}
	
		private void OnBeginDragNoInstantiate(PointerEventData data)
		{
			GetComponent<Item>().SetShaderNormal();
		}
	
		private void SwitchAttachedToDragging(PointerEventData data)
		{
			AccessoryRendererState = false;
			ParentItemDrop = null;
			Item item = GetComponent<Item>();
			item.ResetColliderSize();
			item.AddToHoldList();
			item.State = ItemState.Dragging;
		
			//This ensure that the item is still hovering over the item_Drop it was attached to
			//otherwise it disattaches it, this is done because OnPointerExit will only get called on
			//the item_Drop if the mouse pointer actuall exits it's collider, which may not always occur
			if (data.pointerCurrentRaycast.gameObject == null ||
			   (data.pointerCurrentRaycast.gameObject != null && ThisEnteredDropItem != null &&
			   data.pointerCurrentRaycast.gameObject.transform.parent.gameObject != ThisEnteredDropItem.gameObject))
			{
				ThisEnteredDropItem = null;
			}
		
		}
	
		public void OnDrag(PointerEventData data)
		{
			#if LOG
		Debug.Log("OnDrag "+this.name);	
			#endif	
		
			ItemUtility.StateSwitch(data, GetComponent<Item>().State,
				null,
				null,
				OnDragDragging,
				null,
				null,
				null 
			);					
		}
	
		private void OnDragDragging(PointerEventData data)
		{
			if (ThisEnteredDropItem == null)
			{
				FloatObjectInMainCamera(data);		
				ParentItemSnap = null;
			}
			else
			{
				ItemSnap currentNearestItemSnap = NearestItemSnap(data);
				if (currentNearestItemSnap == null)
				{
					FloatObjectInMainCamera(data);	
					ParentItemSnap = null;	
				}
				else
				{	
					if (ParentItemSnap != currentNearestItemSnap)
					{
						ParentItemSnap = currentNearestItemSnap;
					}		
					Ray ray = ParentItemSnap.Snap(GetComponent<Item>(), data);
					SetTargetPositionRotation(ray.origin, ray.direction); 
				}   
			}
		}

		public void OnEndDrag(PointerEventData data)
		{
			#if LOG
		Debug.Log( "OnEndDrag " + this.name );	
			#endif	
		
			ItemUtility.StateSwitch(data, GetComponent<Item>().State,
				null,
				null,
				OnEndDragDragging,
				null,
				null,
				null 
			);				  
		}
    
		private void OnEndDragDragging(PointerEventData data)
		{		
			Item item = GetComponent<Item>(); 
			if (ThisEnteredDropItem == null)
			{
				StartCoroutine(item.DestroyItemCoroutine());
			}
			else
			{
				item.RemoveFromHoldList();
				AccessoryRendererState = true;
				item.SetLayerRecursive(8);
				item.State = ItemState.AttachedHighlighted;			
			}
		}
	}
}