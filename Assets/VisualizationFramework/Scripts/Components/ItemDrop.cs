//#define LOG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EC.Visualization
{
	[RequireComponent(typeof(Item))]
	public class ItemDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public readonly List<ItemDrag> ChildItemDragList = new List<ItemDrag>();

		public ItemSnap[] ItemSnapArray { get; private set; }

		/// <summary>
		/// Stores the Item that Entered or Exited this object via the EventSystem.
		/// </summary>
		/// <value>The item entered.</value>
		public ItemDrag ItemDragEnteredThis
		{
			get
			{
				return _itemDragEnteredThis;
			}
			set
			{			
				//this be some clever stuff to make it so EnterIntoItemDrop
				//and ItemDragEntered can set each others setters back and forth
				//without causing a stack overflow, recursively calling each other
				//back and forth.	
				if (value == null)
				{
					ItemDrag tempItemDrag = _itemDragEnteredThis;
					_itemDragEnteredThis = null;
					if (tempItemDrag != null)
					{
						tempItemDrag.ThisEnteredDropItem = null;
					}
				}
				else if (_itemDragEnteredThis != value)
				{
					if (_itemDragEnteredThis != null)
					{
						_itemDragEnteredThis.ThisEnteredDropItem = null;
					}
					_itemDragEnteredThis = value;
					_itemDragEnteredThis.ThisEnteredDropItem = this;
				}
			}
		}
		private ItemDrag _itemDragEnteredThis;

		private void Awake()
		{
			ItemSnapArray = GetComponentsInChildren<ItemSnap>();
		}

		/// <summary>
		/// Returns the preallocated and sorted _itemSnapList.
		/// Pass in a PointerEventData and it returns the itemSnapList
		/// with the itemSnaps in order of nearest to farthest.
		/// </summary>
		/// <returns>The item snap list.</returns>
		/// <param name="data">Data.</param>
		public List<ItemSnap> SortedItemSnapList(PointerEventData data)
		{
			if (_sortedItemSnapList == null)
			{
				_sortedItemSnapList = new List<ItemSnap>();
				for (int i = 0; i < ItemSnapArray.Length; ++i)
				{
					_sortedItemSnapList.Add(ItemSnapArray[i]);
				}	
			}
			_sortedItemSnapList.Sort(
				delegate( ItemSnap itemSnap0, ItemSnap itemSnap1)
				{
					float distance0 = (data.pointerCurrentRaycast.worldPosition - itemSnap0.NearestPoint(data)).sqrMagnitude;
					float distance1 = (data.pointerCurrentRaycast.worldPosition - itemSnap1.NearestPoint(data)).sqrMagnitude;
					if (distance0 > distance1)
					{
						return 1;
					}
					else if (distance0 == distance1)
					{
						return 0;
					}
					else
					{
						return -1;
					}
				}
			);
			return _sortedItemSnapList;
		}
		private List<ItemSnap> _sortedItemSnapList;
	
		public bool CanAttach(string[] tagArray)
		{
			for (int i = 0; i < ItemSnapArray.Length; ++i)
			{
				if (ItemSnapArray[i].TagMatch(tagArray) && ItemSnapArray[i].AvailableSnap())
				{
					return true;
				}
			}
			return false;
		}
	
		public bool ContainsItem(Item containedItem)
		{
			for (int i = 0; i < ItemSnapArray.Length; ++i)
			{
				if (ItemSnapArray[i].ContainsItem(containedItem))
				{
					return true;
				}
			}
			return false;
		}

		public void OnDrop(PointerEventData data)
		{
			#if LOG		
			Debug.Log( "OnDrop " + this.name );	
			#endif
		
			ItemUtility.StateSwitch(data, GetComponent<Item>().State,
				OnDropAttached,
				null,
				null,
				null,
				null,
				null 
			);
		}
    
		private void OnDropAttached(PointerEventData data)
		{
			if (ItemDragEnteredThis != null)
			{
				GetComponent<Item>().SetShaderNormal();
				ItemDragEnteredThis.ParentItemDrop = this;
			}
		}
    
		public void OnPointerExit(PointerEventData data)
		{
			#if LOG		
			Debug.Log( "OnPointerExit " + this.name );	
			#endif	

			ItemUtility.StateSwitch(data, GetComponent<Item>().State,
				OnPointerExitAttached,
				null,
				null,
				null,
				null,
				null 
			);	
		}
    
		private void OnPointerExitAttached(PointerEventData data)
		{
			#if LOG		
			Debug.Log( "OnPointerExitAttached " + this.name );	
			#endif		
	
			if (data.pointerPress != null)
			{	
				GetComponent<Item>().SetShaderNormal();
				if (ItemDragEnteredThis != null)
				{
					ItemDragEnteredThis.AccessoryRendererState = false;				
					ItemDragEnteredThis = null;
				}
			}	
		}
	
		public void OnPointerEnter(PointerEventData data)
		{
			#if LOG		
			Debug.Log( "OnPointerEnter " + this.name );	
			#endif	
		
			ItemUtility.StateSwitch(data, GetComponent<Item>().State,
				OnPointerEnterAttached,
				null,
				null,
				null,
				null,
				null 
			);	
		}
    
		private void OnPointerEnterAttached(PointerEventData data)
		{
			#if LOG		
			Debug.Log( "OnPointerEnterAttached " + this.name );	
			#endif	
	
			if (data.pointerPress != null)
			{	
				Item item = data.pointerPress.GetComponent<Item>();
				if (item != null && CanAttach(item.TagArray) ||
				   item != null && ContainsItem(item))
				{	
					GetComponent<Item>().SetShaderOutline(Persistent.GetComponent<ItemSettings>().DropOutlineColor);
					ItemDragEnteredThis = item.GetComponent<ItemDrag>();
					ItemDragEnteredThis.AccessoryRendererState = true;						
				}
			}
		}
	}
}