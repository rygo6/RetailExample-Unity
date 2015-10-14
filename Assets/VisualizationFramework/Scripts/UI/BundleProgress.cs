using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EC
{
	public class BundleProgress : MonoBehaviour 
	{
		[SerializeField]
		private string _bundleName;

		private WWW _www;

		private BundleStore _bundleStore;

		private Vector2 _initialSizeDelta;

		[SerializeField]
		private Button _button;

		private void Awake()
		{
			_button.interactable = false;
			_initialSizeDelta = GetComponent<RectTransform>().sizeDelta;
			_bundleStore = Persistent.GetComponent<BundleStore>();
			StartCoroutine(UpdateProgress());
		}

		private IEnumerator UpdateProgress() 
		{
			while (_www == null)
			{
				_www = _bundleStore.GetWWW(_bundleName);
				yield return null;
			}
					
			while (!_www.isDone)
			{
				float x = Mathf.Lerp(_initialSizeDelta.x, 0f, _www.progress);
				GetComponent<RectTransform>().sizeDelta = new Vector2(x, _initialSizeDelta.y);
				yield return null;
			}

			_button.interactable = true;
			Destroy(gameObject);
		}
	}
}