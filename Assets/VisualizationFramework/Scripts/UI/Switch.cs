using UnityEngine;
using System.Collections;

namespace UnityEngine.EventSystems
{
	public static class Switch
	{
		public readonly static GameObject[] GameObject = new GameObject[10];

		public readonly static int[] Phase = new int[10];

		public static void SwitchGameObject(GameObject gameObject, PointerEventData data)
		{
			GameObject[data.pointerId.NegativeToPositive()] = gameObject;
			Phase[data.pointerId.NegativeToPositive()] = 1;
		}
	}
}