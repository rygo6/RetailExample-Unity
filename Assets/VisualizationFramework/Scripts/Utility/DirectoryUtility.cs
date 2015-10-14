using UnityEngine;
using System.Collections;
using System.IO;

namespace EC
{
	static public class DirectoryUtility 
	{
		static public string AssetBundles()
		{
			switch ( Application.platform )
			{
			default:
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:	
				return Application.dataPath + "/../AssetBundles/";			
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.IPhonePlayer:		
			case RuntimePlatform.LinuxPlayer:
				return Application.dataPath + "/AssetBundles/";		
			}	
		}	

		static public string ExternalAssets()
		{
			switch ( Application.platform )
			{
			default:
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:	
				return Application.dataPath + "/../ExternalAssets/";			
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.IPhonePlayer:		
			case RuntimePlatform.LinuxPlayer:
				return Application.dataPath + "/ExternalAssets/";		
			}	
		}	
		
		static public string Documents()
		{
			switch ( Application.platform )
			{
			default:
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:	
				return Application.dataPath + "/../Documents/";			
			case RuntimePlatform.IPhonePlayer:
				return Application.persistentDataPath + "/";
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.LinuxPlayer:
				return Application.dataPath + "/Documents/";
				
			}	
		}    
	}
}
