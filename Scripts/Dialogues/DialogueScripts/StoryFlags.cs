using System.Collections.Generic;
using UnityEngine;

public class StoryFlags : MonoBehaviour
{
    public static StoryFlags Instance;

    private Dictionary<string, bool> flags =
        new Dictionary<string, bool>();

    void Awake()
    {
        Instance = this;
    }

    // For testing purposes
    // void Start()
    // {
    //     SetFlag("TestFlag", true);

    //     Debug.Log(
    //         "TestFlag is: " +
    //         GetFlag("TestFlag")
    //     );
    // }

    public void SetFlag(string flagName, bool value)
    {
        if (string.IsNullOrEmpty(flagName))
            return;

        flags[flagName] = value;

        Debug.Log(
            "Story Flag: " +
            flagName +
            " = " +
            value
        );
    }

    public bool GetFlag(string flagName)
    {
        if (string.IsNullOrEmpty(flagName))
            return false;

        if (flags.TryGetValue(flagName, out bool value))
            return value;

        return false;
    }

    public bool HasFlag(string flagName)
    {
        return GetFlag(flagName);
    }

    public void ClearFlag(string flagName)
    {
        if (flags.ContainsKey(flagName))
            flags.Remove(flagName);
    }

    public void ClearAllFlags()
    {
        flags.Clear();
    }
}