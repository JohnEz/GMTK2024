using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioClipOptions {
    public float Volume { get; set; } = 1f;
    public float Pitch { get; set; } = 1f;
    public float Delay { get; set; } = 0f;

    public bool RandomPitch = false;

    public float PitchRange = 0.1f;

    public bool Loop = false;
    public bool Persist = false;
}

public class AudioManager : Singleton<AudioManager> {
    private List<AudioSource> sounds = new List<AudioSource>();

    [SerializeField]
    private AudioMixerGroup masterMixer;

    [SerializeField]
    private AudioMixerGroup musicMixer;

    [SerializeField]
    private AudioMixerGroup sfxMixer;

    public bool IsMusicMuted { get; private set; } = false;

    public bool IsSoundMuted { get; private set; } = false;

    public void Start() {
        SetMasterVolume(60);
        SetMusicVolume(40);
    }

    private AudioClip GetRandomClip(List<AudioClip> clips) {
        int randomIndex = Random.Range(0, clips.Count);
        return clips[randomIndex];
    }

    private AudioSource FindSource(AudioClip clip) {
        AudioSource audioSource = sounds.Find(delegate (AudioSource source) {
            if (source != null) {
                return source.clip == clip;
            }
            return false;
        });
        return audioSource;
    }

    public void PlaySound(AudioClip clip, AudioClipOptions options = null) {
        PlaySound(clip, Camera.main.transform, options);
    }

    public void PlaySound(List<AudioClip> clips, AudioClipOptions options = null) {
        //get a random sound clip
        var clip = GetRandomClip(clips);

        PlaySound(clip, Camera.main.transform, options);
    }

    public void PlaySound(List<AudioClip> clips, Vector3 worldPosition, AudioClipOptions options = null) {
        //get a random sound clip
        var clip = GetRandomClip(clips);

        PlaySound(clip, worldPosition, options);
    }

    public void PlaySound(AudioClip clip, Vector3 worldPosition, AudioClipOptions options = null) {
        if (!clip) {
            return;
        }

        GameObject soundGameObject = CreateSoundGameObject(clip.name, worldPosition);
        CreateAudioSource(clip, soundGameObject, options);
    }

    public void PlaySound(AudioClip clip, Transform parent, AudioClipOptions options = null) {
        if (!clip) {
            return;
        }

        GameObject soundGameObject = CreateSoundGameObject(clip.name, parent);
        CreateAudioSource(clip, soundGameObject, options);
    }

    public void StopSound(AudioClip clip) {
        StopAudioSource(clip);
    }

    private GameObject CreateSoundGameObject(string clipName, Vector3 worldPosition) {
        GameObject soundGameObject = new GameObject("Sound " + clipName);
        soundGameObject.transform.position = worldPosition;

        return soundGameObject;
    }

    private GameObject CreateSoundGameObject(string clipName, Transform parent) {
        GameObject soundGameObject = new GameObject("Sound " + clipName);
        soundGameObject.transform.SetParent(parent, false);

        return soundGameObject;
    }

    private void CreateAudioSource(AudioClip clip, GameObject audioObject, AudioClipOptions options) {
        AudioClipOptions audioOptions = options != null ? options : new AudioClipOptions();

        float pitch = audioOptions.RandomPitch ?
            Random.Range(audioOptions.Pitch - audioOptions.PitchRange, audioOptions.Pitch + audioOptions.PitchRange) :
            audioOptions.Pitch;

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;
        audioSource.outputAudioMixerGroup = sfxMixer;
        audioSource.pitch = pitch;
        audioSource.volume = audioOptions.Volume;
        audioSource.loop = audioOptions.Loop;
        audioSource.PlayDelayed(audioOptions.Delay);

        if (audioOptions.Persist) {
            sounds.Add(audioSource);
        }

        Destroy(audioObject, audioSource.clip.length + audioOptions.Delay);
    }

    private void StopAudioSource(AudioClip clip) {
        AudioSource audioSource = FindSource(clip);
        sounds.Remove(audioSource);
        audioSource.Stop();
    }

    private void StopAudioSource(AudioSource source) {
        sounds.Remove(source);
        source.Stop();
    }

    public void SetMasterVolume(float volume) {
        float MAX_VOLUME = 20f;
        float MIN_VOLUME = -50f;

        float newVolume = GetMixerVolume(volume / 100, MIN_VOLUME, MAX_VOLUME);

        if (newVolume == MIN_VOLUME) {
            newVolume = -80;
        }

        masterMixer.audioMixer.SetFloat("MasterVolume", newVolume);
    }

    public void SetMusicVolume(float volume) {
        float MAX_VOLUME = 20f;
        float MIN_VOLUME = -50f;

        float newVolume = GetMixerVolume(volume / 100, MIN_VOLUME, MAX_VOLUME);

        if (newVolume == MIN_VOLUME) {
            newVolume = -80;
        }

        musicMixer.audioMixer.SetFloat("MusicVolume", newVolume);
    }

    public void SetSfxVolume(float volume) {
        float MAX_VOLUME = 0f;
        float MIN_VOLUME = -50f;

        float newVolume = GetMixerVolume(volume / 100, MIN_VOLUME, MAX_VOLUME);

        if (newVolume == MIN_VOLUME) {
            newVolume = -80;
        }

        sfxMixer.audioMixer.SetFloat("SFXVolume", newVolume);
    }

    private float GetMixerVolume(float percent, float min, float max) {
        float VOLUME_RANGE = max - min;

        return min + (VOLUME_RANGE * percent);
    }

    public IEnumerator FadeOut(AudioClip clip, float duration) {
        AudioSource audioSource = FindSource(clip);
        float currentTime = 0;
        float targetVolume = 0;
        float start = audioSource.volume;
        while (currentTime < duration) {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        StopAudioSource(audioSource);
        yield break;
    }

    public IEnumerator FadeIn(AudioClip clip, float duration, float targetVolume) {
        AudioClipOptions options = new AudioClipOptions();

        options.Delay = 1f;
        options.Loop = true;
        options.Persist = true;
        options.Volume = 0f;

        GameObject soundGameObject = CreateSoundGameObject(clip.name, Camera.main.transform);
        CreateMusicAudioSource(clip, soundGameObject, options);

        AudioSource audioSource = FindSource(clip);
        audioSource.loop = true;
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration) {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    private void CreateMusicAudioSource(AudioClip clip, GameObject audioObject, AudioClipOptions options) {
        AudioClipOptions audioOptions = options != null ? options : new AudioClipOptions();

        float pitch = audioOptions.RandomPitch ?
            Random.Range(audioOptions.Pitch - audioOptions.PitchRange, audioOptions.Pitch + audioOptions.PitchRange) :
            audioOptions.Pitch;

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;
        audioSource.outputAudioMixerGroup = musicMixer;
        audioSource.pitch = pitch;
        audioSource.volume = audioOptions.Volume;
        audioSource.loop = audioOptions.Loop;
        audioSource.PlayDelayed(audioOptions.Delay);

        if (audioOptions.Persist) {
            sounds.Add(audioSource);
        }
    }

    public void ToggleMusic() {
        float currentVolume;
        musicMixer.audioMixer.GetFloat("MusicVolume", out currentVolume);

        if (currentVolume > -80f) {
            SetMusicVolume(0);
            IsMusicMuted = true;
        } else {
            SetMusicVolume(40);
            IsMusicMuted = false;
        }
    }

    public void ToggleSFX() {
        float currentVolume;
        musicMixer.audioMixer.GetFloat("SFXVolume", out currentVolume);

        if (currentVolume > -80f) {
            SetSfxVolume(0);
            IsSoundMuted = true;
        } else {
            SetSfxVolume(60);
            IsSoundMuted = false;
        }
    }
}