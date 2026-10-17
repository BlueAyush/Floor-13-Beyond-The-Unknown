using System.Collections;
using UnityEngine;

public class ElevatorButtonLight : MonoBehaviour
{
    [Header("Button Renderer")]
    public Renderer buttonRenderer;

    [Header("Colors")]
    public Color blueColor = Color.blue;
    public Color redColor = Color.red;

    [Header("Flicker")]
    public float flickerSpeed = 0.25f;

    private Material buttonMaterial;
    private Coroutine flickerRoutine;

    void Start()
    {
        buttonMaterial = buttonRenderer.material;

        SetBlue();
    }

    public void StartFlicker()
    {
        if (flickerRoutine != null)
            StopCoroutine(flickerRoutine);

        flickerRoutine = StartCoroutine(Flicker());
    }

    public void StopFlicker()
    {
        if (flickerRoutine != null)
        {
            StopCoroutine(flickerRoutine);
            flickerRoutine = null;
        }

        SetBlue();
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            SetRed();
            yield return new WaitForSeconds(flickerSpeed);

            SetBlue();
            yield return new WaitForSeconds(flickerSpeed);
        }
    }

    void SetBlue()
    {
        buttonMaterial.SetColor("_EmissionColor", blueColor);
    }

    void SetRed()
    {
        buttonMaterial.SetColor("_EmissionColor", redColor);
    }
}