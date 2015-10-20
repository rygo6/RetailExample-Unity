using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EC
{
	public class BundleStore : MonoBehaviour
	{
		[SerializeField]
		private string _url;

		[SerializeField]
		private bool _local;

		private string _target;

		private AssetBundleManifest _manifest;

		private readonly List<AssetBundle> AssetBundleList = new List<AssetBundle>();

		private readonly Dictionary<string, WWW> WWWDictionary = new Dictionary<string, WWW>();

		private void Awake()
		{
			#if UNITY_EDITOR
			_target = UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString();
			#else
			_target = Application.platform.ToString();
			#endif

			if (_currentLevelAssetBundle != null)
			{
				_currentLevelAssetBundle.Unload(false);
				_currentLevelAssetBundle = null;
			}	

			if (_local)
			{
				_target = "Local";
				_url = "file://" + DirectoryUtility.AssetBundles() + _target + "/";
			}
			else
			{
				_url += _target + "/";
			}

			Debug.Log("AssetBundle URL: " + _url);

			StartCoroutine(LoadManifest());

			StartCoroutine(LoadBundleWWW("merchandise"));
			StartCoroutine(LoadBundleWWW("attachments"));
			StartCoroutine(LoadBundleWWW("fixtures"));
			StartCoroutine(LoadBundleWWW("colors"));
		}
	
		private void OnDestroy()
		{
			UnloadAllBundles();
		}

		private string GetFirstAssetName(AssetBundle bundle)
		{
			return bundle.GetAllAssetNames()[0];
		}

		private IEnumerator LoadManifest()
		{
			#if UNITY_EDITOR
			string target = UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString();
			#else
			string target = Application.platform.ToString();
			#endif
			if (_local)
			{
				target = "Local";
			}
			WWW www = new WWW(_url + target);
			yield return www;
			AssetBundle bundle = www.assetBundle;
			const string manifestName = "assetbundlemanifest";
			_manifest = bundle.LoadAsset(manifestName) as AssetBundleManifest;
			www.Dispose();
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

		private IEnumerator LoadBundleWWW(string name)
		{
			Debug.Log("Loading Bundle: " + name);

			while (_manifest == null)
			{
				yield return null;
			}

			WWW getWWW;
			WWWDictionary.TryGetValue(name, out getWWW);
			if (getWWW == null)
			{
				WWW www = WWW.LoadFromCacheOrDownload(_url + name, _manifest.GetAssetBundleHash(name));
				WWWDictionary.Add(name, www);
				yield return www;
				www.assetBundle.name = name;
				AssetBundleList.Add(www.assetBundle);
			}
			yield return null;
		}

		public AssetBundle LoadBundle(string name)
		{
			Debug.Log("Loading Bundle: " + name);
			AssetBundle assetBundle = GetBundle(name);
			if (assetBundle == null)
			{
				LoadBundleWWW(name);
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
