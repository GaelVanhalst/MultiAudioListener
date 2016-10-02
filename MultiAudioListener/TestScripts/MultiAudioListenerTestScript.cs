using UnityEngine;

namespace Assets.MultiAudioListener.TestScripts
{
    public class MultiAudioListenerTestScript : MonoBehaviour
    {
        [SerializeField] private MultiAudioSource _audioSource = null;

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.P)) _audioSource.Play();
            if (Input.GetKeyDown(KeyCode.S)) _audioSource.Stop();

            if (Input.GetKeyDown(KeyCode.Pause))
            {
                if (_audioSource.IsPaused)
                {
                    _audioSource.UnPause();
                }
                else
                {
                    _audioSource.Pause();
                }
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                _audioSource.Mute = !_audioSource.Mute;
            }
        }
    }
}
