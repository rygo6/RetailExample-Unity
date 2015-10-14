using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace EC.Visualization
{
	static public class ItemUtility
	{
		static public void StateSwitch(PointerEventData data, ItemState state,
		                               System.Action<PointerEventData> attachedAction,  
		                               System.Action<PointerEventData> attachedHighlightedAction,  
		                               System.Action<PointerEventData> draggingAction,  
		                               System.Action<PointerEventData> floatingAction,
		                               System.Action<PointerEventData> instantiateAction,
		                               System.Action<PointerEventData> noInstantiateAction)
		{
			switch (state)
			{
			case ItemState.Attached:
				if (attachedAction != null)
				{
					attachedAction(data);
				}
				break;
			case ItemState.AttachedHighlighted:
				if (attachedHighlightedAction != null)
				{
					attachedHighlightedAction(data);
				}
				break;				
			case ItemState.Dragging:
				if (draggingAction != null)
				{
					draggingAction(data);
				}
				break;		
			case ItemState.Floating:
				if (floatingAction != null)
				{
					floatingAction(data);
				}
				break;					
			case ItemState.Instantiate:
				if (instantiateAction != null)
				{
					instantiateAction(data);
				}
				break;	
			case ItemState.NoInstantiate:
				if (noInstantiateAction != null)
				{
					noInstantiateAction(data);
				}
				break;							
			}			
		}

		static public bool TestTagArrays(string[] firstArray, string[] secondArray)
		{
			if (firstArray == null || secondArray == null)
			{
				return false;
			}	
			for (int i = 0; i < firstArray.Length; ++i)
			{
				for (int o = 0; o < secondArray.Length; ++o)
				{
					if (firstArray[i] == secondArray[o])
					{
						return true;
					}
				}
			}
			return false;
		}

		static public string TickToString()
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