using UnityEngine;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using DigitalRuby.SoundManagerNamespace;

public class SoundCtrl : ObjectSingleton<SoundCtrl>
{
    public enum SFX
    {
        COMBO = 0,
        FEVER,
        DROP_GEM,

        SFX_COLORBOMB,
        SFX_ENERGYBOMB,

        SFX_TICKING,
        SFX_TIMEOVER,
        SFX_JELLYPANG
    }

    public AudioSource[] SFXSources;

    public void PlaySound(SFX sfx)
    {
        AudioSource source = SFXSources[UnsafeUtility.EnumToInt(sfx)];
        if(source.loop)
            source.PlayLoopingSoundManaged();
        else
            source.PlayOneShotSoundManaged(source.clip);
    }

    public void StopSound(SFX sfx)
    {
        AudioSource source = SFXSources[UnsafeUtility.EnumToInt(sfx)];
        source.Stop();
    }

    public void PlayComboSFX(int curCombo)
    {
        if (curCombo <= 1)
            return;

        AudioSource source = SFXSources[UnsafeUtility.EnumToInt(SFX.COMBO)];
        source.pitch = 0.9f + ((curCombo - 1) / (float)Define.FEVER_COMBO);
        source.PlayOneShotMusicManaged(source.clip);
    }
}
