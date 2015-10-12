using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EC.Visualization
{
	public class ItemColor : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
	{	
		private Texture _initialBlendTexture;
	
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
			if (data.pointerPress != null)
			{	
				Item dragItem = data.pointerPress.GetComponent<Item>();
				if (dragItem != null && dragItem.HasTag("Color"))
				{
					StartCoroutine(dragItem.DestroyItemCoroutine());
				}
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
				Item dragItem = data.pointerPress.GetComponent<Item>();
				if (dragItem != null && dragItem.HasTag("Color"))
				{	
					GetComponent<Item>().SetBlendMaterial(_initialBlendTexture);
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
				Item dragItem = data.pointerPress.GetComponent<Item>();
				if (dragItem != null && dragItem.HasTag("Color"))
				{	
					Item item = GetComponent<Item>();
					_initialBlendTexture = item.BlendMaterialArray[0].GetTexture("_Blend");
					item.SetBlendMaterial(dragItem.MaterialArray[0].mainTexture);
				}
			}
		}
	}
}