using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

namespace EC.Visualization
{
	public class SplineReplicator : Replicator
	{
		public Transform _targetStartPoint;
	
		public Transform _targetEndPoint;

		private Transform _startPoint;

		private Transform _endPoint;

		private const float TargetLerpTime = 1.0f;

		private float _lerpTime;
	
		public override Item Item
		{ 
			get
			{ 
				return base.Item; 
			} 
			set
			{
				base.Item = value;			
				_startPoint.position = RootTransform.position;
				_endPoint.position = RootTransform.position;
			}	
		}

		private void Awake()
		{
			_targetStartPoint = new GameObject(this.name + "TargetStart").GetComponent<Transform>();

			_targetEndPoint = new GameObject(this.name + "TargetEnd").GetComponent<Transform>();

			_startPoint = new GameObject(this.name + "Start").GetComponent<Transform>();

			_endPoint = new GameObject(this.name + "End").GetComponent<Transform>();
		}
	
		private void Update()
		{
			DrawReplicator();
		}

		public void SetAllPointParent(Transform parentTransform)
		{
			_targetStartPoint.parent = parentTransform;
			_targetEndPoint.parent = parentTransform;
			_startPoint.parent = parentTransform;
			_endPoint.parent = parentTransform;
		}

		public float DisctanceBetweenStartEnd()
		{
			return Vector3.Distance(_startPoint.position, _endPoint.position);
		}
	
		public void SetStartEndPoint(Vector3 overallRangeMin, Vector3 overallRangeMax, float startRatio, float endRatio)
		{	
			_targetStartPoint.position = Vector3.Lerp(overallRangeMin, overallRangeMax, startRatio);
			_targetEndPoint.position = Vector3.Lerp(overallRangeMin, overallRangeMax, endRatio);
		}
	
		public Vector3 DuplicateRangeMidPoint()
		{
			return ((_targetStartPoint.position - Offset + _targetEndPoint.position - Offset)) / 2f;
		}

		private void DrawReplicator()
		{
			_lerpTime = Mathf.Lerp(_lerpTime, TargetLerpTime, .1f);
		
			_startPoint.position = Vector3.SmoothDamp(_startPoint.position, _targetStartPoint.position, ref _startPointVelocity, .1f);	
			_endPoint.position = Vector3.SmoothDamp(_endPoint.position, _targetEndPoint.position, ref _endPointVelocity, .1f);	
		
			Vector3 startPos = RootTransform.position;
			Vector3 endPos = _endPoint.position - Offset;
			Vector3 pos;
			float lerpStep = 1f / (Vector3.Distance(startPos, endPos) / Spacing.z);
			float lerp = lerpStep;
			float spacing = Spacing.z;
			Matrix4x4 matrix;
			int i;
			int limit = MeshArray.Length;
		
			while (lerp < 1f)
			{
				pos = Vector3.Lerp(startPos, endPos, lerp);
				pos = Vector3.Lerp(RootTransform.position, pos, _lerpTime);
			
				for (i = 0; i < limit; ++i)
				{
					matrix = Matrix4x4.TRS(pos + (MeshTransformArray[i].position - RootTransform.position), 
						MeshTransformArray[i].rotation, RootTransform.localScale);
					Graphics.DrawMesh(MeshArray[i], matrix, MeshRendererArray[i].material, 0);
				}
			
				lerp += lerpStep;
				spacing += Spacing.z;
			}
		
			endPos = _startPoint.position - Offset;
			lerpStep = 1f / (Vector3.Distance(startPos, endPos) / Spacing.z);
			lerp = lerpStep;
			spacing = Spacing.z;
			while (lerp < 1f)
			{
				pos = Vector3.Lerp(startPos, endPos, lerp);
				pos = Vector3.Lerp(RootTransform.position, pos, _lerpTime);
			
				for (i = 0; i < limit; ++i)
				{
					matrix = Matrix4x4.TRS(pos + (MeshTransformArray[i].position - RootTransform.position),
						MeshTransformArray[i].rotation, RootTransform.localScale);
					Graphics.DrawMesh(MeshArray[i], matrix, MeshRendererArray[i].material, 0);
				}
			
				lerp += lerpStep;
				spacing += Spacing.z;
			}		
		}
		private Vector3 _startPointVelocity;
		private Vector3 _endPointVelocity;
	}
}