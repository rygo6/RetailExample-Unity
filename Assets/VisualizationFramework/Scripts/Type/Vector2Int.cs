using UnityEngine;

[System.Serializable]
public struct Vector2Int
{

	#region Properties

	public int x { get { return _x; } set { _x = value; } }
	[SerializeField]
	private int _x;
	
	public int y { get { return _y; } set { _y = value; } }
	[SerializeField]
	private int _y;

	#endregion

	#region LifeCycle
	
	public Vector2Int(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	#endregion

	#region object

	public int Multiplied()
	{
		return x * y;	
	}
	
	public override string ToString()
	{
		return x.ToString() + ", " + y.ToString();
	}
	
	public static Vector2Int operator +(Vector2Int a, Vector2Int b)
	{
		return new Vector2Int(a.x + b.x, a.y + b.y);
	}

	public static Vector2Int operator /(Vector2Int a, Vector2Int b)
	{
		return new Vector2Int(a.x / b.x, a.y / b.y);
	}

	public static Vector2Int operator /(Vector2Int a, int b)
	{
		return new Vector2Int(a.x / b, a.y / b);
	}

	#endregion
	
}