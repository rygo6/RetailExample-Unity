using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace EC.Visualization
{
	public class ItemRoot : MonoBehaviour
	{
		public readonly Dictionary<string,MonoBehaviour> UniqueTickDictionary = new Dictionary<string,MonoBehaviour>();

		public readonly List<Item> ItemHoldList = new List<Item>();

		public readonly List<Item> ItemHighlightList = new List<Item>();

		private List<Item> _rootItemRaycastList;
	
		private void Awake()
		{
			_rootItemRaycastList = new List<Item>(Item.FindObjectsOfType<Item>());
		}

		public void UnHighlightAll()
		{
			for (int i = 0; i < ItemHighlightList.Count; ++i)
			{
				ItemHighlightList[i].UnHighlight();
			}	
		}

		public void SetAllOutlineNormalAttach()
		{
			System.Action<Item> action = delegate( Item itemRaycast)
			{
				itemRaycast.SetShaderNormal();
				itemRaycast.State = ItemState.Attached;
			};		
			CallDelegateAll(action);	
		}

		public void SetAllActualPositionToTarget()
		{
			System.Action<Item> action = delegate( Item item)
			{
				ItemDrag dragItemMod = item.GetComponent<ItemDrag>();
				if (dragItemMod != null)
				{
					dragItemMod.SetActualPositionRotationToTarget();
				}
			};		
			CallDelegateAll(action);	
		}

		public int CallDelegateTagFilter(System.Func<Item,bool> filterAction, System.Action<Item> trueAction, System.Action<Item> falseAction)
		{
			int trueCount = 0;
			for (int i = 0; i < _rootItemRaycastList.Count; ++i)
			{
				trueCount += CallDelegateTagFilterItemRaycast(_rootItemRaycastList[i], filterAction, trueAction, falseAction);		
			}
			return trueCount;
		}

		private int CallDelegateTagFilterItemRaycast(Item item, System.Func<Item,bool> filterAction, System.Action<Item> trueAction, System.Action<Item> falseAction)
		{	
			int trueCount = 0;
			if (filterAction(item))
			{
				trueAction(item);
				trueCount = 1;
			}
			else if (falseAction != null)
			{
				falseAction(item);
			}
			ItemDrop itemDrop = item.GetComponent<ItemDrop>();
			if (itemDrop != null)
			{
				for (int i = 0; i < itemDrop.ChildItemDragList.Count; ++i)
				{
					trueCount += CallDelegateTagFilterItemRaycast(itemDrop.ChildItemDragList[i].GetComponent<Item>(), filterAction, trueAction, falseAction);		
				}
			}
			return trueCount;
		}

		public void CallDelegateAll(System.Action<Item> action)
		{
			for (int i = 0; i < _rootItemRaycastList.Count; ++i)
			{
				CallDelegateItemRaycast(_rootItemRaycastList[i], action);		
			}	
		}

		public void CallDelegateItemRaycast(Item item, System.Action<Item> action)
		{
			action(item);
			ItemDrop itemDrop = item.GetComponent<ItemDrop>();
			if (itemDrop != null)
			{		
				for (int i = 0; i < itemDrop.ChildItemDragList.Count; ++i)
				{
					CallDelegateItemRaycast(itemDrop.ChildItemDragList[i].GetComponent<Item>(), action);		
				}	
			}
		}
	}
}
