using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace EC
{
	public class AssetBundleBuild
	{
		[MenuItem("Assets/Build Local Bundles")]	
		static public void BuildBundlesLocal()
		{	
			BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle;
			string path = DirectoryUtility.AssetBundles() + "/Local";
			Directory.CreateDirectory(path);
			BuildPipeline.BuildAssetBundles(path, assetBundleOptions, target);
		}

		[MenuItem("Assets/Build WebGL Bundles")]	
		static public void BuildBundlesWebGL()
		{
			BuildBundles(BuildTarget.WebGL);
		}

		[MenuItem("Assets/Build Android Bundles")]	
		static public void BuildBundlesAndroid()
		{
			BuildBundles(BuildTarget.WebGL);
		}

		[MenuItem("Assets/Build iOS Bundles")]	
		static public void BuildBundlesiOS()
		{
			BuildBundles(BuildTarget.iOS);
		}

		static private void BuildBundles(BuildTarget target)
		{
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle;
			string path = DirectoryUtility.AssetBundles() + "/" + target.ToString();
			Directory.CreateDirectory(path);
			BuildPipeline.BuildAssetBundles(path, assetBundleOptions, target);
		}
		
	}
}