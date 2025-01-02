using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public GameObject talkUI;
    public PlayerInput playerInput;
    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Enable();
    }
}
