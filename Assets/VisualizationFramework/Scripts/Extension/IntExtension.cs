using UnityEngine;
using System.Collections;

static public class IntExtension 
{
	static public int NegativeToZero(this int value)
	{
		if (value < 0)
		{
			value = 0;
		}
		return value;
	}

	static public int NegativeToPositive(this int value)
	{
		if (value < 0)
		{
			value *= -1;
			value--;
		}
		return value;
	}
}
