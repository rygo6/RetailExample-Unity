using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineInternal;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace EC.Visualization
{
	public class VisualizationUI : MonoBehaviour
	{
		private readonly List<GameObject> ItemBarHorizontalGroupItemList = new List<GameObject>();

		private ItemRoot Root { get; set; }

		private Catcher _catcher;

		[SerializeField]
		private GameObject _itemBarGameObject;

		[SerializeField]
		private GameObject _ItemBarHorizontalGroupGameObject;

		private ItemReference _selectedItem;
		
		private int _selectedItemIndex = -1;

		[SerializeField]
		private GameObject _itemBarItemPrefab;

		private ItemReference[] _currentlyLoadedItemArray;

		private ItemReference[] ProductItemArray
		{
			get
			{
				if (_productItemArray == null)
				{
					PopulateItemArrayFromBundle(out _productItemArray, "merchandise");
				}
				return _productItemArray;
			}
		}
		private ItemReference[] _productItemArray;

		private ItemReference[] AttachmentsItemArray
		{
			get
			{
				if (_attachmentsItemArray == null)
				{
					PopulateItemArrayFromBundle(out _attachmentsItemArray, "attachments");
				}
				return _attachmentsItemArray;
			}
		}
		private ItemReference[] _attachmentsItemArray;

		private ItemReference[] FixturesItemArray
		{
			get
			{
				if (_fixturesItemArray == null)
				{
					PopulateItemArrayFromBundle(out _fixturesItemArray, "fixtures");
				}
				return _fixturesItemArray;
			}
		}
		private ItemReference[] _fixturesItemArray;

		private ItemReference[] ColorItemArray
		{
			get
			{
				if (_colorItemArray == null)
				{
					PopulateItemArrayFromBundle(out _colorItemArray, "colors");
				}
				return _colorItemArray;
			}
		}
		private ItemReference[] _colorItemArray;

		private void Awake()
		{
			Root = FindObjectOfType<ItemRoot>();

			_catcher = GameObject.FindObjectOfType<Catcher>();
		}

		public void UnselectSelectedItem()
		{
			if (_selectedItemIndex != -1)
			{
				Root.SetAllOutlineNormalAttach(); 		
				_selectedItem = null;
				ItemBarHorizontalGroupItemList[_selectedItemIndex].GetComponent<Transform>().FindChild("ItemBarItemHighlight").GetComponent<Image>().enabled = false;
				_selectedItemIndex = -1;
			}
		}
	
		public void ItemButtonClick(int itemArrayIndex)
		{
			if (_selectedItemIndex != itemArrayIndex)
			{
				UnselectSelectedItem();
		
				_selectedItemIndex = itemArrayIndex;
			
				_selectedItem = _currentlyLoadedItemArray[itemArrayIndex];	
	       	        
				System.Action<Item> trueAction = delegate(Item item)
				{
					item.SetShaderOutline(Persistent.GetComponent<ItemSettings>().InstantiateOutlineColor);
					item.State = ItemState.Instantiate;
				};
				System.Action<Item> falseAction = delegate(Item item)
				{
					item.SetShaderNormal();			
					item.State = ItemState.NoInstantiate;
				};		
				System.Func<Item,bool> filterAction = delegate(Item item)
				{		
					ItemDrop itemDrop = item.GetComponent<ItemDrop>();
					ItemColor itemColor = item.GetComponent<ItemColor>();
					const string colorTag = "Color";
					if (itemDrop != null)
					{
						return itemDrop.CanAttach(_selectedItem.TagArray);
					}
					if (itemColor != null && _selectedItem.HasTag(colorTag))
					{
						return true;
					}
					return false;
				};					
				
				int trueCount = Root.CallDelegateTagFilter(filterAction, trueAction, falseAction);	
			
				if (trueCount == 0)
				{
					_selectedItem = null;
					Root.SetAllOutlineNormalAttach();			
					_selectedItemIndex = -1;
				}
				else
				{
					System.Action action = delegate()
					{
						UnselectSelectedItem();
					};       	
					_catcher.EmptyClickAction = action;
	           	
					ItemBarHorizontalGroupItemList[_selectedItemIndex].GetComponent<Transform>().FindChild("ItemBarItemHighlight").GetComponent<Image>().enabled = true;
				}
			}
			else
			{
				UnselectSelectedItem();        
			}
		}

		public void SpawnItemFromMenuDrag(PointerEventData data, int itemArrayIndex)
		{
			Root.UnHighlightAll();
	
			_selectedItemIndex = itemArrayIndex;
			_selectedItem = _currentlyLoadedItemArray[itemArrayIndex];	
		
			Item item = InstantiateSelectedItem(data);
		
			item.Highlight();

			UnselectSelectedItem();	

			Switch.SwitchGameObject(item.gameObject, data);
		}

		public Item InstantiateSelectedItem(PointerEventData data)
		{
			GameObject prefab = (GameObject)_selectedItem.AssetBundle.LoadAsset(_selectedItem.PrefabName);
		
			Ray ray = ViewSingleton.Instance.mainView.GetComponent<Camera>().ScreenPointToRay(data.position);	
		
			GameObject instance = (GameObject)Instantiate(prefab);
		
			Item item = instance.GetComponent<Item>();
			item.SourceAssetBundle = _selectedItem.AssetBundle;
			item.SourceBundlePrefabName = _selectedItem.PrefabName;
			item.transform.position = ray.GetPoint(3.5f);	
			item.GetComponent<ItemDrag>().TargetTransform.position = item.transform.position;			

			return item;	
		}

		public void LoadItemArray(ItemReference[] itemArray)
		{	
			_currentlyLoadedItemArray = itemArray;	
	
			RectTransform rectTransform = _ItemBarHorizontalGroupGameObject.GetComponent<RectTransform>();
			float totalWidth = 0f;
		
			for (int i = 0; i < itemArray.Length; ++i)
			{
				GameObject itemGameObject;
				//if menu item already has instance, recycle it, otherwise instantiate a new one
				if (i < ItemBarHorizontalGroupItemList.Count)
				{
					itemGameObject = ItemBarHorizontalGroupItemList[i];		
					if (!itemGameObject.activeSelf)
					{
						itemGameObject.SetActive(true);	
					}
				}
				else
				{
					itemGameObject = (GameObject)Instantiate(_itemBarItemPrefab);
					RectTransform itemGameObjectRectTransform = itemGameObject.GetComponent<RectTransform>();
					itemGameObjectRectTransform.SetParent(_ItemBarHorizontalGroupGameObject.GetComponent<RectTransform>(), false);																		
					ItemBarHorizontalGroupItemList.Add(itemGameObject);
					ItemBarHorizontalGroupItemList[i].GetComponent<ScrollItemEvent>().ItemArrayIndex = i;
				}
			
				if (itemArray[i].PreviewSprite != null)
				{
					Resources.UnloadAsset(itemArray[i].PreviewSprite);
					itemArray[i].PreviewSprite = null;
				}
			
				//load preview images
				Image image = (Image)itemGameObject.GetComponent<Image>();
				if (itemArray[i].PreviewSprite == null)
				{
					itemArray[i].PreviewSprite = itemArray[i].AssetBundle.LoadAsset(itemArray[i].PreviewName, typeof(Sprite)) as Sprite;
				}
				image.sprite = itemArray[i].PreviewSprite;
				LayoutElement layoutElement = itemGameObject.GetComponent<LayoutElement>();
				float ratio = (float)itemArray[i].PreviewSprite.texture.width / (float)itemArray[i].PreviewSprite.texture.height;
				layoutElement.minWidth = rectTransform.rect.height * ratio;
				layoutElement.preferredWidth = layoutElement.minWidth;
			
				totalWidth += layoutElement.minWidth;
			}

			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
			rectTransform.transform.position = new Vector3(_itemBarGameObject.GetComponent<RectTransform>().rect.width, 
				rectTransform.transform.position.y, 
				rectTransform.transform.position.z);
	
			//disable additional Items
			for (int i = itemArray.Length; i < ItemBarHorizontalGroupItemList.Count; ++i)
			{
				ItemBarHorizontalGroupItemList[i].SetActive(false);
			}
		
			Resources.UnloadUnusedAssets();
		}
		
		public void PopulateItemArrayFromBundle(out ItemReference[] itemArray, string bundlePath)
		{
			AssetBundle assetBundle = Persistent.GetComponent<BundleStore>().LoadBundle(bundlePath);
			List<ItemReference> itemList = new List<ItemReference>();
			string[] assetNames = assetBundle.GetAllAssetNames();
			for (int i = 0; i < assetNames.Length; ++i)
			{
				if (assetNames[i].Contains("preview"))
				{
					string name = Path.GetFileNameWithoutExtension(assetNames[i]);
					ItemReference itemReference = new ItemReference();
					itemReference.Name = name.Replace("_preview", "");
					itemReference.PrefabName = name.Replace("_preview", "_prefab");
					itemReference.PreviewName = name;
					GameObject asset = (GameObject)assetBundle.LoadAsset(itemReference.PrefabName);
//					Debug.Log(asset);
					Item item = asset.GetComponent<Item>();
					itemReference.TagArray = item.TagArray;					
					itemReference.AssetBundle = assetBundle;
					itemList.Add(itemReference);
				}
			}
			itemArray = itemList.ToArray();
			itemList.Clear();
		}

		public void LoadFixtures()
		{
			UnselectSelectedItem();
			LoadItemArray(FixturesItemArray);
		}
	
		public void LoadAttachments()
		{
			UnselectSelectedItem();
			LoadItemArray(AttachmentsItemArray);		
		}
	
		public void LoadProduct()
		{
			UnselectSelectedItem();
			LoadItemArray(ProductItemArray);			
		}
	
		public void LoadColors()
		{
			UnselectSelectedItem();
			LoadItemArray(ColorItemArray);			
		}
	}
}