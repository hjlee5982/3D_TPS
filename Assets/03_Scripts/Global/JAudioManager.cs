using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class JAudioManager : MonoBehaviour
{
    #region DATA
    public class AudioSettingData
    {
        public float BGM_Slider_Value;
        public float SFX_Slider_Value;
        //public float VOICE_Slider_Value;
        public bool  BGM_Toggle_Value;
        public bool  SFX_Toggle_Value;
        //public bool  VOICE_Toggle_Value;
    }
    #endregion





    #region SINGLETON
    public static JAudioManager Instance { get; private set; }

    private bool SingletonInitialize(bool dontDestroy = true)
    {
        if (Instance == null)
        {
            Instance = this;

            if(dontDestroy == true)
            {
                DontDestroyOnLoad(gameObject);
            }
            return true;
        }
        else
        {
            Destroy(gameObject);
            return false;
        }
    }
    #endregion





    #region VARIABLES
    [Header("오디오 믹서")]
    public AudioMixer AudioMixer = null;

    [Space(20)]
    [Header("BGM")]
    public  List<AudioClip>               BGM_ClipList = new List<AudioClip>();
    private Dictionary<string, AudioClip> BGM_Clips    = new Dictionary<string, AudioClip>();
    private AudioSource                   BGM_Source   = null;

    [Space(10)]
    [Header("SFX")]
    public  List<AudioClip>               SFX_ClipList       = new List<AudioClip>();
    private Dictionary<string, AudioClip> SFX_Clips          = new Dictionary<string, AudioClip>();
    private List<AudioSource>             SFX_Sources        = new List<AudioSource>();
    private int                           _sfxSourcePoolSize = 10;

    //[Space(10)]
    //[Header("VOICE")]
    //public  List<AudioClip>               VOICE_ClipList       = new List<AudioClip>();
    //private Dictionary<string, AudioClip> VOICE_Clips          = new Dictionary<string, AudioClip>();
    //private List<AudioSource>             VOICE_Sources        = new List<AudioSource>();
    //private int                           _voiceSourcePoolSize = 10;

    [Header("현재 볼륨값")]
    private float _current_BGM_Volume   = 0f;
    private float _current_SFX_Volume   = 0f;
    //private float _current_VOICE_Volume = 0f;

    [Header("페이드 이펙트")]
    private Coroutine _currentFadeCoroutine = null;
    #endregion





    #region MONOBEHAVIOUR
    private void Awake()
    {
        if (SingletonInitialize() == false)
        {
            return;
        }

        MakeDictionary();
    }

    private void Start()
    {
        MakeAudioSource();
    }
    #endregion





    #region FUNCTIONS
    private void MakeDictionary()
    {
        // BGM List -> Dictionary
        foreach(AudioClip clip in BGM_ClipList)
        {
            if(BGM_Clips.ContainsKey(clip.name) == false)
            {
                BGM_Clips.Add(clip.name, clip);
            }
            else
            {
                Debug.LogWarning($"[JAudioManager] : {clip.name} 이(가) 중복이에요!");
            }
        }
        // SFX List -> Dictionary
        foreach (AudioClip clip in SFX_ClipList)
        {
            if (SFX_Clips.ContainsKey(clip.name) == false)
            {
                SFX_Clips.Add(clip.name, clip);
            }
            else
            {
                Debug.LogWarning($"[JAudioManager] : {clip.name} 이(가) 중복이에요!");
            }
        }
        // // VOICE List -> Dictionary
        // foreach (AudioClip clip in VOICE_ClipList)
        // {
        //     if (VOICE_Clips.ContainsKey(clip.name) == false)
        //     {
        //         VOICE_Clips.Add(clip.name, clip);
        //     }
        //     else
        //     {
        //         Debug.LogWarning($"[JAudioManager] : {clip.name} 이(가) 중복이에요!");
        //     }
        // }
    }

    private void MakeAudioSource()
    {
        // Make BGM Source
        GameObject bgmObject = new GameObject("BGM_AudioSource");
        {
            bgmObject.transform.parent = transform;

            BGM_Source = bgmObject.transform.AddComponent<AudioSource>();

            AudioMixerGroup[] bgmGroups = AudioMixer.FindMatchingGroups("BGM");

            if(bgmGroups.Length > 0)
            {
                BGM_Source.outputAudioMixerGroup = bgmGroups[0];
                BGM_Source.loop = true;
            }
            else
            {
                Debug.LogWarning("[JAudioManager] : AudioMixer에 BGM 그룹이 없어요!");
            }
        }

        // Make SFX Source and Pool
        for(int i = 0; i < _sfxSourcePoolSize; ++i)
        {
            GameObject sfxObject = new GameObject($"SFX_AudioSource_{i}");
            {
                sfxObject.transform.parent = transform;

                AudioSource audioSource = sfxObject.transform.AddComponent<AudioSource>();

                AudioMixerGroup[] sfxGroups = AudioMixer.FindMatchingGroups("SFX");

                if (sfxGroups.Length > 0)
                {
                    audioSource.outputAudioMixerGroup = sfxGroups[0];
                    SFX_Sources.Add(audioSource);
                }
                else
                {
                    Debug.LogWarning("[JAudioManager] : AudioMixer에 SFX 그룹이 없어요!");
                }
            }
        }

        // // Make VOICE Source and Pool
        // for (int i = 0; i < _voiceSourcePoolSize; ++i)
        // {
        //     GameObject voiceObject = new GameObject($"VOICE_AudioSource_{i}");
        //     {
        //         voiceObject.transform.parent = transform;
        // 
        //         AudioSource audioSource = voiceObject.transform.AddComponent<AudioSource>();
        // 
        //         AudioMixerGroup[] voiceGroups = AudioMixer.FindMatchingGroups("VOICE");
        // 
        //         if (voiceGroups.Length > 0)
        //         {
        //             audioSource.outputAudioMixerGroup = voiceGroups[0];
        //             VOICE_Sources.Add(audioSource);
        //         }
        //         else
        //         {
        //             Debug.LogWarning("[JAudioManager] : AudioMixer에 VOICE 그룹이 없어요!");
        //         }
        //     }
        // }
    }

    public void InitializeAudioSettingData(AudioSettingData audioSettingData)
    {
        SetBGMVolume(audioSettingData.BGM_Slider_Value);
        SetSFXVolume(audioSettingData.SFX_Slider_Value);
        //SetVOICEVolume(audioSettingData.VOICE_Slider_Value);
        ToggleBGM   (audioSettingData.BGM_Toggle_Value);
        ToggleSFX   (audioSettingData.SFX_Toggle_Value);
        //ToggleVOICE (audioSettingData.VOICE_Toggle_Value);
    }

    public void SetBGMVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f + 5f;

        _current_BGM_Volume = dB;

        AudioMixer.SetFloat("BGM", dB);
    }

    public void SetSFXVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f + 5f;

        _current_SFX_Volume = dB;

        AudioMixer.SetFloat("SFX", dB);
    }

    //public void SetVOICEVolume(float volume)
    //{
    //    float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f + 5f;

    //    _current_VOICE_Volume = dB;

    //    AudioMixer.SetFloat("VOICE", dB);
    //}

    public void ToggleBGM(bool mute)
    {
        if(mute == true)
        {
            AudioMixer.GetFloat("BGM", out _current_BGM_Volume);
            AudioMixer.SetFloat("BGM", -75f);
        }
        else
        {
            AudioMixer.SetFloat("BGM", _current_BGM_Volume);
        }
    }

    public void ToggleSFX(bool mute)
    {
        if (mute == true)
        {
            AudioMixer.GetFloat("SFX", out _current_SFX_Volume);
            AudioMixer.SetFloat("SFX", -75f);
        }
        else
        {
            AudioMixer.SetFloat("SFX", _current_SFX_Volume);
        }
    }

    //public void ToggleVOICE(bool mute)
    //{
    //    if (mute == true)
    //    {
    //        AudioMixer.GetFloat("VOICE", out _current_VOICE_Volume);
    //        AudioMixer.SetFloat("VOICE", -75f);
    //    } 
    //    else
    //    {
    //        AudioMixer.SetFloat("VOICE", _current_VOICE_Volume);
    //    }
    //}

    public void PlayBGM(string name, bool fadeEffect = false, float duration = 1f)
    {
        if (BGM_Clips.TryGetValue(name, out AudioClip clip) == true)
        {
            if (fadeEffect == true)
            {
                if (_currentFadeCoroutine != null)
                {
                    StopCoroutine(_currentFadeCoroutine);
                }

                BGM_Source.clip = clip;
                BGM_Source.volume = 0f;
                BGM_Source.Play();

                _currentFadeCoroutine = StartCoroutine(FadeBGM(BGM_Source, 1f, duration));
            }
            else
            {
                BGM_Source.clip = clip;
                BGM_Source.Play();
            }
        }
        else
        {
            Debug.LogWarning($"[JAudioManager] : {name} BGM을 찾을 수 없어요!");
        }
    }

    public void StopBGM(bool fadeEffect = false, float duration = 1f)
    {
        if (BGM_Source.isPlaying == true)
        {
            if (fadeEffect == true)
            {
                if (_currentFadeCoroutine != null)
                {
                    StopCoroutine(_currentFadeCoroutine);
                }

                _currentFadeCoroutine = StartCoroutine(FadeBGM(BGM_Source, 0f, duration, () => BGM_Source.Stop()));
            }
            else
            {
                BGM_Source.Stop();
            }
        }
    }

    private IEnumerator FadeBGM(AudioSource audioSource, float targetVolume, float duration, Action onComplete = null)
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            // 오디오 믹서의 SetFloat는 로그로 변환해서 적용하는거라 Lerp와 쓰기 쉽지 않음
            // 반면에 리소스의 .volume은 0~1의 선형 범위로 다루기 떄문에 페이드 연출이 좋음
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
        onComplete?.Invoke();
        _currentFadeCoroutine = null;
    }

    public void PlaySFX(string name, bool fadeEffect = false, float duration = 1f)
    {
        if(SFX_Clips.TryGetValue(name, out AudioClip clip) == true)
        {
            AudioSource availableAudioSource = GetAvailableSFXSource();

            if (availableAudioSource != null)
            {
                if(fadeEffect == true)
                {
                    StartCoroutine(FadeSFX(availableAudioSource, clip, duration));
                }
                else
                {
                    availableAudioSource.PlayOneShot(clip);
                }
            }
            else
            {
                // TODO : 리소스를 새로 만드는 로직도 고려해볼 만 함
                Debug.LogWarning("SFX풀이 부족해요!");
            }
        }
        else
        {
            Debug.LogWarning($"[JAudioManager] : {name} SFX를 찾을 수 없어요!");
        }
    }

    private AudioSource GetAvailableSFXSource()
    {
        foreach(AudioSource audioSource in SFX_Sources)
        {
            if(audioSource.isPlaying == false)
            {
                return audioSource;
            }
        }
        return null;
    }

    private IEnumerator FadeSFX(AudioSource audioSource, AudioClip audioClip, float duration)
    {
        audioSource.volume = 1f;
        audioSource.clip = audioClip;
        audioSource.Play();

        float waitTime = Mathf.Max(audioClip.length - duration, 0f);

        yield return new WaitForSeconds(waitTime);

        float time = 0f;
        float startVolume = audioSource.volume;

        while(time < duration)
        {
            time += Time.deltaTime;

            // 오디오 믹서의 SetFloat는 로그로 변환해서 적용하는거라 Lerp와 쓰기 쉽지 않음
            // 반면에 리소스의 .volume은 0~1의 선형 범위로 다루기 떄문에 페이드 연출이 좋음
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 1f;
    }
    #endregion
}