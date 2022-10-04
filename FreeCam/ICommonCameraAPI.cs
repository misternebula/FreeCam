using System;
using UnityEngine;

namespace FreeCam;

public interface ICommonCameraAPI
{
	(OWCamera, Camera) CreateCustomCamera(string name, Action<OWCamera> postInitMethod);
	void ExitCamera(OWCamera OWCamera);
	void EnterCamera(OWCamera OWCamera);
}
