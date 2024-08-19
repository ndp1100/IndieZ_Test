using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static SoundDataScriptableObject;

public class SoundManager : Singleton<SoundManager>
{
    public SoundDataScriptableObject soundResource;

    [SerializeField]
    private AudioSource SoundFXSource;
    [SerializeField]
    private AudioSource MusicFXSource;

    [Space(10)]
    [Tooltip("Using effect FadeIn and FadeOut for Playing Music backgroud")]
    public bool FadeInFadeOutBGM = true;

    [ShowIf("FadeInFadeOutBGM")]
    [Range(0.1f, 1)]
    public float FadeOutTime = 0.25f;
    [ShowIf("FadeInFadeOutBGM")]
    [Range(0.1f, 1)]
    public float FadeInTime = 0.5f;

    private void ProcessSoundAction(SoundSignalData data)
    {
        if (data.SoundActionType == SoundActionType.PLAY)
        {
            if (data.SoundType == SoundType.SOUND_FX)
            {
                var audioClip = soundResource.GetSoundFXAudioClip(data.SoundFXId);
                if (audioClip != null)
                {
                    SoundFXSource.PlayOneShot(audioClip, data.Volume);
                }
                else
                {
                    YOLogger.Error("SoundFXID: " + data.SoundFXId + " not found in SoundDataScriptableObject");
                }
            }
            else //play music
            {
                var audioClip = soundResource.GetSoundMusicAudioClip(data.SoundMusicID);
                if (audioClip != null)
                {
                    if (FadeInFadeOutBGM)
                    {
                        FadeInPlayMusic(audioClip, FadeOutTime, FadeInTime);
                    }
                    else
                    {
                        MusicFXSource.clip = audioClip;
                        MusicFXSource.Play();
                    }
                }
                else
                {
                    YOLogger.Error("SoundMusicID: " + data.SoundMusicID + " not found in SoundDataScriptableObject");
                }
            }
        }
        else if (data.SoundActionType == SoundActionType.STOP)
        {
            if (data.SoundType == SoundType.SOUND_FX)
            {
                SoundFXSource.Stop();
            }
            else 
            {
                MusicFXSource.Stop();
            }
        }
        else if (data.SoundActionType == SoundActionType.PAUSE)
        {
            if (data.SoundType == SoundType.SOUND_FX)
            {
                SoundFXSource.Pause();
            }
            else 
            {
                MusicFXSource.Pause();
            }
        }

    }

    #region public Methods
    public void DoActionSound(SoundSignalData data)
    {
        ProcessSoundAction(data);
    }

    public void PlaySoundFX(SoundFXID soundFX)
    {
        SoundSignalData data = new SoundSignalData(SoundActionType.PLAY, SoundType.SOUND_FX, soundFX, SoundMusicID.NONE);
        ProcessSoundAction(data);
    }

    public void PlaySoundFX(SoundFXID soundFX, float volume)
    {
        SoundSignalData data = new SoundSignalData(SoundActionType.PLAY, SoundType.SOUND_FX, soundFX, SoundMusicID.NONE, volume);
        ProcessSoundAction(data);
    }

    public void PauseBGM()
    {
        MusicFXSource.Pause();
    }

    public void ResumeBGM()
    {
        fadeInCoroutine = StartCoroutine(Fadein(0.5f, MusicFXSource.clip));
    }

    public void StopBGM()
    {
        MusicFXSource.Stop();
    }

    #endregion

    #region Utils
    //Caching
    private Coroutine fadeOutCoroutine = null;
    private Coroutine fadeInCoroutine = null;
    private static float timeStep = 0.01f;
    private WaitForSeconds waitTimeStep = new WaitForSeconds(timeStep);

    private void FadeInPlayMusic(AudioClip fadeinAudioClip, float fadeoutTime, float fadeinTime)
    {
        if (fadeOutCoroutine != null) StopCoroutine(fadeOutCoroutine);
        fadeOutCoroutine = StartCoroutine(Fadeout(fadeoutTime, () =>
        {
            if (fadeInCoroutine != null) StopCoroutine(fadeInCoroutine);
            fadeInCoroutine = StartCoroutine(Fadein(fadeinTime, fadeinAudioClip));
        }));
    }

    IEnumerator Fadeout(float fadeoutTime, UnityAction callback)
    {
        if (MusicFXSource.isPlaying == false || fadeoutTime <= 0)
            callback.Invoke();
        else
        {
            float _time = fadeoutTime;
            while (_time >= 0)
            {
                yield return waitTimeStep;
                _time -= timeStep;
                MusicFXSource.volume = Mathf.Clamp01(_time / fadeoutTime);
            }

            MusicFXSource.volume = 0;
            callback.Invoke();
        }
    }

    IEnumerator Fadein(float fadeinTime, AudioClip audioClip)
    {
        MusicFXSource.clip = audioClip;
        MusicFXSource.volume = 0;

        MusicFXSource.Play();

        if (fadeinTime <= 0)
        {
            MusicFXSource.volume = 1;
            yield break;
        }

        float _time = 0;
        while (_time <= fadeinTime)
        {
            yield return waitTimeStep;
            _time += timeStep;
            MusicFXSource.volume = Mathf.Clamp01(_time / fadeinTime);
        }
        MusicFXSource.volume = 1;
    }


    #endregion
}
