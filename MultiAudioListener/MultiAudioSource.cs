/*
 * Copyright (c) 2016 Gaël Vanhalst
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 *    1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 
 *    2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 
 *    3. This notice may not be removed or altered from any source
 *    distribution.
 */

//Activate this pragma in case the sub audio needs to be shown in hierachy for debugging purposes.
#define ShowSubAudioSourcesInHierachy

using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Assets.MultiAudioListener;
using UnityEngine.Audio;

namespace Assets.MultiAudioListener
{
    public class MultiAudioSource : MonoBehaviour
    {
        #region AudioSourceProperties

        //Properties from the normal audiosource

        [SerializeField] private AudioClip _audioClip = null;

        public AudioClip AudioClip
        {
            get { return _audioClip; }
            set
            {
                _audioClip = value;
                //Audio clip has influence on length so safety audio source needs it
                if (_safetyAudioSource != null) _safetyAudioSource.clip = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.clip = value;
                }
            }
        }

        [SerializeField] private AudioMixerGroup _output = null;

        public AudioMixerGroup Output
        {
            get { return _output; }
            set
            {
                _output = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.outputAudioMixerGroup = value;
                }
            }
        }

        [SerializeField] private bool _mute = false;

        public bool Mute
        {
            get { return _mute; }
            set
            {
                _mute = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.mute = value;
                }
            }
        }

        [SerializeField] private bool _bypassEffects = false;

        public bool BypassEffects
        {
            get { return _bypassEffects; }
            set
            {
                _bypassEffects = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.bypassEffects = value;
                }
            }
        }

        [SerializeField] private bool _bypassListenerEffects = false;

        public bool BypassListenerEffects
        {
            get { return _bypassListenerEffects; }
            set
            {
                _bypassListenerEffects = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.bypassListenerEffects = value;
                }
            }
        }

        [SerializeField] private bool _bypassReverbZone = false;

        public bool BypassReverbZone
        {
            get { return _bypassReverbZone; }
            set
            {
                _bypassReverbZone = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.bypassReverbZones = value;
                }
            }
        }

        [SerializeField] private bool _playOnAwake = true;

        public bool PlayOnAwake
        {
            get { return _playOnAwake; }
            set
            {
                _playOnAwake = value;
                //Play on awake gets handles by the main audiosource, so subAudioSources don't need it
            }
        }

        [SerializeField] private bool _loop = false;

        public bool Loop
        {
            get { return _loop; }
            set
            {
                _loop = value;
                //Loop has influence on length so safety audio source needs it
                if (_safetyAudioSource != null) _safetyAudioSource.loop = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.loop = value;
                }
            }
        }

        [Range(0, 256)] [SerializeField] private int _priority = 128;

        public int Priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.priority = value;
                }
                //Priority is important, because if safety audio stops all subs stop too
                if (_safetyAudioSource != null) _safetyAudioSource.priority = Priority + 1;
            }
        }

        [Range(0.0f, 1.0f)] [SerializeField] private float _volume = 1.0f;

        public float Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.volume = value*subAudioSource.Key.Volume;
                }
            }
        }

        [Range(-3.0f, 3.0f)] [SerializeField] private float _pitch = 1.0f;

        public float Pitch
        {
            get { return _pitch; }
            set
            {
                _pitch = value;
                //Pitch has influence on length so safety audio source needs it
                if (_safetyAudioSource != null) _safetyAudioSource.pitch = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.pitch = value;
                }
            }
        }

        [Range(0.0f, 1.1f)] [SerializeField] private float _reverbZoneMix = 1.0f;

        public float ReverbZoneMix
        {
            get { return _reverbZoneMix; }
            set
            {
                _reverbZoneMix = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.reverbZoneMix = value;
                }
            }
        }

        //3D sound settings
        [Range(0.0f, 5.0f)] [SerializeField] private float _dopplerLevel = 1.0f;

        public float DopplerLevel
        {
            get { return _dopplerLevel; }
            set
            {
                _dopplerLevel = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.dopplerLevel = value;
                }
            }
        }

        [Range(0.0f, 360.0f)] [SerializeField] private float _spread = 0.0f;

        public float Spread
        {
            get { return _spread; }
            set
            {
                _spread = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.spread = value;
                }
            }
        }

        [SerializeField] private AudioRolloffMode _volumeRolloff = AudioRolloffMode.Logarithmic;

        public AudioRolloffMode VolumeRolloff
        {
            get { return _volumeRolloff; }
            set
            {
                _volumeRolloff = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.rolloffMode = value;
                }
            }
        }

        [SerializeField] private float _minDistance = 1.0f;

        public float MinDistance
        {
            get { return _minDistance; }
            set
            {
                _minDistance = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.minDistance = value;
                }
            }
        }

        [SerializeField] private float _maxDistance = 500.0f;

        public float MaxDistance
        {
            get { return _maxDistance; }
            set
            {
                _maxDistance = value;
                foreach (var subAudioSource in _subAudioSources)
                {
                    subAudioSource.Value.maxDistance = value;
                }
            }
        }

        //Add custom curves when needed

        #endregion
        //Extra options
        public bool OnlyPlayForClosestCamera = false;

        //Internal components

        //Sometimes sound gets culled, because of too many sounds.
        //Every interval (from reboot time) we will try to reboot the culled audio
        //Note: Maybe a better system for automatic culling could be added in the future. The current system is far from perfect
        private const float RebootTime = 1.0f;
        private const float MaxStartRebootDelay = 1.0f;

        //Safety audiosource is a mute audio source who's only purpose is to keep track of the time in a sound. This audio source will always exist even if there are no virtual audio listeners
        //This does cost one of the 256 active sounds limit (for each active Multi Audio Source), so an improvement could be to only have one active when there are no subaudio sources
        private AudioSource _safetyAudioSource = null;

        private Dictionary<VirtualMultiAudioListener, AudioSource> _subAudioSources =
            new Dictionary<VirtualMultiAudioListener, AudioSource>();

        private bool _isPlaying = false;
        private bool _isPaused = false;

        public bool IsPaused
        {
            get { return _isPaused; }
        }

        public bool IsPlaying
        {
            get { return _isPlaying && !_isPaused; }
        }

        private Coroutine _update = null;

        private void Awake()
        {
            //Create the safety audio source
            bool hardWareChannelsLeft = false;
            _safetyAudioSource = CreateAudioSource(0, "Safety Audio Source", ref hardWareChannelsLeft);
            _safetyAudioSource.mute = true;

            //Priority safety audio is +1 of the normal priority.
            //This is because safety audio can become virtual before all other audio
            _safetyAudioSource.priority = Priority+1;
        }

        private void OnEnable()
        {
            if (_playOnAwake)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        private void OnDestroy()
        {
            //Stop all sounds and destroy the safety audio source
            if (_isPlaying)
            {
                Stop();
            }

            if (_safetyAudioSource != null)
            {
                MainMultiAudioListener.EnquequeAudioSourceInPool(_safetyAudioSource);
            }
        }

        /// <summary>
        /// Start playing the sound
        /// </summary>
        public void Play()
        {
            if (!_isPlaying)
            {
                _isPlaying = true;

                //We subscribe to these events so sub audio sources can be added or removed if needed
                MainMultiAudioListener.OnVirtualAudioListenerAdded += VirtualAudioListenerAdded;
                MainMultiAudioListener.OnVirtualAudioListenerRemoved += VirtualAudioListenerRemoved;

                //Play and start the play update
                _safetyAudioSource.Play();
                bool hardwareChannelsLeft = _safetyAudioSource.isPlaying;

                //Create all sub audio sources
                var virtualAudioListeners = MainMultiAudioListener.VirtualAudioListeners;
                for (int i = 0; i < virtualAudioListeners.Count; i++)
                {
                    CreateSubAudioSource(virtualAudioListeners[i],ref hardwareChannelsLeft);
                }

                if (_update == null)
                {
                    _update = StartCoroutine(PlayUpdate());
                }

            }
            else
            {
                //The sound was still playing so we let is play again from start
                _safetyAudioSource.Play();
                foreach (var audioSource in _subAudioSources)
                {
                    audioSource.Value.Play();
                }
            }

            _isPaused = false;
        }

        /// <summary>
        /// Stop playing the sound
        /// </summary>
        public void Stop()
        {
            if (!_isPlaying) return;
            _isPaused = false;
            _isPlaying = false;
            if (_update != null)
            {
                StopCoroutine(_update);
                _update = null;
            }

            MainMultiAudioListener.OnVirtualAudioListenerAdded -= VirtualAudioListenerAdded;
            MainMultiAudioListener.OnVirtualAudioListenerRemoved -= VirtualAudioListenerRemoved;

            //Remove all old subAudio
            foreach (var subAudioSource in _subAudioSources)
            {
                if (subAudioSource.Value != null)
                {
                    MainMultiAudioListener.EnquequeAudioSourceInPool(subAudioSource.Value);
                }
            }
            _subAudioSources.Clear();

            if (_safetyAudioSource != null)
            {
                _safetyAudioSource.Stop();
            }
        }

        /// <summary>
        /// Pause the sound
        /// </summary>
        public void Pause()
        {
            if (!_isPlaying || _isPaused) return;

            _isPaused = true;
            foreach (var subAudioSource in _subAudioSources)
            {
                subAudioSource.Value.Pause();
            }
            _safetyAudioSource.Pause();
        }

        /// <summary>
        /// Unpause the sound if it was paused
        /// </summary>
        public void UnPause()
        {
            if (!_isPaused) return;

            _isPaused = false;
            foreach (var subAudioSource in _subAudioSources)
            {
                subAudioSource.Value.UnPause();
            }
            _safetyAudioSource.UnPause();
        }

        private IEnumerator PlayUpdate()
        {
            float tryToRebootTimer = RebootTime + Random.Range(0, MaxStartRebootDelay);
            while (_safetyAudioSource.isPlaying || _isPaused||(_safetyAudioSource.loop&&_isPlaying))
            {
                yield return null;
                tryToRebootTimer -= Time.deltaTime;
                bool shouldRebootAudio = tryToRebootTimer <= 0;
                bool safetyWasRebooted = false;
                bool hardwareChannelsLeft = true;
                bool safetyIsPlaying = _safetyAudioSource.isPlaying;
                int correctTimesamples = _safetyAudioSource.timeSamples;

                //Closest audioCulling
                AudioSource closestAudio = null;
                float distanceClosestAudio=0;

                if (shouldRebootAudio)
                {
                    //If the safety was not playing try to reboot it
                    tryToRebootTimer += RebootTime;
                    if (!_safetyAudioSource.isPlaying)
                    {
                        _safetyAudioSource.Play();
                        safetyWasRebooted = true;
                        if (!_safetyAudioSource.isPlaying) hardwareChannelsLeft = false;
                    }
                }

                foreach (var subAudioSource in _subAudioSources)
                {
                    //We set the mute on the correct value before we cull
                    subAudioSource.Value.mute = Mute;

                    //ClosestAudioCulling
                    if (OnlyPlayForClosestCamera)
                    {
                        var distance = (subAudioSource.Key.transform.position - subAudioSource.Value.transform.position).sqrMagnitude;

                        if (closestAudio == null || distance < distanceClosestAudio)
                        {
                            closestAudio = subAudioSource.Value;
                            closestAudio.mute = Mute;
                            distanceClosestAudio = distance;
                            if (!closestAudio.isPlaying)
                            {
                                closestAudio.Play();
                            }
                        }
                        else
                        {
                            closestAudio.mute = true;
                        }
                    }
                    
                    if (shouldRebootAudio)
                    {
                        if (subAudioSource.Value.isPlaying)
                        {
                            //This subaudio was playing while the safety was off. This subaudio's timesample should have a more accurate timesamples
                            if (safetyWasRebooted && subAudioSource.Value.timeSamples > 0)
                                correctTimesamples = subAudioSource.Value.timeSamples;
                        }
                        else if(hardwareChannelsLeft)
                        {
                            //Reboot the culled audio
                            subAudioSource.Value.Play();
                            subAudioSource.Value.timeSamples = _safetyAudioSource.timeSamples;

                            //If this sound gets culled all following ones will be too
                            if (!subAudioSource.Value.isPlaying) hardwareChannelsLeft = false;
                        }
                    }
                    //Update position and volume
                    if (safetyIsPlaying&&correctTimesamples!=subAudioSource.Value.timeSamples)
                    {
                        subAudioSource.Value.timeSamples = correctTimesamples;
                    }
                    subAudioSource.Value.volume = Volume*subAudioSource.Key.Volume;
                    MoveSubAudioSourceToNeededLocation(subAudioSource.Key, subAudioSource.Value);
                }

                if (safetyWasRebooted)
                {
                    //We sync all timesamples

                    _safetyAudioSource.timeSamples = correctTimesamples;
                    foreach (var subAudioSource in _subAudioSources)
                    {
                        subAudioSource.Value.timeSamples = correctTimesamples;
                    }
                }

            }

            //The sound has stopped
            _update = null;
            Stop();
        }

        private void VirtualAudioListenerAdded(VirtualMultiAudioListener virtualAudioListener)
        {
            bool hardwareChannelsLeft = true;
            CreateSubAudioSource(virtualAudioListener,ref hardwareChannelsLeft);
        }

        private void VirtualAudioListenerRemoved(VirtualMultiAudioListener virtualAudioListener)
        {
            var audioSource = _subAudioSources[virtualAudioListener];
            _subAudioSources.Remove(virtualAudioListener);

            if (audioSource != null)
            {
                MainMultiAudioListener.EnquequeAudioSourceInPool(audioSource);
            }
        }

        private void CreateSubAudioSource(VirtualMultiAudioListener virtualAudioListener,ref bool hardWareChannelsLeft)
        {
            //Take time so that the new sub audio source starts at the correct time in the sound
            int timeSample = 0;
            if (_safetyAudioSource != null) timeSample = _safetyAudioSource.timeSamples;

            var audioSource = CreateAudioSource(timeSample, "Sub Audio Source",ref hardWareChannelsLeft);
            _subAudioSources.Add(virtualAudioListener, audioSource);
            audioSource.volume = Volume*virtualAudioListener.Volume;

            //Do transform
            MoveSubAudioSourceToNeededLocation(virtualAudioListener, audioSource);
            audioSource.playOnAwake = false;
        }

        private void MoveSubAudioSourceToNeededLocation(VirtualMultiAudioListener virtualListener,
            AudioSource subAudioSource)
        {
            //There is no main listener so translation is not needed
            if (MainMultiAudioListener.Main == null) return;

            //We calculate and translate the local pos of the audio from the virtual listener to the main listener
            var localPos = virtualListener.transform.InverseTransformPoint(transform.position);
            subAudioSource.transform.position = MainMultiAudioListener.Main.transform.TransformPoint(localPos);
        }

        private AudioSource CreateAudioSource(int timeSamples, string nameSubAudioSource,ref bool hardwareChannelsLeft)
        {
            AudioSource audioSource = MainMultiAudioListener.GetAudioSourceFromPool();
            //If no audiosource was given by pool, make a new one
            if (audioSource == null)
            {
                var subAudioSourceGameObject = new GameObject(nameSubAudioSource);
                audioSource = subAudioSourceGameObject.AddComponent<AudioSource>();
            }
            else
            {
                audioSource.gameObject.name = nameSubAudioSource;
            }
#if !ShowSubAudioSourcesInHierachy
            //We hide the sub audio source in hierarchy so that it doesn't flood it
            audioSource.gameObject.hideFlags = HideFlags.HideInHierarchy;
#endif

            SetAllValuesAudioSource(audioSource);

            if (_isPlaying&& hardwareChannelsLeft)
            {
                audioSource.Play();
                //If this sound gets culled all following will be too
                if (!audioSource.isPlaying)
                {
                    hardwareChannelsLeft = false;
                }
            }
            if (_isPaused) audioSource.Pause();
            audioSource.timeSamples = timeSamples;

            //All audio should be fully 3d
            audioSource.spatialBlend = 1.0f;
            return audioSource;
        }

        /// <summary>
        /// This refreshes all the properties of the subaudiosources, to guarantee that they are in sync with the main properties
        /// </summary>
        public void RefreshAllPropertiesAudioSources()
        {
            if (_safetyAudioSource != null)
            {
                _safetyAudioSource.loop = Loop;
                _safetyAudioSource.clip = AudioClip;
                _safetyAudioSource.pitch = Pitch;
                _safetyAudioSource.priority =Priority + 1;
            }

            foreach (var subAudioSource in _subAudioSources)
            {
                SetAllValuesAudioSource(subAudioSource.Value);
            }
        }

        private void SetAllValuesAudioSource(AudioSource audioSource)
        {
            audioSource.clip = AudioClip;
            audioSource.outputAudioMixerGroup = Output;
            audioSource.loop = Loop;
            audioSource.mute = Mute;
            audioSource.bypassEffects = BypassEffects;
            audioSource.bypassListenerEffects = BypassListenerEffects;
            audioSource.bypassReverbZones = BypassReverbZone;
            audioSource.pitch = Pitch;
            audioSource.priority = Priority;
            audioSource.reverbZoneMix = ReverbZoneMix;
            audioSource.dopplerLevel = DopplerLevel;
            audioSource.spread = Spread;
            audioSource.rolloffMode = VolumeRolloff;
            audioSource.minDistance = MinDistance;
            audioSource.maxDistance = MaxDistance;
        }
    }
}