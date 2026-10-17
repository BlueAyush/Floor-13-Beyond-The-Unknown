
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FanSwitch : MonoBehaviour, IInteractables
{
    public FanBlades fanBlades;

    [Header("Switch Audio")]
    public AudioClip switchSound;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void Interact()
    {
        if (fanBlades != null)
        {
            fanBlades.ToggleFan();
        }

        if (switchSound != null)
        {
            audioSource.PlayOneShot(switchSound);
        }
    }
}