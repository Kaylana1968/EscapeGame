using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class ThronePuzzle : MonoBehaviour
{
    [SerializeField] List<XRSocketInteractor> sockets;
    [SerializeField] GameObject wall;
    [SerializeField] TeleportationArea teleportationArea;
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
        teleportationArea.enabled = true;
    }
}
