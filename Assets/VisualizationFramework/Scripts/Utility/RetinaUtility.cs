using UnityEngine;
using System.Collections;

namespace EC
{
	/// <summary>
	/// Retina utility.
	/// Class of Utility functions to aid with switching things
	/// between standard resolution and retina.
	/// </summary>
	public class RetinaUtility
	{
		static public string RPath(string path)
		{
			if (Screen.width > 1024)
			{
				path += "@2x";	
			}
			return path;
		}
	
		static public Rect RetinaToStandardRect(float left, float top, float width, float height)
		{
			if (Screen.width == 1024)
			{
				left /= 2;	
				top /= 2;
				width /= 2;
				height /= 2;
			}
			return new Rect(left, top, width, height);		
		}
	
		static public Rect RetinaToStandardRect(Rect rect)
		{
			if (Screen.width == 1024)
			{
				rect.x /= 2;	
				rect.y /= 2;
				rect.width /= 2;
				rect.height /= 2;
			}
			return rect;
		}
	
		static public Vector3 RVector3(Vector3 vector3)
		{
			if (Screen.width > 1024)
			{
				vector3.x *= 2;	
				vector3.y *= 2;
				vector3.z *= 2;
			}
			return vector3;		
		}
	
		static public Rect RRect(float left, float top, float width, float height)
		{
			if (Screen.width > 1024)
			{
				left *= 2;	
				top *= 2;
				width *= 2;
				height *= 2;
			}
			return new Rect(left, top, width, height);		
		}
	
		static public Rect RRect(Rect rect)
		{
			if (Screen.width > 1024)
			{
				rect.x *= 2;
				rect.y *= 2;
				rect.width *= 2;
				rect.height *= 2;
			}
			return rect;
		}
	
		static public int RetinaToStandardInt(int number)
		{
			if (Screen.width == 1024)
			{
				number /= 2;
			}
			return number;
		}
	
		static public int RInt(int number)
		{
			if (Screen.width > 1024)
			{
				number *= 2;
			}
			return number;
		}
	
		static public float RFloat(float number)
		{
			if (Screen.width > 1024)
			{
				number *= 2;
			}
			return number;
		}
	
		static public float InverseRFloat(float number)
		{
			if (Screen.width > 1024)
			{
				number /= 2;
			}
			return number;
		}
	}
}