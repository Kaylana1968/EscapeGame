using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ThronePuzzle : MonoBehaviour
{
    [SerializeField] List<XRSocketInteractor> sockets;
    [SerializeField] GameObject wall;
    private bool _areSocketsFilled;

    // Update is called once per frame
    void Update()
    {
        if (CheckAllSockets() && !_areSocketsFilled)
        {
            _areSocketsFilled = true;
            OnPuzzleSolved();
        }
    }

    private bool CheckAllSockets()
    {
        foreach (var socket in sockets)
        {
            if (!socket.hasSelection)
            {
                return false;
            }
        }

        return true;
    }

    private void OnPuzzleSolved()
    {
        Destroy(wall);
    }
}
