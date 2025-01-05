using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public Dialogsystem1 dialogSystem; // 对话系统的引用
    private bool isPlayerInRange = false; // 玩家是否在范围内

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) // 按E键触发对话
        {
            dialogSystem.ShowPanel(); // Panel 显示
            dialogSystem.StartDialogue(); // 开始对话
        }
        // 按ESC键退出对话
        if (dialogSystem.isInDialogue && Input.GetKeyDown(KeyCode.Escape)) // 对话中按ESC退出
        {
            dialogSystem.EndDialogue(); // 结束对话
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 确保是玩家进入触发器
        {
            isPlayerInRange = true;
            // 可以在UI中显示提示：按E与NPC对话
            dialogSystem.ShowInteractionPrompt(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // 可以在UI中隐藏提示
            dialogSystem.ShowInteractionPrompt(false);
        }
    }
}
