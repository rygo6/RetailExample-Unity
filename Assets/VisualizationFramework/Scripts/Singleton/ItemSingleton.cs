using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace EC.Visualization
{
	public class ItemSingleton : Singleton<ItemSingleton>
	{
		public readonly Dictionary<string,MonoBehaviour> UniqueTickDictionary = new Dictionary<string,MonoBehaviour>();

		public readonly List<Item> ItemRaycastHoldList = new List<Item>();

		public readonly List<Item> ItemRaycastHighlightList = new List<Item>();

		private List<Item> _rootItemRaycastList;
	
		public Color DownHighlightItemColor { get { return _downHighlightItemColor; } }
		[SerializeField]	
		private Color _downHighlightItemColor = Color.blue;
	
		public Color HighlightItemColor { get { return _highlightItemColor; } }
		[SerializeField]	
		private Color _highlightItemColor = Color.blue;
	
		public Color DropOutlineColor { get { return _dropOutlineColor; } }
		[SerializeField]	
		private Color _dropOutlineColor = Color.green;
	
		public Color IinstantiateOutlineColor { get { return _instantiateOutlineColor; } }
		[SerializeField]
		private Color _instantiateOutlineColor = Color.cyan;
	
		public Color DeleteOutlineColor { get { return _deleteOutlineColor; } }
		[SerializeField]
		private Color _deleteOutlineColor = Color.red;

		public float OutlineSize { get { return _outlineSize; } }
		[SerializeField]
		private float _outlineSize = .003f;
	
		private void Awake()
		{
			_rootItemRaycastList = new List<Item>(Item.FindObjectsOfType<Item>());
		}
    
		public void GetUniqueTick(string uniqueTick)
		{


		}

		public void UnHighlightAll()
		{
			for (int i = 0; i < ItemRaycastHighlightList.Count; ++i)
			{
				ItemRaycastHighlightList[i].UnHighlight();
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
	
		public string TickToString()
		{
			long ticks = System.DateTime.Now.Ticks;
			byte[] bytes = System.BitConverter.GetBytes(ticks);
			string id = System.Convert.ToBase64String(bytes)
			.Replace('+', '_')
				.Replace('/', '-')
                .TrimEnd('=');	
			return id;
		}
	}
}
