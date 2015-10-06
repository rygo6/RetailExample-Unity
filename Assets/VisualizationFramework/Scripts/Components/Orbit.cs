using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EC
{
	public class Orbit : MonoBehaviour
	{		
		private View _orbitView;

		private Catcher _catcher;
	
		[SerializeField]
		private float _xSpeed = 10.0f;
	
		[SerializeField]
		private float _ySpeed = 10.0f;

		[SerializeField]
		private float _xMinLimit = 20f;
	
		[SerializeField]
		private float _xMaxLimit = 180f;
	
		[SerializeField]
		private float _yMinLimit = -80f;
	
		[SerializeField]
		private float _yMaxLimit = 80f;
	
		[SerializeField]
		private float _pinchSpeed = 10.0f;
	
		[SerializeField]
		private float _minZoom = -15f;
	
		[SerializeField]
		private float _maxZoom = -5f;
	
		[SerializeField]
		private float _strafeSpeed = 1.0f;
	
		[SerializeField]
		private Vector3 _minStrafe = new Vector3(-2.0f, -2.0f, -2.0f);
	
		[SerializeField]
		private Vector3 _maxStrafe = new Vector3(2.0f, 2.0f, 2.0f);
	
		private Plane _axisPlane;
	
		[SerializeField]
		private Axis _strafeAxis;
	
		private const float MaxMomentumWait = .05f;

		private void Awake()
		{
			SetAxisPlane();

			_orbitView = GetComponentInChildren<View>();

			_catcher = GameObject.FindObjectOfType<Catcher>();
		}

		private void Update()
		{			
			InputStateSwitch();
		}

		private void SetAxisPlane()
		{
			switch (_strafeAxis)
			{
			case ( Axis.X ):
				_axisPlane = new Plane(Vector3.forward, Vector3.zero);	
				break;
			case ( Axis.Z ):
				_axisPlane = new Plane(Vector3.right, Vector3.zero);				
				break;
			}			
		}
	
		private void InputStateSwitch()
		{		
			switch (_catcher.PointerEventDataList.Count)
			{
			case 0:
				NoInput();
				break;
			case 1:
				ProcessOrbit();
				break;
			case 2:
				Zoom();
				Pan();
				break;
			case 3:
		
				break;
			default:
				break;
			}

			if (_orbitMomentumTimer.x >= 0f)
			{
				_orbitMomentumTimer.x -= Time.deltaTime;
			}
			if (_orbitMomentumTimer.y >= 0f)
			{
				_orbitMomentumTimer.y -= Time.deltaTime;	
			}
			if (_zoomMomentumTimer >= 0f)
			{
				_zoomMomentumTimer -= Time.deltaTime;
			}
			if (_panMomentumTimer.x >= 0f)
			{
				_panMomentumTimer.x -= Time.deltaTime;
			}
			if (_panMomentumTimer.y >= 0f)
			{
				_panMomentumTimer.y -= Time.deltaTime;
			}
		}
		private const float _inputSwitchTimeTreshhold = .1f;

		private void NoInput()
		{
			_orbitDelta = Vector2.Lerp(_orbitDelta, Vector2.zero, Time.deltaTime * 8);
			_zoomDelta = Mathf.Lerp(_zoomDelta, 0f, Time.deltaTime * 8);
			_panDelta = Vector3.Lerp(_panDelta, Vector3.zero, Time.deltaTime * 8);						
		
			//zoom
			Vector3 vector3 = _orbitView.transform.localPosition;
			vector3.z += _zoomDelta;
			if (vector3.z > _maxZoom)
			{
				vector3.z = _maxZoom;
			}
			else if (vector3.z < _minZoom)
			{
				vector3.z = _minZoom;
			}
			_orbitView.transform.localPosition = vector3;		
		
			//rotate turntable
			vector3 = transform.eulerAngles;
			vector3.y += _orbitDelta.x;
			if (_xMaxLimit != 0 && _xMinLimit != 0)
			{				
				vector3.y = ClampUtility.ClampAngleX(vector3.y, _xMinLimit, _xMaxLimit);	
			}
			transform.eulerAngles = vector3;
			vector3 = transform.localEulerAngles;
			vector3.x -= _orbitDelta.y;
			vector3.x = ClampUtility.ClampAngleY(vector3.x, _yMinLimit, _yMaxLimit);				
			transform.localEulerAngles = vector3;
		
			Vector3 newPos = transform.position;
			switch (_strafeAxis)
			{
			case Axis.X:
				PanAxis(ref newPos.x, _panDelta.x, _minStrafe.x, _maxStrafe.x);	
				break;
			case Axis.Y:
				PanAxis(ref newPos.y, _panDelta.y, _minStrafe.y, _maxStrafe.y);	
				break;
			case Axis.Z:
				PanAxis(ref newPos.z, _panDelta.z, _minStrafe.z, _maxStrafe.z);		
				break;
			case Axis.XY:
				PanAxis(ref newPos.x, _panDelta.x, _minStrafe.x, _maxStrafe.x);	
				PanAxis(ref newPos.y, _panDelta.y, _minStrafe.y, _maxStrafe.y);				
				break;
			case Axis.YZ:
				PanAxis(ref newPos.y, _panDelta.y, _minStrafe.y, _maxStrafe.y);				
				PanAxis(ref newPos.z, _panDelta.z, _minStrafe.z, _maxStrafe.z);				
				break;
			}
			transform.position = newPos;	
		}
	
		private void ProcessOrbit()
		{		
			Vector2 incomingInputDelta = new Vector2();	
		
			incomingInputDelta.x = _catcher.PointerEventDataList[0].delta.x * _xSpeed * .02f;
			incomingInputDelta.y = _catcher.PointerEventDataList[0].delta.y * _ySpeed * .02f;			
		
			//dont remember specifics of this
			float momentumWait = Mathf.Clamp(incomingInputDelta.magnitude / 8f, 0f, MaxMomentumWait);
		
			//if( Mathf.Abs( incomingInputDelta.x ) > Mathf.Abs( inputDelta.x ) )
			if ((_orbitDelta.x > 0 && incomingInputDelta.x > _orbitDelta.x) ||
			  (_orbitDelta.x < 0 && incomingInputDelta.x < _orbitDelta.x) ||
			  _orbitDelta.x == 0)
			{
				_orbitDelta.x = incomingInputDelta.x;
				_orbitMomentumTimer.x = momentumWait;
			}
			else if (_orbitMomentumTimer.x <= 0)
			{
				_orbitDelta.x = incomingInputDelta.x;			
			}
		
			if ((_orbitDelta.y > 0 && incomingInputDelta.y > _orbitDelta.y) ||
			  (_orbitDelta.y < 0 && incomingInputDelta.y < _orbitDelta.y) ||
			  _orbitDelta.y == 0)
			{
				_orbitDelta.y = incomingInputDelta.y;				
				_orbitMomentumTimer.y = momentumWait;
			}
			else if (_orbitMomentumTimer.y <= 0)
			{
				_orbitDelta.y = incomingInputDelta.y;				
			}
		
			Vector3 newRotation = transform.eulerAngles;
			newRotation.y += _orbitDelta.x;	
			if (_xMaxLimit != 0 && _xMinLimit != 0)
			{	
				newRotation.y = ClampUtility.ClampAngleX(newRotation.y, _xMinLimit, _xMaxLimit);
			}
			transform.eulerAngles = newRotation;
			Vector3 newLocalRotation = transform.localEulerAngles;
			newLocalRotation.x -= _orbitDelta.y;
			newLocalRotation.x = ClampUtility.ClampAngleY(newLocalRotation.x, _yMinLimit, _yMaxLimit);				
			transform.localEulerAngles = newLocalRotation;
		}
		private Vector2 _orbitDelta;
		private Vector2 _orbitMomentumTimer;

		private void Pan()
		{
			Vector3 incomingDeltaPos = (_catcher.PointerEventDataList[0].delta + _catcher.PointerEventDataList[0].delta) / 2f;

			incomingDeltaPos = incomingDeltaPos * .001f * _strafeSpeed;
		
			float momentumWait = Mathf.Clamp(Mathf.Abs(incomingDeltaPos.x / 8f), 0f, MaxMomentumWait);			
		
			//strafing
			float rightPlaneAngle = Vector3.Angle(_orbitView.transform.forward, _axisPlane.normal);			
		
			if ((_panDelta.x > 0 && incomingDeltaPos.x > _panDelta.x) ||
			  (_panDelta.x < 0 && incomingDeltaPos.x < _panDelta.x) ||
			  _panDelta.x == 0)
			{
				if (rightPlaneAngle < 90)
					_panDelta.x = Mathf.SmoothDamp(_panDelta.x, incomingDeltaPos.x, ref _panSmoothDamp.x, .1f);
				else
					_panDelta.x = Mathf.SmoothDamp(_panDelta.x, -incomingDeltaPos.x, ref _panSmoothDamp.x, .1f);				
			
				_panMomentumTimer.x = momentumWait;
			}
			else if (_panMomentumTimer.x <= 0)
			{
				if (rightPlaneAngle < 90)
				{
					_panDelta.x = Mathf.SmoothDamp(_panDelta.x, incomingDeltaPos.x, ref _panSmoothDamp.x, .1f);
				}
				else
				{
					_panDelta.x = Mathf.SmoothDamp(_panDelta.x, -incomingDeltaPos.x, ref _panSmoothDamp.x, .1f);	
				}
			}

			if ((_panDelta.y > 0 && incomingDeltaPos.y > _panDelta.y) ||
			  (_panDelta.y < 0 && incomingDeltaPos.y < _panDelta.y) ||
			  _panDelta.y == 0)
			{
				_panDelta.y = Mathf.SmoothDamp(_panDelta.y, -incomingDeltaPos.y, ref _panSmoothDamp.y, .1f);
				_panMomentumTimer.y = momentumWait;
			}
			else if (_panMomentumTimer.x <= 0)
			{
				_panDelta.y = Mathf.SmoothDamp(_panDelta.y, -incomingDeltaPos.y, ref _panSmoothDamp.y, .1f);	
			}

			Vector3 newPos = transform.position;
		
			switch (_strafeAxis)
			{
			case Axis.X:
				PanAxis(ref newPos.x, _panDelta.x, _minStrafe.x, _maxStrafe.x);	
				break;
			case Axis.Y:
				PanAxis(ref newPos.y, _panDelta.y, _minStrafe.y, _maxStrafe.y);	
				break;
			case Axis.Z:
				PanAxis(ref newPos.z, _panDelta.z, _minStrafe.z, _maxStrafe.z);		
				break;
			case Axis.XY:
				PanAxis(ref newPos.x, _panDelta.x, _minStrafe.x, _maxStrafe.x);	
				PanAxis(ref newPos.y, _panDelta.y, _minStrafe.y, _maxStrafe.y);				
				break;
			case Axis.YZ:
				PanAxis(ref newPos.y, _panDelta.y, _minStrafe.y, _maxStrafe.y);				
				PanAxis(ref newPos.z, _panDelta.z, _minStrafe.z, _maxStrafe.z);				
				break;
			}

			transform.position = newPos;
		}
		private Vector3 _panDelta;
		private Vector2 _panMomentumTimer;
		private Vector2 _panSmoothDamp;

		private void PanAxis(ref float newPos, float panDelta, float min, float max)
		{
			newPos -= panDelta;
			if (newPos > max)
			{
				newPos = max;
			}
			else if (newPos < min)
			{
				newPos = min;	
			}
		}

		private void Zoom()
		{			
			Vector2 curDist = _catcher.PointerEventDataList[0].position - _catcher.PointerEventDataList[1].position ;
			Vector2 prevDist = (_catcher.PointerEventDataList[0].position - _catcher.PointerEventDataList[0].delta) -
								(_catcher.PointerEventDataList[1].position - _catcher.PointerEventDataList[1].delta);			
			float incomingZoomDelta = (curDist.magnitude - prevDist.magnitude);
			incomingZoomDelta = incomingZoomDelta * _pinchSpeed * 0.02f;
		
			//zooming
			float momentumWait = Mathf.Clamp(incomingZoomDelta, 0f, MaxMomentumWait);
			if ((_zoomDelta > 0 && incomingZoomDelta > _zoomDelta) ||
			  (_zoomDelta < 0 && incomingZoomDelta < _zoomDelta) ||
			  _zoomDelta == 0)
			{
				_zoomDelta = Mathf.SmoothDamp(_zoomDelta, incomingZoomDelta, ref _zoomSmoothDamp, .1f);
				_zoomMomentumTimer = momentumWait;
			}
			else if (_zoomMomentumTimer <= 0)
			{
				_zoomDelta = Mathf.SmoothDamp(_zoomDelta, incomingZoomDelta, ref _zoomSmoothDamp, .1f);		
			}			
		
			Vector3 newPos = _orbitView.transform.localPosition;
			newPos.z += _zoomDelta;
			if (newPos.z > _maxZoom)
			{
				newPos.z = _maxZoom;
			}
			else if (newPos.z < _minZoom)
			{
				newPos.z = _minZoom;
			}
			_orbitView.transform.localPosition = newPos;		
		}
		private float _zoomDelta;
		private float _zoomMomentumTimer;
		private float _zoomSmoothDamp;
	}
}
