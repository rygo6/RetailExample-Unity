using UnityEngine;
using System.Collections;

namespace EC
{
	[RequireComponent(typeof(GUITexture))]
	public class ViewSingleton : Singleton<ViewSingleton>
	{
		public View mainView
		{ 
			get
			{ 
				if (_mainView == null)
				{
					GameObject mainViewGameObject = GameObject.Find("MainView");
					if (mainViewGameObject == null)
					{
						Debug.LogError(this.name + " could not fine MainView. Drag in MainView prefab.");
					}
					else
					{
						_mainView = mainViewGameObject.GetComponent<View>();
						if (_mainView == null)
						{
							Debug.LogError(mainViewGameObject + " doesn't have view component.");
						}
					}
				}
				return _mainView; 
			} 
		}
		private View _mainView;
	
		private View currentView { get { return _currentView; } set { _currentView = value; } }
		private View _currentView;
	
		private RenderTexture fadeRenderTexture
		{
			get
			{
				if (_fadeRenderTexture == null)
				{
					_fadeRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				}
				return _fadeRenderTexture;
			}
		}
		private RenderTexture _fadeRenderTexture;

		private void Awake()
		{
			Application.targetFrameRate = 60;
	
			//this is done to ensure that the GUITexture is positioned on screen correctly
			transform.position = new Vector3(0.5f, 0.5f, 0.0f);
		}

		//	public void FadeToView( View view )
		//	{
		//		view.componentCache.camera.enabled = true;
		//		view.componentCache.camera.targetTexture = fadeRenderTexture;
		//
		//		GoProxyProp<float> goType = new GoProxyProp<float>( 0.0f );
		//		GoTween tween = Go.to( goType, 1.0f, new GoTweenConfig()
		//		                      .floatProp( "value", 1.0f, false)
		//		                      .setEaseType( GoEaseType.SineOut ) );
		//
		//        tween.setOnInitHandler(
		//			c =>
		//			{
		//				_currentView = null;
		//				componentCache.guiTexture.enabled = true;
		//				componentCache.guiTexture.texture = fadeRenderTexture;
		//			}
		//		);
		//
		//        tween.setOnUpdateHandler(
		//        	c =>
		//        	{
		//				componentCache.guiTexture.color = new Color( 0.5f, 0.5f, 0.5f, goType.value );
		//        	}
		//        );
		//
		//		tween.setOnCompleteHandler(
		//			c =>
		//			{
		//				_currentView = view;
		//				mainView.transform.position = view.transform.position;
		//				mainView.transform.rotation = view.transform.rotation;
		//				view.componentCache.camera.enabled = true;
		//				view.componentCache.camera.targetTexture = null;
		//				componentCache.guiTexture.enabled = false;
		//				currentView	= view;
		//			}
		//		);
		//	}
		//
		//	public void TweenToView( View view )
		//	{
		//		GoTween tween = Go.to( mainView.transform, 4.0f, new GoTweenConfig()
		//				.position( view.transform.position )
		//		        .setEaseType( GoEaseType.SineOut ) );
		//
		//		Go.to( mainView.transform, 4.0f, new GoTweenConfig()
		//		        .rotation( view.transform.rotation )
		//		        .setEaseType( GoEaseType.SineOut ) );
		//
		//		tween.setOnInitHandler(
		//			c =>
		//			{
		//				_currentView = null;
		//			}
		//		);
		//
		//		tween.setOnCompleteHandler(
		//			c =>
		//		    {
		//				_currentView = view;
		//			}
		//		);
		//	}
	}
}
