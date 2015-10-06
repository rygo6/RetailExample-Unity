using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace EC.Visualization
{
	public class PointItemSnap : ItemSnap
	{
		[SerializeField]
		private PointItemSnap[] _disableItemSnapArray;

		private void OnDrawGizmos()
		{	
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(this.transform.position, .01f);
		}

		public override Ray Snap(Item item, PointerEventData data)
		{
			ItemDrag dragItem = item.GetComponent<ItemDrag>();
			if (dragItem.AttachPointArray != null && dragItem.AttachPointArray.Length > 0)
			{
				for (int i = 0; i < dragItem.AttachPointArray.Length; ++i)
				{
					if (ItemUtility.TestTagArrays(dragItem.AttachPointArray[i].TagArray, ChildTagArray))
					{
						return new Ray(transform.position - dragItem.AttachPointArray[i].transform.localPosition, -transform.forward);
					}
				}
				return new Ray(transform.position, -transform.forward);
			}
			else
			{
				return new Ray(this.transform.position, -transform.forward);
			}
		}

		public override Vector3 NearestPoint(PointerEventData data)
		{
			return transform.position;
		}

		public override void AddItem(Item item)
		{
			ChildItemList.Add(item);
		}

		public override void RemoveItem(Item item)
		{
			ChildItemList.Remove(item);
		}

		public override bool ContainsItem(Item item)
		{
			return ChildItemList.Contains(item);
		}

		public override bool AvailableSnap()
		{
			return ChildItemList.Count == 0;
		}
	}
}
