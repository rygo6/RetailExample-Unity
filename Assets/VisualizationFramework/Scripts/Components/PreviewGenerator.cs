using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EC
{
	public class PreviewGenerator : MonoBehaviour
	{
		private void Start()
		{
			string path = EditorApplication.currentScene;
			path = path.Replace("unity", "png");
			SaveTransparentScreenshotToFile(path);
		}

		private Color[] InvertAlpha(Color[] pixels)
		{
			for (int i = 0; i < pixels.Length; ++i)
			{
				if (pixels[i].a == 0)
				{
					pixels[i].a = 1;
				}
				else if (pixels[i].a == 1)
				{
					pixels[i].a = 0;
				}
			}
			return pixels;
		}
			
		private int CoordinateToGridIndex(int x, int y, int xMax, int yMax)
		{       
			if (x >= xMax || y >= yMax || x < 0 || y < 0)
			{
				return -1;
			}
			return ((y * xMax) + x);
		}

		private Rect CropRect(Texture2D texture)
		{
			int top = 0;
			int bottom = int.MaxValue;
			int right = 0;
			int left = int.MaxValue;
			Color[] pixels = texture.GetPixels();
			for (int y = 0; y < texture.height; ++y)
			{
				for (int x = 0; x < texture.width; ++x)
				{
					int index = CoordinateToGridIndex(x, y, texture.width, texture.height);

					if (pixels[index].a > 0)
					{
						if (y > top)
						{
							top = y;
						}
						if (y < bottom)
						{
							bottom = y;
						}
						if (x > right)
						{
							right = x;
						}
						if (x < left)
						{
							left = x;
						}
					}
				}
			}
			return new Rect(left, bottom, right - left, top - bottom);
		}

		private void SaveTransparentScreenshotToFile(string fileName)
		{
			int resWidth = 1024;
			int resHeight = 1024;

			Camera camera = Camera.main;
			RenderTexture rt = new RenderTexture(resWidth, resHeight, 32);

			//yes, for some reason you set this to SolidColor black if you want a transparent BG
			camera.clearFlags = CameraClearFlags.SolidColor;
			camera.backgroundColor = Color.black;
			camera.targetTexture = rt;
			camera.Render();

			RenderTexture.active = rt;

			Texture2D texture = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
			texture.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);

			camera.targetTexture = null;
			RenderTexture.active = null; 

			texture.SetPixels(InvertAlpha(texture.GetPixels()));

			Rect cropRect = CropRect(texture);
			Texture2D cropTexture = new Texture2D((int)cropRect.width, (int)cropRect.height, TextureFormat.ARGB32, false);
			Color[] cropPixels = texture.GetPixels((int)cropRect.x, (int)cropRect.y, (int)cropRect.width, (int)cropRect.height);
			cropTexture.SetPixels(cropPixels);	

			byte[] bytes = cropTexture.EncodeToPNG();
			System.IO.File.Delete(fileName);
			System.IO.File.WriteAllBytes(fileName, bytes);
		}
	}
}