using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace EC.Visualization
{
	public abstract class ItemSnap : MonoBehaviour
	{
		public readonly List<Item> ChildItemList = new List<Item>();

		public string UniqueTick
		{ 
			get
			{ 
				if (string.IsNullOrEmpty(_uniqueTick))
				{
					_uniqueTick = ItemUtility.TickToString();
					Root.UniqueTickDictionary.Add(_uniqueTick, this);	
				}	
				return _uniqueTick; 
			} 
			set
			{
				if (!string.IsNullOrEmpty(_uniqueTick))
				{
					Root.UniqueTickDictionary.Remove(_uniqueTick);
				}
				_uniqueTick = value;
				Root.UniqueTickDictionary.Add(_uniqueTick, this);
			}	
		}
		private string _uniqueTick;

		public ItemRoot Root { get; private set; }

		public string[] ChildTagArray { get { return _childTagArray; } }
		[Header("Enter a specific set of tags for this point. If no tags are entered it will inherit them from the parent item")]
		[SerializeField]
		private string[] _childTagArray;

		protected virtual void Awake()
		{
			//TODO make automaticaly set by generator
			Root = FindObjectOfType<ItemRoot>();

			if (_childTagArray == null || _childTagArray.Length == 0)
			{
				_childTagArray = GetComponentInParent<Item>().ChildTagArray;
			}
		}
	
		public bool TagMatch(string[] tagArray)
		{
			return ItemUtility.TestTagArrays(tagArray, ChildTagArray);
		}
	
		/// <summary>
		/// Gets the point to snap to.
		/// </summary>
		public abstract Ray Snap(Item item, PointerEventData data);
	
		/// <summary>
		/// Nearest Snappable point.
		/// </summary>
		/// <returns>The point.</returns>
		/// <param name="data">Data.</param>
		public abstract Vector3 NearestPoint(PointerEventData data);
	
		public abstract void AddItem(Item item);
	
		public abstract void RemoveItem(Item item);
	
		/// <summary>
		/// Returns if this snap already contains a given item.
		/// </summary>
		/// <returns><c>true</c>, if item was containsed, <c>false</c> otherwise.</returns>
		/// <param name="item">Item.</param>
		public abstract bool ContainsItem(Item item);
	
		/// <summary>
		/// Determines whether this instance can snap the specified item.
		/// </summary>
		/// <returns><c>true</c> if this instance can snap the specified item; otherwise, <c>false</c>.</returns>
		/// <param name="item">Item.</param>
		public abstract bool AvailableSnap();
	}
}