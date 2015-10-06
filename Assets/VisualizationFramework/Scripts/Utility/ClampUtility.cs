using UnityEngine;
using System.Collections;

namespace EC
{
	static public class ClampUtility
	{
		static public float ClampAngleX(float angle, float min, float max)
		{
			float minMaxDelta = Mathf.DeltaAngle(min, max);
			if (minMaxDelta < 0f)
			{
				minMaxDelta += 360f;
			}
			float minDeltaAngle = Mathf.DeltaAngle(angle, min);
			if (minDeltaAngle > 360f - minMaxDelta)
			{
				minDeltaAngle -= 360f;
			}	
			if (minDeltaAngle < -minMaxDelta)
			{
				angle = max;
			}
			else if (minDeltaAngle > 0f)
			{
				float minMaxHalfDiff = Mathf.DeltaAngle(min, max) / 2f;
				if (minMaxHalfDiff < 0f)
				{
					minMaxHalfDiff *= -1;
				}
				if (minDeltaAngle < minMaxHalfDiff)
				{
					angle = min;	
				}
				else if (minDeltaAngle > minMaxHalfDiff)
				{
					angle = max;
				}
			}
			return angle;
		}
    
		static public float ClampAngleY(float angle, float min, float max)
		{
			if (angle < -180)
			{
				angle += 360;
			}
			if (angle > 180)
			{
				angle -= 360;
			}
			return Mathf.Clamp(angle, min, max);
		}
	}
}
