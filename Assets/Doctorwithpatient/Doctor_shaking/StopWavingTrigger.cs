using UnityEngine;

public class StopWavingTrigger : MonoBehaviour
{
    public NPCAnimationController npcController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            npcController.StopWaving();
        }
    }
}