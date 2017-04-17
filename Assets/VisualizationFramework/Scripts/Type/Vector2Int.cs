using UnityEngine;

[System.Serializable]
public struct Vector2Int
{
	public int x { get { return _x; } set { _x = value; } }
	[SerializeField]
	private int _x;
	
	public int y { get { return _y; } set { _y = value; } }
	[SerializeField]
	private int _y;
	
	public Vector2Int(int x, int y)
	{
		_x = x;
		_y = y;
	}

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
}