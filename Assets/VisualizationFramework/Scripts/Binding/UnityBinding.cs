using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class UnityBinding
{

	[DllImport("__Internal")]
	private static extern string _DocumentsDirectory();
	
	public static string DocumentsDirectory()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
		{
			string path = _DocumentsDirectory();
			path = path.Substring(7,path.Length-7);
			return path;
		}
		return string.Empty;
	}
	
	[DllImport("__Internal")]
	private static extern string _CachesDirectory();
	
	public static string CachesDirectory()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
		{
			string path = _CachesDirectory();
			path = path.Substring(7,path.Length-7);
			return path;
		}
		return string.Empty;
	}
	

	//this is used primarily for opening other iOS apps through url links
	//vrlink uses this
	[DllImport("__Internal")]	
	private static extern void _OpenURL(string url);	
	
	public static void OpenURL(string url)
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_OpenURL(url);
	}		

	
	[DllImport("__Internal")]
	private static extern void _ShowWebPage( string url, bool showControls );
	
	// Opens a web view with the url (Url can either be a resource on the web or a local file) and optional controls (back, forward, copy, open in Safari)
	public static void showWebPage( string url, bool showControls )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_ShowWebPage( url, showControls );
	}


	[DllImport("__Internal")]
	private static extern void _OpenReader(string name);
	
	public static void OpenReader(string name)
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_OpenReader(name);
	}	

	[DllImport("__Internal")]
	private static extern void _ShowStatusBar();
	
	public static void ShowStatusBar()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_ShowStatusBar();
	}	
	
	[DllImport("__Internal")]
	private static extern void _HideStatusBar();
	
	public static void HideStatusBar()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_HideStatusBar();
	}	

	[DllImport("__Internal")]
	private static extern void _ShowActivityView();
	
	public static void ShowActivityView()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_ShowActivityView();
	}	
	
	[DllImport("__Internal")]
	private static extern void _HideActivityView();
	
	public static void HideActivityView()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_HideActivityView();
	}		


	[DllImport("__Internal")]
	private static extern void _Loaded();
	
	public static void Loaded()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_Loaded();
	}		

	[DllImport("__Internal")]	
	private static extern void _CloseUnity(string message);
	
	public static void CloseUnity(string message)
	{
		Debug.Log("Close Unity with message: "+message);
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_CloseUnity(message);
	}	

	[DllImport("__Internal")]
	private static extern void _PauseUnity();
	
	public static void PauseUnity()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_PauseUnity();
	}	


	[DllImport("__Internal")]
	private static extern string _GetLocalizedString( string key, string defaultValue );
	
	// Uses the Xcode Localizable.strings system to get a localized version of the given string
	public static string GetLocalizedString( string key, string defaultValue )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _GetLocalizedString( key, defaultValue );
		return string.Empty;
	}


	[DllImport("__Internal")]
	private static extern void _SaveImageToPhotoAlbum( string filePath );
	
	// Writes the given image to the users photo album
	public static void SaveImageToPhotoAlbum( string filePath )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_SaveImageToPhotoAlbum( filePath );
	}

	public static IEnumerator TakeScreenshotSaveToPhotoAlbum( MonoBehaviour monoBehaviour )
	{
		yield return monoBehaviour.StartCoroutine( TakeScreenShot( monoBehaviour, Application.persistentDataPath+"/screenshot.png", path =>
			{
				SaveImageToPhotoAlbum(path);
			}) );	
	}
	

#region Screenshot
	public static bool takingScreenShot;	

	public static IEnumerator TakeScreenShot( MonoBehaviour monoBehaviour, string filename )
	{
		return TakeScreenShot( monoBehaviour, filename, null );
	}

	public static IEnumerator TakeScreenShot( MonoBehaviour monoBehaviour, string filename, Action<string> completionHandler )
	{
		yield return monoBehaviour.StartCoroutine( GetScreenShotTexture( tex =>
		                                                                                     {
			string path = Path.Combine( Application.persistentDataPath, filename );
			File.WriteAllBytes( path, tex.EncodeToPNG() );
			
			if( completionHandler != null )
				completionHandler( path );
		}) );
	}

	public static IEnumerator GetScreenShotTexture( Action<Texture2D> completionHandler )
	{
		Debug.Log("Taking Screenshot");
		takingScreenShot = true;
		yield return new WaitForEndOfFrame();
		
		Texture2D tex = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
		tex.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
		tex.Apply();

		takingScreenShot = false;
		Debug.Log("Screenshot Taken");
		completionHandler( tex );
	}
#endregion


#region Mail Composer
	[DllImport("__Internal")]
	private static extern void _ShowMailComposer( string toAddress, string subject, string body, bool isHTML );
	
	// Opens the mail composer with the given information
	public static void ShowMailComposer( string toAddress, string subject, string body, bool isHTML )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			ShowMailComposer( toAddress, subject, body, isHTML );
	}
	
	
	// Opens the mail composer with a screenshot of the current state of the game attached
	public static IEnumerator ShowMailComposerWithScreenshot( MonoBehaviour monoBehaviour, string toAddress, string subject, string body, bool isHTML )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
		{
			yield return monoBehaviour.StartCoroutine( GetScreenShotTexture( tex =>
			                                                                                     {
				byte[] bytes = tex.EncodeToPNG();
				ShowMailComposerWithAttachment( bytes, "image/png", "screenshot.png", toAddress, subject, body, isHTML );
			}) );
		}
	}

	// Opens the mail composer with the given attachment file. The attachment data must be stored in a file on disk.
	public static void ShowMailComposerWithAttachment( string filePathToAttachment, string attachmentMimeType, string attachmentFilename, string toAddress, string subject, string body, bool isHTML )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
		{
			if( !filePathToAttachment.StartsWith( "/" ) )
			{
				Debug.Log( "file path passed to showMailComposerWithAttachment is not a legit path: " + filePathToAttachment + ". Be sure to test your paths with File.Exists before using them!" );
				return;
			}
			
			if( !File.Exists( filePathToAttachment ) )
			{
				Debug.Log( "file path passed to showMailComposerWithAttachment does not exist: " + filePathToAttachment + ". Be sure to test your paths with File.Exists before using them!" );
				return;
			}
			
			byte[] bytes = File.ReadAllBytes( filePathToAttachment );
			ShowMailComposerWithAttachment( bytes, attachmentMimeType, attachmentFilename, toAddress, subject, body, isHTML );
		}
	}
	
	
	[DllImport("__Internal")]
	private static extern void _ShowMailComposerWithRawAttachment( byte[] bytes, int length, string attachmentMimeType, string attachmentFilename, string toAddress, string subject, string body, bool isHTML );
	
	// Opens the mail composer with the given attachment
	public static void ShowMailComposerWithAttachment( byte[] attachmentData, string attachmentMimeType, string attachmentFilename, string toAddress, string subject, string body, bool isHTML )
	{
		Debug.Log("Showing Mail Composer");
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_ShowMailComposerWithRawAttachment( attachmentData, attachmentData.Length, attachmentMimeType, attachmentFilename, toAddress, subject, body, isHTML );
	}
#endregion


}
