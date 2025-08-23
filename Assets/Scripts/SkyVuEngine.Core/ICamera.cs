using SkyVuEngine.Core.Camera;
using UnityEngine;

public class ICamera : MonoBehaviour
{
	protected SkyVuEngine.Core.Camera.CameraType _cameraType;

	public bool IsPlayerFriendly { get; private set; }

	public SkyVuEngine.Core.Camera.CameraType CameraType
	{
		get
		{
			return _cameraType;
		}
	}

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
	}
}
