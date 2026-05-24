using UnityEngine.Events;

namespace FreeCam
{
    public interface IFreeCamAPI
    {
        bool IsInFreeCam();

        void ToggleFreeCam();
        void EnterFreeCam();
        void ExitFreeCam();

        UnityEvent GetFreeCamEnteredEvent();
        UnityEvent GetFreeCamExitedEvent();
    }
}
