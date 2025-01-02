using NodeCanvas.DialogueTrees;
using NodeCanvas.DialogueTrees.UI.Examples;
using UnityEngine;

public class NpcTalk : MonoBehaviour
{
    HUD hud;
    DialogueUGUI ugui;
    [SerializeField]DialogueTreeController dialogue;

    private void Awake()
    {
        hud = FindObjectOfType<HUD>();
        ugui = FindAnyObjectByType<DialogueUGUI>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hud.talkUI.SetActive(true);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")&&hud.playerInput.Gameplay.Interact.WasPressedThisFrame())
        {
            Debug.Log("按下对话键");
            dialogue.StartDialogue();
            hud.talkUI.SetActive(false);
        }
        if (!ugui.isRunning)
        {
            hud.talkUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hud.talkUI.SetActive(false);
        }
    }
}
