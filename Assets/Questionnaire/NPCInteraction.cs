using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public Dialogsystem1 dialogSystem; // �Ի�ϵͳ������
    private bool isPlayerInRange = false; // ����Ƿ��ڷ�Χ��

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) // ��E�������Ի�
        {
            dialogSystem.ShowPanel(); // Panel ��ʾ
            dialogSystem.StartDialogue(); // ��ʼ�Ի�
        }
        // ��ESC���˳��Ի�
        if (dialogSystem.isInDialogue && Input.GetKeyDown(KeyCode.Escape)) // �Ի��а�ESC�˳�
        {
            dialogSystem.EndDialogue(); // �����Ի�
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ȷ������ҽ��봥����
        {
            isPlayerInRange = true;
            // ������UI����ʾ��ʾ����E��NPC�Ի�
            dialogSystem.ShowInteractionPrompt(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // ������UI��������ʾ
            dialogSystem.ShowInteractionPrompt(false);
        }
    }
}
