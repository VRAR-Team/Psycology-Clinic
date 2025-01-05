using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class DialogsystemX : MonoBehaviour
{
    [Header("UI组件")]
    public TextMeshProUGUI textLabel;
    public Image faceImage;
    public GameObject promptUI; // 提示UI
    public GameObject promptUI1; // 提示UI
    public GameObject choicePanel; // 显示选项的面板
    public TextMeshProUGUI[] choiceTexts; // 存放每个选项的文本
    public GameObject panel;      // 引用 Panel
    [Header("文本文件")]
    public TextAsset textFile;
    private int index;
    private List<string> textList = new List<string>();
    [Header("按钮组件")]
    public Button continueButton; // 继续对话按钮
    public Button backButton;     // 返回上一段对话按钮
    public Button outButton;     // 退出按钮

    public bool isInDialogue = false; // 判断是否正在对话
    private bool isChoosing = false; // 判断是否在选择阶段
                                     // 用来记录退出时的状态
    private int savedIndex = 0;
    private bool savedIsChoosing = false;
    // 记录玩家的选择
    // 存储每个问题的选择结果，键为问题索引，值为选择的索引
    private Dictionary<int, int> choicesResults = new Dictionary<int, int>();

    void Start()
    {
        GetTextFromFile(textFile);
        isInDialogue = false;
        panel.SetActive(false); // Panel 不显示
        index = 0;
        promptUI.SetActive(false); // 初始时提示UI不可见
        promptUI1.SetActive(false); // 显示退出提示UI
        choicePanel.SetActive(false); // 初始时选项面板不可见
                                      // 按钮事件绑定
        continueButton.onClick.AddListener(OnContinueButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);
        outButton.onClick.AddListener(OutButtonClick); ;     // 退出按钮

}
    // 点击继续对话按钮
    void OutButtonClick()
    {
        if (isInDialogue)
        {
            EndDialogue(); // 继续对话
        }
    }

    // 点击继续对话按钮
    void OnContinueButtonClick()
    {
        if (isInDialogue)
        {
            ShowNextDialogue(); // 继续对话
        }
    }

    // 点击返回上一段对话按钮
    void OnBackButtonClick()
    {
        if (isInDialogue && index > 2)
        {
            GoBackToPreviousQuestion(); // 继续对话
                                        // 返回上一段对话
        }
    }

    void Update()
    {
        // 当按下 E键并且玩家接近 NPC，开始对话
      if (Input.GetKeyDown(KeyCode.E) && !isInDialogue)
        {
      
            StartDialogue();
   
        }
        // 如果正在对话，并且玩家按下 R 键
       // if (isInDialogue && !isChoosing && Input.GetKeyDown(KeyCode.R))
       //{

            //ShowNextDialogue();
        //}
        // 监听返回上一个问题的按键（如 Backspace 或 Esc）
        
        if (isChoosing)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectChoice(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) SelectChoice(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) SelectChoice(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) SelectChoice(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5)) SelectChoice(4);
            //else if (Input.GetKeyDown(KeyCode.Space)) GoBackToPreviousQuestion();
        }


    }
    public void ShowPanel()
    {
        panel.SetActive(true); // 显示 Panel，包括其中的 Image
       
    }
    void GetTextFromFile(TextAsset file)
    {
        textList.Clear();
        index = 0;
        var lines = file.text.Split('\n');
        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                textList.Add(line.Trim()); // 添加每行对话内容（仅问题）
            }
        }
    }


    // 显示下一段对话
    void ShowNextDialogue()
    {

        if (index < textList.Count)
        {
            string currentText = textList[index];
            DisplayQuestion(currentText);

        }
        else
        {
            // 对话结束，隐藏对话框
            EndDialogue();
        }
    }

    void DisplayQuestion(string question)
    {
        textLabel.text = question; // 显示当前问题

        isChoosing = true; // 进入选择阶段
        choicePanel.SetActive(true); // 显示选项面板

        // 固定的5个选项
        string[] options = new string[] { "从无", "很轻", "中等", "偏重", " 严重" };

        for (int i = 0; i < options.Length; i++)
        {
            choiceTexts[i].text = options[i]; // 设置每个选项的文本
        }
        // 如果已有选择，恢复选中状态
       // if (choicesResults.ContainsKey(index))
       // {
       // HighlightChoice(choicesResults[index]);
       // }

        index++; // 跳过当前问题

    }

    // 显示对话（主角的台词）
    void DisplayDialogue(string dialogue)
    {
        textLabel.text = dialogue;
        index++; // 继续到下一行对话
   
    }

    void SelectChoice(int choiceIndex)
    {
        // 处理选择的逻辑：这里可以根据需要扩展更多内容
        isChoosing = false; // 退出选择阶段
                            // 重置所有选项的状态（恢复原大小和颜色）
                            // 保存玩家选择的答案
                            // 更新选择结果，如果已存在该问题的选择，则覆盖
        if (choicesResults.ContainsKey(index - 1)) // index - 1 是当前问题的索引
        {
            choicesResults[index - 1] = choiceIndex;
        }
        else
        {
            choicesResults.Add(index - 1, choiceIndex);
        }
        // 让选中的选项添加动画（例如放大）
        choiceTexts[choiceIndex].transform.DOScale(2f, 0.2f).OnComplete(() =>
        {
            choiceTexts[choiceIndex].transform.DOScale(1f, 0.2f); // 放大后恢复原大小
        });
       // Debug.Log(index - 1);
       // Debug.Log(choiceIndex);

        // 你也可以为选项添加其他动画效果，例如颜色变化、透明度变化等
        //choiceTexts[choiceIndex].DOColor(Color.green, 0.2f);

        //choicePanel.SetActive(true); // 隐藏选项面板
        //choiceTexts[choiceIndex].color = Color.white;  // 直接恢复原色

        ShowNextDialogue(); // 继续显示对话
    }

    // 开始对话
    public void StartDialogue()
    {
        if (savedIndex > 0 && savedIndex < textList.Count) // 如果有保存的对话进度
        {
            index = savedIndex; // 恢复之前的进度
            isChoosing = savedIsChoosing; // 恢复之前的选择状态
        }

        textLabel.text = textList[index]; // 显示当前对话内容
        

        // 根据是否处于选择阶段显示选项面板
        if (isChoosing)
        {
            DisplayQuestion(textList[index]); // 显示当前的选择题
        }
        else if(!isChoosing)
        {
            index++;
        }
        // 其他UI状态控制
        promptUI.SetActive(false); // 隐藏交互提示UI
        promptUI1.SetActive(true); // 显示退出提示UI
        isInDialogue = true;

    }
    // 返回上一个问题
    // void GoBackToPreviousQuestion()
    //{
    // index=index-2; // 返回到上一个问题

    // gameObject.SetActive(true); // 隐藏对话框
    //  choicePanel.SetActive(true); // 隐藏选项面板
    //    
    // string previousText = textList[index];
    //textLabel.text = previousText;

    // 如果是在选择阶段，显示选项
    // isChoosing = true;
    // Debug.Log(previousText);
    // DisplayQuestion(previousText);

    //}
    void GoBackToPreviousQuestion()
    {
        index = index - 2; // 返回到上一个问题

        gameObject.SetActive(true); // 隐藏对话框
        choicePanel.SetActive(true); // 隐藏选项面板

        string previousText = textList[index];
        //textLabel.text = previousText;

        // 如果是在选择阶段，显示选项
        isChoosing = true;
        // Debug.Log(previousText);
        DisplayQuestion(previousText);

       
    }
   


    // 结束对话
    public void EndDialogue()
    {
        savedIndex = index-1; // 保存当前的对话进度
        savedIsChoosing = isChoosing; // 保存是否正在选择阶段

        isInDialogue = false; // 标记对话结束
        gameObject.SetActive(false); // 隐藏对话框
        promptUI.SetActive(true); // 显示交互提示UI
        promptUI1.SetActive(false); // 隐藏退出提示UI
        choicePanel.SetActive(false); // 隐藏选项面板
       
    }
    // 显示或隐藏与NPC互动的提示
    public void ShowInteractionPrompt(bool show)
    {
        promptUI.SetActive(show);
    }
}
