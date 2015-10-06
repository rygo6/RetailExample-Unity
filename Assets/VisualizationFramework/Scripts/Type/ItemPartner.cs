using UnityEngine;
using System.Collections;

namespace EC.Visualization
{
	[System.Serializable]
	public class ItemPartner
	{
		public string PrefabName { get; set; }
		public string[] TagArray { get; set; }
		public Item PartnerItemRaycast { get; set; }
	}
}