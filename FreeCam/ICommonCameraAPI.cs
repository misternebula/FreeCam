using UnityEngine;

namespace FreeCam
{
	public interface ICommonCameraAPI
	{
		void RegisterCustomCamera(OWCamera OWCamera);
		(OWCamera, Camera) CreateCustomCamera(string name);
	}
}
