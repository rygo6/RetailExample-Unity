using UnityEngine;
using System.Collections;

namespace EC.Visualization
{
	public abstract class Replicator : MonoBehaviour
	{
		protected Transform RootTransform { get; private set; }
	
		protected Mesh[] MeshArray { get; private set; }
	
		protected MeshRenderer[] MeshRendererArray { get; private set; }
	
		protected Transform[] MeshTransformArray { get; private set; }

		protected Vector3 Spacing { get; private set; }
	
		/// <summary>
		/// The amount the entire replicator should be offset.
		/// </summary>
		protected Vector3 Offset { get; private set; }

		/// <summary>
		/// One may feed the replicator an Item. 
		/// But one could also fill in the other fields manually
		/// and use the replicator that way.
		/// </summary>
		/// <value>The item.</value>
		public virtual Item Item
		{ 
			get
			{ 
				return _item; 
			} 
			set
			{
				_item = value;
				RootTransform = _item.transform;
				MeshArray = _item.MeshArray;
				MeshRendererArray = _item.MeshRendererArray;
				MeshTransformArray = _item.MeshTransformArray;
				Offset = _item.GetComponent<ItemDrag>().AttachPointArray[0].transform.localPosition;
				Spacing = _item.InitialColliderSizeArray[0];
			}	
		}
		private Item _item;
	}
}
