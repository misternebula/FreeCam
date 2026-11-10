using UnityEngine.Events;

namespace FreeCam
{
    public class FreeCamAPI : IFreeCamAPI
    {
        public bool IsInFreeCam() => MainClass.InFreeCam;

        public void ToggleFreeCam() => MainClass.ToggleFreeCam();
        public void EnterFreeCam() => MainClass.EnterFreeCam();
        public void ExitFreeCam() => MainClass.ExitFreeCam();

        public UnityEvent GetFreeCamEnteredEvent() => MainClass.OnFreeCamEntered;
        public UnityEvent GetFreeCamExitedEvent() => MainClass.OnFreeCamExited;
    }
}
