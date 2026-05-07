using UnityEngine;

namespace UI.Round
{
    public class ReadyUIButton : MonoBehaviour
    {
        public void OnClickReady()
        {
            StageManager.Instance.SetNextRound();
        }
    }
}


