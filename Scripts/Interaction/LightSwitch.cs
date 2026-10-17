
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LightSwitch : MonoBehaviour, IInteractables
{
    public Light lightSource;

    [Header("Emission")]
    public Renderer[] emissionObjects;

    private Material[] emissionMaterials;

    [Header("Switch Audio")]
    public AudioClip switchSound;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        emissionMaterials = new Material[emissionObjects.Length];

        for (int i = 0; i < emissionObjects.Length; i++)
        {
            if (emissionObjects[i] != null)
                emissionMaterials[i] = emissionObjects[i].material;
        }
    }

    public void Interact()
    {
        if (lightSource == null)
            return;

        bool turnOn = !lightSource.enabled;

        // Toggle the actual light
        lightSource.enabled = turnOn;

        // Play the switch sound
        if (switchSound != null)
            audioSource.PlayOneShot(switchSound);

        // Toggle emission
        foreach (Material material in emissionMaterials)
        {
            if (material == null) continue;

            if (turnOn)
                material.EnableKeyword("_EMISSION");
            else
                material.DisableKeyword("_EMISSION");
        }
    }
}