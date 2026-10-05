using UnityEngine;

public class LeverPuzzle : MonoBehaviour
{
    [SerializeField]
    bool[] solution = { true, false, true };
    [SerializeField]
    DoorController doorController;

    readonly bool[] guess = { false, false, false };

    bool CheckSolved()
    {
        for (int i = 0; i < 3; i++)
        {
            if (solution[i] != guess[i]) return false;
        }

        return true;
    }

    public void OnLeverActivate(int index)
    {
        guess[index] = true;

        if (CheckSolved())
        {
            doorController.OpenDoorAutomatic(2f);
        }
    }

    public void OnLeverDeactivate(int index)
    {
        guess[index] = false;

        if (CheckSolved())
        {
            doorController.OpenDoorAutomatic(2f);
        }
    }
}
