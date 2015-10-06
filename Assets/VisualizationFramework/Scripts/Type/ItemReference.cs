using UnityEngine;
using System.Collections;

namespace EC.Visualization
{
	public class ItemReference
	{
		public string Name { get; set; }
		public string PrefabName { get; set; }
		public string PreviewName { get; set; }
		public Sprite PreviewSprite { get; set; }
		public string[] TagArray { get; set; }
		public AssetBundle AssetBundle { get; set; }
	}
}
