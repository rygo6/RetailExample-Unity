using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;
using System.Collections;

public class GetPreview
{

	[MenuItem("Assets/Get Preview")]	
	public static Texture2D TakeScreenShot()
	{
		return SaveScreenshotToFile (Application.dataPath+"/test.png");
	}

	public static Texture2D Screenshot() 
	{
		int resWidth = Camera.main.pixelWidth;
		int resHeight = Camera.main.pixelHeight;
		Camera camera = Camera.main;
		RenderTexture rt = new RenderTexture(resWidth, resHeight, 32);
		rt.DiscardContents ();
		camera.targetTexture = rt;
		Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
		camera.clearFlags = CameraClearFlags.Depth;
		camera.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		screenShot.Apply ();
		camera.targetTexture = null;
		RenderTexture.active = null; // JC: added to avoid errors
		return screenShot;
	}

	public static Texture2D SaveScreenshotToFile(string fileName)
	{
		Texture2D screenShot = Screenshot ();

		var pixels = screenShot.GetPixels();

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

		screenShot.SetPixels (pixels);


		byte[] bytes = screenShot.EncodeToPNG();
		System.IO.File.WriteAllBytes(fileName, bytes);
		return screenShot;
	}


//	static public void Get()
//	{
//		var tex = new Texture2D(512, 512, TextureFormat.ARGB32, false);
//		tex.ReadPixels(new Rect(0, 0, 512, 512), 0, 0, false);
//
//		var pixels = tex.GetPixels();
//
//		for (int i = 0; i < pixels.Length; ++i)
//		{
//			if (pixels[i].a == 0)
//			{
//				pixels[i].a = 1;
//			}
//			else if (pixels[i].a == 1)
//			{
//				pixels[i].a = 0;
//			}
//		}
//
//		tex.SetPixels(pixels);
//
//		byte[] png = tex.EncodeToPNG();
//		File.WriteAllBytes(Application.dataPath + "/" + Selection.activeGameObject + ".png", png);
//
//	}
}
