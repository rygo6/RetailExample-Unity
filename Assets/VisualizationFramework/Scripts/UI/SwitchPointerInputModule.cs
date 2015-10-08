using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

namespace UnityEngine.EventSystems
{
	public abstract class SwitchPointerInputModule : PointerInputModule
	{
		private readonly MouseState m_MouseState = new MouseState();

        protected PointerEventData GetTouchPointerEventData(Touch input, out bool pressed, out bool released)
        {
            PointerEventData pointerData;
            var created = GetPointerData(input.fingerId, out pointerData, true);

            pointerData.Reset();

            pressed = created || (input.phase == TouchPhase.Began);
            released = (input.phase == TouchPhase.Canceled) || (input.phase == TouchPhase.Ended);


			//Added
			if (Switch.Phase[input.fingerId] == 1)
			{
				released = true;
				pressed = false;
				Switch.Phase[input.fingerId] = 2;
			}
			else if (Switch.Phase[input.fingerId] == 2)	
			{
				Switch.Phase[input.fingerId] = 3;
				released = false;
				pressed = true;		
			}				
			//End added


            if (created)
                pointerData.position = input.position;

            if (pressed)
                pointerData.delta = Vector2.zero;
            else
                pointerData.delta = input.position - pointerData.position;

            pointerData.position = input.position;

            pointerData.button = PointerEventData.InputButton.Left;

			Debug.Log(input.fingerId);

			if (Switch.Phase[input.fingerId] == 3)	
			{
				Debug.Log("HERE");
				Switch.Phase[input.fingerId] = 0;
				RaycastResult result = new RaycastResult();
				result.gameObject = Switch.GameObject[input.fingerId];
				m_RaycastResultCache.Add(result);
				Switch.GameObject[input.fingerId] = null;
			}
			else
			{	
				eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
			}


				
			var raycast = FindFirstRaycast(m_RaycastResultCache);
			pointerData.pointerCurrentRaycast = raycast;
			m_RaycastResultCache.Clear();
            return pointerData;
        }

        protected virtual MouseState GetMousePointerEventData(int id)
        {
            // Populate the left button...
            PointerEventData leftData;
            var created = GetPointerData(kMouseLeftId, out leftData, true);

            leftData.Reset();

            if (created)
                leftData.position = Input.mousePosition;

            Vector2 pos = Input.mousePosition;
            leftData.delta = pos - leftData.position;
            leftData.position = pos;
            leftData.scrollDelta = Input.mouseScrollDelta;
            leftData.button = PointerEventData.InputButton.Left;



			if (Switch.Phase[0] == 2)	
			{
				RaycastResult result = new RaycastResult();
				result.gameObject = Switch.GameObject[0];
				Switch.GameObject[0] = null;
				m_RaycastResultCache.Add(result);
			}
			else
			{	
				eventSystem.RaycastAll(leftData, m_RaycastResultCache);
			}	



            var raycast = FindFirstRaycast(m_RaycastResultCache);
            leftData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();

            // copy the apropriate data into right and middle slots
            PointerEventData rightData;
            GetPointerData(kMouseRightId, out rightData, true);
            CopyFromTo(leftData, rightData);
            rightData.button = PointerEventData.InputButton.Right;

            PointerEventData middleData;
            GetPointerData(kMouseMiddleId, out middleData, true);
            CopyFromTo(leftData, middleData);
            middleData.button = PointerEventData.InputButton.Middle;



			m_MouseState.SetButtonState(PointerEventData.InputButton.Left, SwitchStateForMouseButton(0), leftData);
			m_MouseState.SetButtonState(PointerEventData.InputButton.Right, SwitchStateForMouseButton(1), rightData);
			m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, SwitchStateForMouseButton(2), middleData);



            return m_MouseState;
        }

		private PointerEventData.FramePressState SwitchStateForMouseButton(int id)
		{
			PointerEventData.FramePressState buttonState = StateForMouseButton(id);
			if (Switch.Phase[id] == 1)
			{
				buttonState = PointerEventData.FramePressState.Released;
				Switch.Phase[id] = 2;
			}
			else if (Switch.Phase[id] == 2)	
			{	
				buttonState = PointerEventData.FramePressState.Pressed;
				Switch.Phase[id] = 0;			
			}	
			return buttonState;
		}	
    }
}
