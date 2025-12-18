using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vs;

public class SoundMixerBehaviour : MonoBehaviour
{
    public Slider SliderBgm;
    public Slider SliderSE;

    private static SoundMixerBehaviour instance;

    public static SoundMixerBehaviour Instance
    {
        get
        {
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        SliderBgm.value = SoundService.Instance.getBGMVol();
        SliderSE.value = SoundService.Instance.getSEVol();
    }

    public void onChangeBGM()
    {
        Debug.Log("onChangeBGM run..");

        float volume = SliderBgm.value;
        SoundService.Instance.SetBgmVolume(volume);

    }

    public void onChangeSE()
    {
        Debug.Log("onChangeSE run..");

        float volume = SliderSE.value;
        SoundService.Instance.SetSeVolume(volume);
    }


}
