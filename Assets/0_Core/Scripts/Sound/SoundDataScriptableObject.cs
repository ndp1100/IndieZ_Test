using Cysharp.Text;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "config/SoundConfig")]
public class SoundDataScriptableObject : ScriptableObject
{
    public enum SoundActionType
    {
        PLAY = 0,
        PAUSE,
        STOP
    }

    public enum SoundType
    {
        SOUND_FX = 0,
        SOUND_MUSIC
    }

    public enum SoundFXID
    {
        NONE = 0,
        SOUNDFX_AssaultShot,
        SOUNDFX_SniperShot,
        SOUNDFX_Reload,
        SOUNDFX_Explosion,
    }

    public enum SoundMusicID
    {
        NONE = 0,
        BGM_MainPlay,
    }

    [Serializable]
    public struct SoundFXDataItem
    {
        public SoundFXID SoundFXID;
        public AudioClip AudioClip;
    }

    [Serializable]
    public struct SoundMusicDataItem
    {
        public SoundMusicID SoundMusicID;
        public AudioClip AudioClip;
    }

    [Serializable]
    public struct SoundSignalData
    {
        public SoundActionType SoundActionType { get; private set; }
        public SoundType SoundType { get; private set; }
        public SoundFXID SoundFXId { get; private set; }
        public SoundMusicID SoundMusicID { get; private set; }
        public float Volume { get; private set; }


        public SoundSignalData(SoundActionType _soundActionType, SoundType _soundType, SoundFXID soundFxId,
            SoundMusicID _soundMusicId, float volume = 1f)
        {
            this.SoundActionType = _soundActionType;
            this.SoundType = _soundType;
            this.SoundFXId = soundFxId;
            this.SoundMusicID = _soundMusicId;
            this.Volume = volume;
        }

        public override string ToString()
        {
            string message = ZString.Format("{0}/{1}/{2}/{3}", this.SoundActionType.ToString(),
                this.SoundType.ToString(),
                this.SoundFXId.ToString(), this.SoundMusicID.ToString());
            return message;
        }
    }

    public List<SoundFXDataItem> SoundFXItems;
    public List<SoundMusicDataItem> SoundMusicItems;

    public AudioClip GetSoundFXAudioClip(SoundFXID soundId)
    {
        var _soundDataItem = SoundFXItems.FindLast(e => e.SoundFXID == soundId);
        if (_soundDataItem.AudioClip == null)
        {
            Debug.LogError("SoundFXID: " + soundId + " not found in SoundDataScriptableObject");
            return null;
        }

        return _soundDataItem.AudioClip;
    }

    public AudioClip GetSoundMusicAudioClip(SoundMusicID soundId)
    {
        var _soundDataItem = SoundMusicItems.FindLast(e => e.SoundMusicID == soundId);
        if (_soundDataItem.AudioClip == null)
        {
            Debug.LogError("SoundMusicID: " + soundId + " not found in SoundDataScriptableObject");
            return null;
        }

        return _soundDataItem.AudioClip;
    }
}