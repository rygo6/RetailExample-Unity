//#define LOG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EC.Visualization
{
	public class Item : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
	{
		public string UniqueTick
		{ 
			get
			{ 
				if (string.IsNullOrEmpty(_uniqueTick))
				{
					_uniqueTick = ItemSingleton.Instance.TickToString();
					ItemSingleton.Instance.UniqueTickDictionary.Add(_uniqueTick, this);	
				}	
				return _uniqueTick; 
			} 
			set
			{
				if (!string.IsNullOrEmpty(_uniqueTick))
				{
					ItemSingleton.Instance.UniqueTickDictionary.Remove(_uniqueTick);
				}
				_uniqueTick = value;
				ItemSingleton.Instance.UniqueTickDictionary.Add(_uniqueTick, this);
			}	
		}
		private string _uniqueTick;
    
		[SerializeField]
		private bool _disableOutline;
    
		public string SourceBundlePrefabName { get; set; }
	
		public AssetBundle SourceAssetBundle { get; set; }
    
		public string[] TagArray { get { return _tagArray; } }
		[SerializeField]
		private string[] _tagArray;
	
		public string[] ChildTagArray { get { return _childTagArray; } }
		[SerializeField]
		private string[] _childTagArray;
	
		[SerializeField]
		private ItemPartner[] _ItemPartnerArray;
	
		public Material[] BlendMaterialArray { get; private set; }
	
		public Material[] MaterialArray { get; private set; }
    
		public Mesh[] MeshArray { get; private set; }
	
		public MeshRenderer[] MeshRendererArray { get; private set; }
	
		public Transform[] MeshTransformArray { get; private set; }

		[SerializeField]
		private Vector3 _colliderExpand = new Vector3(1.0f, 1.0f, 1.0f);
	
		public BoxCollider[] ColliderArray { get; private set; }
	
		public Vector3[] InitialColliderSizeArray { get; private set; }
			
		public Vector3[] InitialColliderCenterArray { get; private set; }
    
		public GameObject[] ColliderGameObjectArray { get; private set; }

		public ItemState State { get; set; }

		private Catcher _catcher;

		private void Awake()
		{
			MeshRendererArray = this.GetComponentsInChildren<MeshRenderer>();

			MeshArray = new Mesh[MeshRendererArray.Length];
			for (int i = 0; i < MeshArray.Length; ++i)
			{
				MeshArray[i] = MeshRendererArray[i].GetComponent<MeshFilter>().sharedMesh;
			}

			MeshTransformArray = new Transform[MeshRendererArray.Length];
			for (int i = 0; i < MeshTransformArray.Length; ++i)
			{
				MeshTransformArray[i] = MeshRendererArray[i].GetComponent<Transform>();
			}

			MaterialArrayInitialize();
			ColliderArrayInitialize();

			_catcher = GameObject.FindObjectOfType<Catcher>();
		}

		private void MaterialArrayInitialize()
		{
			Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
			List<Material> blendMateriaList = new List<Material>();
			List<Material> materiaList = new List<Material>();
			for (int i = 0; i < renderers.Length; ++i)
			{
				for (int m = 0; m < renderers[i].materials.Length; ++m)
				{
					if (renderers[i].materials[m].shader.ToString() == "Mobile/VertexLit (Only Directional Lights) Blend (UnityEngine.Shader)")
					{
						blendMateriaList.Add(renderers[i].materials[m]);
					}
					else
					{	
						materiaList.Add(renderers[i].materials[m]);
					}
				}
			}
			BlendMaterialArray = blendMateriaList.ToArray();
			MaterialArray = materiaList.ToArray();		 
		}

		private void ColliderArrayInitialize()
		{
			ColliderArray = this.GetComponentsInChildren<BoxCollider>();
			ColliderGameObjectArray = new GameObject[ColliderArray.Length];
			InitialColliderSizeArray = new Vector3[ColliderArray.Length];
			InitialColliderCenterArray = new Vector3[ColliderArray.Length];
			for (int i = 0; i < ColliderGameObjectArray.Length; ++i)
			{
				ColliderGameObjectArray[i] = ColliderArray[i].gameObject;
				InitialColliderSizeArray[i] = ColliderArray[i].size;
				InitialColliderCenterArray[i] = ColliderArray[i].center;
			}	
		}
	
		public void OnPointerDown(PointerEventData data)
		{
			#if LOG
			Debug.Log( "OnPointerDown " + this.name );	
			#endif	
		
			ItemUtility.StateSwitch(data, State,
				OnPointerDownAttached,
				OnPointerDownAttachedHighlighted,
				null,
				null,
				OnPointerDownInstantiate,
				null 
			);	  			   
		}
	
		private void OnPointerDownAttached(PointerEventData data)
		{
			#if LOG
			Debug.Log( "OnPointerDownAttached " + this.name );
			#endif
		
			SetShaderOutline(ItemSingleton.Instance.DownHighlightItemColor);
		}
	
		private void OnPointerDownAttachedHighlighted(PointerEventData data)
		{
			#if LOG
			Debug.Log( "OnPointerDownAttachedHighlighted " + this.name );
			#endif
		
			SetLayerRecursive(2);
			SetShaderOutline(ItemSingleton.Instance.DownHighlightItemColor);
		}
    
		private void OnPointerDownInstantiate(PointerEventData data)
		{
			#if LOG
			Debug.Log( "OnPointerDownAttachedHighlighted " + this.name );
			#endif
		
			SetShaderOutline(ItemSingleton.Instance.DropOutlineColor);
		}
	
		public void OnPointerUp(PointerEventData data)
		{
			#if LOG
			Debug.Log( "OnPointerUp " + this.name );	
			#endif	
        
			ItemUtility.StateSwitch(data, State,
				OnPointerUpAttached,
				OnPointerUpAttachedHighlighted,
				OnPointerUpDragging,
				null,
				OnPointerUpInstantiate,
				null 
			);	              
		}
    
		private void OnPointerUpAttached(PointerEventData data)
		{
			#if LOG
			Debug.Log( "OnPointerUpAttached " + this.name );
			#endif
		
			if (data.pointerCurrentRaycast.gameObject == data.pointerPressRaycast.gameObject)
			{
				ItemSingleton.Instance.UnHighlightAll();
				Highlight();
			}
			else
			{
				SetShaderNormal();
			}
		}
    
		private void OnPointerUpAttachedHighlighted(PointerEventData data)
		{
			#if LOG
			Debug.Log( "OnPointerUpAttachedHighlighted " + this.name );
			#endif
		
			SetLayerRecursive(8);
			ItemSingleton.Instance.UnHighlightAll();		
		}
    
		private void OnPointerUpDragging(PointerEventData data)
		{
			#if LOG
			Debug.Log( "OnPointerUpAttachedHighlighted " + this.name );
			#endif
		
		}
    
		private void OnPointerUpInstantiate(PointerEventData data)
		{
			#if LOG
			Debug.Log( "OnPointerUpAttachedHighlighted " + this.name );
			#endif

			VisualizationUI plannerUI = FindObjectOfType<VisualizationUI>();
			Item instantiatedItem = plannerUI.InstantiateSelectedItem(data);
			ItemDrag instantiatedDragItemMod = instantiatedItem.GetComponent<ItemDrag>();
			ItemDrop dropItem = GetComponent<ItemDrop>();
			instantiatedDragItemMod.ThisEnteredDropItem = dropItem;
			instantiatedDragItemMod.ParentItemDrop = dropItem;
		
			ItemSnap itemSnap = instantiatedDragItemMod.NearestItemSnap(data);
			instantiatedDragItemMod.ParentItemSnap = itemSnap;
			Ray ray = itemSnap.Snap(instantiatedItem, data);
			instantiatedDragItemMod.SetTargetPositionRotation(ray.origin, ray.direction); 		
			//set to outline and normal to get rid of quirk where instantied shader isn't immediately properly lit
			instantiatedItem.SetShaderOutline(ItemSingleton.Instance.IinstantiateOutlineColor);
			instantiatedItem.SetShaderNormal();
			instantiatedItem.State = ItemState.NoInstantiate;

			if (dropItem.CanAttach(instantiatedItem.TagArray))
			{
				SetShaderOutline(ItemSingleton.Instance.IinstantiateOutlineColor);
			}
			else
			{
				SetShaderNormal();
				State = ItemState.NoInstantiate;
			}
		}
	                        	   
		public IEnumerator DestroyItemCoroutine()
		{	
			UnHighlight();		
			SetShaderOutline(Color.red);

			ItemDrop dropItem = GetComponent<ItemDrop>();
			if (dropItem != null)
			{
				for (int i = 0; i < dropItem.ChildItemDragList.Count; ++i)
				{	
					StartCoroutine(dropItem.ChildItemDragList[i].GetComponent<Item>().DestroyItemCoroutine());
				}
			}		
		
			yield return new WaitForSeconds(.4f);

			RemoveUniqueTickRecursive(this);
			Destroy(this.gameObject);
		
			Resources.UnloadUnusedAssets();
			System.GC.Collect();    
		}
	
		private void RemoveUniqueTickRecursive(Item item)
		{
			ItemSingleton.Instance.UniqueTickDictionary.Remove(item.UniqueTick);
			ItemDrop item_Drop = item.GetComponent<ItemDrop>();
			if (item_Drop != null)
			{
				for (int i = 0; i < item_Drop.ItemSnapArray.Length; ++i)
				{
					ItemSingleton.Instance.UniqueTickDictionary.Remove(item_Drop.ItemSnapArray[i].UniqueTick);			
				}
			}	
			ItemDrop itemDrop = item.GetComponent<ItemDrop>();//TODO WHY IS THIS TWICE??
			if (itemDrop != null)
			{
				for (int i = 0; i < itemDrop.ChildItemDragList.Count; ++i)
				{
					RemoveUniqueTickRecursive(itemDrop.ChildItemDragList[i].GetComponent<Item>());
				}
			}			
		}
	
		public void Highlight()
		{
			System.Action action = delegate()
			{
				ItemSingleton.Instance.UnHighlightAll();
			};       	
			_catcher.EmptyClickAction = action;
		
			SetShaderOutline(ItemSingleton.Instance.HighlightItemColor);
			State = ItemState.AttachedHighlighted;
			ItemSingleton.Instance.ItemRaycastHighlightList.Add(this);
		}
	
		public void UnHighlight()
		{
			ItemDrag dragItem = GetComponent<ItemDrag>();
			if (dragItem != null)
			{
				dragItem.SetActualPositionRotationToTarget();
			}
			SetShaderNormal();
			State = ItemState.Attached;
			ItemSingleton.Instance.ItemRaycastHighlightList.Remove(this);
		}
		
		public void SetBlendMaterial(Texture texture)
		{
			for (int i = 0; i < BlendMaterialArray.Length; ++i)
			{
				BlendMaterialArray[i].SetTexture("_Blend", texture);
				//this is a total hack figure out why
				BlendMaterialArray[i].shader = Shader.Find("Mobile/VertexLit (Only Directional Lights) Blend");
			}	
		}
	
		public void SetLayerRecursive(int layer)
		{
			for (int i = 0; i < ColliderGameObjectArray.Length; i++)
			{
				ColliderGameObjectArray[i].layer = layer;
			}
			ItemDrop dropItem = GetComponent<ItemDrop>();
			if (dropItem != null)
			{
				for (int i = 0; i < dropItem.ChildItemDragList.Count; i++)
				{
					dropItem.ChildItemDragList[i].GetComponent<Item>().SetLayerRecursive(layer);			
				}
			}
		}
	
		public bool TestTagArrays(string[] firstArray, string[] secondArray)
		{
			for (int i = 0; i < firstArray.Length; ++i)
			{
				for (int o = 0; o < secondArray.Length; ++o)
				{
					if (firstArray[i] == secondArray[o])
					{
						return true;
					}
				}
			}
			return false;
		}
	
		public bool HasTag(string tag)
		{
			for (int i = 0; i < _tagArray.Length; ++i)
			{
				if (_tagArray[i] == tag)
				{
					return true;
				}
			}
			return false;
		}
	
		private void ExpandColliderSize()
		{
			if (_unExpandColliderSize == null)
			{
				_unExpandColliderSize = new Vector3[ColliderArray.Length];
			}
			for (int i = 0; i < ColliderArray.Length; ++i)
			{
				_unExpandColliderSize[i] = ColliderArray[i].size;
				ColliderArray[i].size = Vector3.Scale(ColliderArray[i].size, _colliderExpand);
			}
		}
	
		private void UnExpandColliderSize()
		{
			for (int i = 0; i < ColliderArray.Length; ++i)
			{
				ColliderArray[i].size = _unExpandColliderSize[i];
			}
		}
		private Vector3[] _unExpandColliderSize;
	
		public void ResetColliderSize()
		{
			for (int i = 0; i < ColliderArray.Length; ++i)
			{
				ColliderArray[i].size = InitialColliderSizeArray[i];
				ColliderArray[i].center = InitialColliderCenterArray[i];
			}
		}
	
		public void SetShaderOutline(Color color)
		{
			if (_currentOutlineColor != color && !_disableOutline)
			{
				_currentOutlineColor = color;
				_outlineNormal = false;
				for (int i = 0; i < BlendMaterialArray.Length; ++i)
				{
					BlendMaterialArray[i].shader = Shader.Find("Toon/Vertex Lighted Blend Outline");
					BlendMaterialArray[i].color = Color.white;
					BlendMaterialArray[i].SetFloat("_Outline", ItemSingleton.Instance.OutlineSize);
					BlendMaterialArray[i].SetColor("_OutlineColor", color);
				}
				for (int i = 0; i < MaterialArray.Length; ++i)
				{
					MaterialArray[i].shader = Shader.Find("Toon/Vertex Lighted Blend Outline");
					MaterialArray[i].color = Color.white;
					MaterialArray[i].SetFloat("_Outline", ItemSingleton.Instance.OutlineSize);
					MaterialArray[i].SetColor("_OutlineColor", color);
				}
			}
		}
	
		public void SetShaderNormal()
		{
			if (!_outlineNormal)
			{
				_outlineNormal = true;
				_currentOutlineColor = Color.clear;
				if (BlendMaterialArray != null)
				{
					for (int i = 0; i < BlendMaterialArray.Length; ++i)
					{
						BlendMaterialArray[i].shader = Shader.Find("Mobile/VertexLit (Only Directional Lights) Blend");
					
					}
				}
				if (MaterialArray != null)
				{
					for (int i = 0; i < MaterialArray.Length; ++i)
					{
						MaterialArray[i].shader = Shader.Find("Mobile/VertexLit (Only Directional Lights)");
					
					}
				}
			}
		}
		private bool _outlineNormal = true;
		private Color _currentOutlineColor;
	}
}