using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace EC
{
	/// <summary>
	/// NoOrbitEvent
	/// Place on a UI Component in order to have it disable the OrbitManager.
	/// </summary>
	public class NoOrbitEvent : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
	{
		public void OnPointerDown(PointerEventData data)
		{	
//			Orbit.Instance.InputNoOrbit[data.pointerId.NoNegative()] = true;
		}

		public void OnPointerUp(PointerEventData data)
		{
//			Orbit.Instance.InputNoOrbit[data.pointerId.NoNegative()] = false;			
		}
	}
}