using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundManager : Singleton<SoundManager>
{
    [Header("Audio Sources")]
    [Space(5)]
    public AudioSource Looper;
    public AudioSource OneShotLooper;

    [Space(5)]
    [Header("BG Sounds")]
    [Space(5)]

    public AudioClip[] BGz;

    [Space(5)]
    [Header("OneShot Sounds")]
    [Space(5)]

    public AudioClip[] Buttons;
    public AudioClip Swish;
    public AudioClip LevelCompleted;
    public AudioClip Sparkle;
    public AudioClip PackingSound;
    public AudioClip RailSound;
    public AudioClip PuffSound;
    public AudioClip CoinCollect;
    public AudioClip FlasherSound;




    #region BackGroundSoundMethods

    public void PlayBackGround(int index)
    {
        Looper.clip = BGz[index];
        Looper.Play();
    }



    #endregion




    #region OneShotSoundMethods

    /*
	 * Playing one shot for each OneShotSound.
	 */


    //-------------------------------------------------


    public void PlayOneShots(AudioClip toPlay)
    {
        this.GetComponent<AudioSource>().PlayOneShot(toPlay);
    }

    /*
    * Playing one shot for looping.
    */


    //-------------------------------------------------

    public void PlayToggleForOneShotSloop(AudioClip clips)
    {
        if (clips != null)
        {
            if (OneShotLooper.isPlaying)
            {
                OneShotLooper.Stop();
            }
            else
            {
                OneShotLooper.clip = clips;
                OneShotLooper.Play();
            }
        }
        else
        {
            OneShotLooper.Stop();
        }
    }


    #endregion


    #region Helpers

    // You can add your custom helper methods here e.g Mute Background, Mute SFx etc

    #endregion

}