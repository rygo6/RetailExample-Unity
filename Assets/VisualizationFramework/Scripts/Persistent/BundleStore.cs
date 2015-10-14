using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EC
{
	public class BundleStore : MonoBehaviour
	{
		[SerializeField]
		private string _url;

		private readonly List<AssetBundle> AssetBundleList = new List<AssetBundle>();

		private readonly Dictionary<string, WWW> WWWDictionary = new Dictionary<string, WWW>();

		private void Awake()
		{
			if (_currentLevelAssetBundle != null)
			{
				_currentLevelAssetBundle.Unload(false);
				_currentLevelAssetBundle = null;
			}	

			StartCoroutine(LoadBundleWWW(DirectoryUtility.ExternalAssets() + "merchandise"));
			StartCoroutine(LoadBundleWWW(DirectoryUtility.ExternalAssets() + "attachments"));
			StartCoroutine(LoadBundleWWW(DirectoryUtility.ExternalAssets() + "fixtures"));
			StartCoroutine(LoadBundleWWW(DirectoryUtility.ExternalAssets() + "colors"));
		}
	
		public void OnDestroy()
		{
			UnloadAllBundles();
		}

		public WWW GetWWW(string name)
		{
			WWW getWWW;
			WWWDictionary.TryGetValue(name, out getWWW);
			return getWWW;
		}

		public AssetBundle GetBundle(string name)
		{
			for (int i = 0; i < AssetBundleList.Count; ++i)
			{
				if (name == AssetBundleList[i].name)
				{
					return AssetBundleList[i];
				}
			}
			return null;
		}

		public IEnumerator LoadBundleWWW(string path)
		{
			Debug.Log("Loading Bundle: " + path);
			string name = System.IO.Path.GetFileNameWithoutExtension(path);
			AssetBundle assetBundle = GetBundle(name);
			WWW getWWW;
			WWWDictionary.TryGetValue(name, out getWWW);
			if (assetBundle == null && getWWW == null)
			{
				WWW www = WWW.LoadFromCacheOrDownload("file://" + path, 0);
				WWWDictionary.Add(name, www);
				yield return www;
				www.assetBundle.name = name;
				AssetBundleList.Add(www.assetBundle);
			}
			else
			{
				yield return null;
			}
		}

		public AssetBundle LoadBundle(string path)
		{
			Debug.Log("Loading Bundle: " + path);
			string name = System.IO.Path.GetFileNameWithoutExtension(path);
			AssetBundle assetBundle = GetBundle(name);
			if (assetBundle == null)
			{
				LoadBundleWWW(path);
				return null;
			}
			else
			{
				return assetBundle;
			}
		}

		public void UnloadAllBundles()
		{
			for (int i = 0; i < AssetBundleList.Count; ++i)
			{
				AssetBundleList[i].Unload(false);
			}
			AssetBundleList.Clear();
		}
	
		public void LoadLevelAssetBundle(string level)
		{
			string path = DirectoryUtility.ExternalAssets() + level + ".assetBundle";
			Debug.Log("LoadLevelAssetBundle: " + path);
			_currentLevelAssetBundle = AssetBundle.CreateFromFile(path);
			if (_currentLevelAssetBundle != null && Application.CanStreamedLevelBeLoaded(level))
			{
				UnloadAllBundles();
				Application.LoadLevel(level);	
			}
			else
			{
				Debug.Log("AssetBundle Not Found: " + path);
			}
		}
		static private AssetBundle _currentLevelAssetBundle;
	}
}
