using UnityEngine;

public class PlayerDialogueLock : MonoBehaviour
{
    public bool IsMovementLocked { get; private set; }

    public void LockMovement()
    {
        IsMovementLocked = true;
    }

    public void UnlockMovement()
    {
        IsMovementLocked = false;
    }
}