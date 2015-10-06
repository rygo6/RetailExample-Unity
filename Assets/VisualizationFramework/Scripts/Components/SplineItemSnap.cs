using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EC.Visualization
{
	public class SplineItemSnap : ItemSnap
	{
		private readonly List<SplineReplicator> SplineReplicatorList = new List<SplineReplicator>();

		private Point[] _pointArray;
	
		[SerializeField]
		private int _maxItemCount = 8;

		private void Awake()
		{
			_pointArray = GetComponentsInChildren<Point>();

			base.Awake();
		}

		public override Ray Snap(Item item, PointerEventData data)
		{
			Ray ray = AttachPointRay();
			ray.origin = UpdateItem(item, data.pointerCurrentRaycast.worldPosition);	
			ray.direction = -ray.direction;	
			return ray;
		}
	
		public override Vector3 NearestPoint(PointerEventData data)
		{
			Ray lineRay = AttachPointRay();
			Vector3 linePoint = Math3D.ProjectPointOnLine(lineRay.origin, lineRay.direction, data.worldPosition);			
			return linePoint;
		}
	
		public override void AddItem(Item item)
		{
			SplineReplicator replicator = gameObject.AddComponent<SplineReplicator>();
			replicator.Item = item;
			SplineReplicatorList.Add(replicator);
		}
	
		public override void RemoveItem(Item item)
		{
			#if LOG
			Debug.Log("RemoveItem "+this.name);
			#endif
			SplineReplicator replicator = SplineReplicatorList.Find(r => r.Item == item);
			SplineReplicatorList.Remove(replicator);
			Destroy(replicator);
			SetStartEndPoint();
			SetColliderSize();	
		}
	
		public override bool ContainsItem(Item item)
		{
			SplineReplicator replicator = SplineReplicatorList.Find(r => r.Item == item);
			if (replicator == null)
			{
				return false;
			}
			else
			{
				return true;
			}	
		}
    
		public override bool AvailableSnap()
		{
			return SplineReplicatorList.Count < _maxItemCount;
		}

		/// <summary>
		/// Adds the item raycast or updates it if already in list.
		/// </summary>
		/// <returns> Returns the midpoint of where Item was Added. </returns>
		/// <param name="itemRaycast">Item raycast.</param>
		/// <param name="hitPoint">Hit point.</param>
		public Vector3 UpdateItem(Item item, Vector3 hitPoint)
		{
			//Retieve or Create a new duplicate data
			SplineReplicator replicator = SplineReplicatorList.Find(r => r.Item == item);

			SplineReplicatorList.Remove(replicator);
		
			SetStartEndPoint();
		
			Ray lineRay = AttachPointRay();

			Vector3 linePoint = Math3D.ProjectPointOnLine(lineRay.origin, lineRay.direction, hitPoint);										
				
			InsertReplicator(replicator, linePoint);
		
			SetStartEndPoint();
			SetColliderSize();
		
			return replicator.DuplicateRangeMidPoint();
		}
	
		private Ray AttachPointRay()
		{
			Vector3 from = _pointArray[0].transform.position;
			Vector3 to = _pointArray[1].transform.position;
			Vector3 direction = to - from;
			direction.Normalize();
			return new Ray(from, direction);		
		}
	
		public void InsertReplicator(SplineReplicator replicator, Vector3 inserPoint)
		{
			Vector3 from = _pointArray[0].transform.position;
			Vector3 to = _pointArray[1].transform.position;
			float fromToDistance = Vector3.Distance(from, to);	
	
			float inserPointDistance = Vector3.Distance(from, inserPoint);
			float insertRatio = inserPointDistance / fromToDistance;	

			float midRatio = 0f;
			float priorRatio = 0f;		
			int insertIndex = -1;
			for (int i = 0; i < SplineReplicatorList.Count; ++i)
			{
				midRatio = (Vector3.Distance(from, SplineReplicatorList[i].DuplicateRangeMidPoint()) / fromToDistance);
				if (insertRatio > priorRatio && insertRatio < midRatio)
				{
					insertIndex = i;
					break;
				}		
			
				priorRatio = midRatio;
			}
		
			if (insertIndex == -1)
			{
				//check if it is in last segement between final item and end
				if (insertRatio > priorRatio && insertRatio < 1f)
				{	
					insertIndex = SplineReplicatorList.Count;
				}
			
				//if beyond 0 to 1, just force it to beginning or end
				if (insertRatio > 1f)
				{
					insertIndex = SplineReplicatorList.Count;
				}
				if (insertRatio < 0f)
				{
					insertIndex = 0;
				}
			}	
		
			//if this is in the very last spot or there is none added, then add instead of insert
			if (insertIndex != -1 && SplineReplicatorList.Count == 0)
			{
				SplineReplicatorList.Add(replicator);
			}
			else if (insertIndex != -1)
			{	
				SplineReplicatorList.Insert(insertIndex, replicator);
			}		
		}
	
		public void SetColliderSize()
		{
			for (int d = 0; d < SplineReplicatorList.Count; ++d)
			{	
				SplineReplicator replicator = SplineReplicatorList[d];
				Item item = replicator.Item;
				for (int i = 0; i < item.ColliderArray.Length; ++i)
				{
					item.ColliderArray[i].size = new Vector3(item.ColliderArray[i].size.x, item.ColliderArray[i].size.y, replicator.DisctanceBetweenStartEnd());
				}
			}
		}
	
		/// <summary>
		/// Sets the ratios on DuplicateDataList on assumption of equal spacing
		/// </summary>
		public void SetStartEndPoint()
		{
			float priorRatio = 0f;
			float ratioSplit = 1f / (float)SplineReplicatorList.Count;

			for (int i = 0; i < SplineReplicatorList.Count; ++i)
			{			
				SplineReplicatorList[i].SetStartEndPoint(_pointArray[0].transform.position, 
					_pointArray[1].transform.position, 
					priorRatio, 
					priorRatio + ratioSplit);
			
				SplineReplicatorList[i].Item.GetComponent<ItemDrag>().TargetTransform.position = SplineReplicatorList[i].DuplicateRangeMidPoint();
			
				SplineReplicatorList[i].SetAllPointParent(transform);	
						
				priorRatio += ratioSplit;
			}	
		}
	}
}
