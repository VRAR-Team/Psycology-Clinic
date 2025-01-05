using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.IO;


public class Dialogsystem1 : MonoBehaviour

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
    // 因子的项目ID对应关系
    private Dictionary<string, List<int>> factorItems = new Dictionary<string, List<int>>()
    {
        { "躯体化", new List<int> {1, 4, 12, 27, 40, 42, 48, 49, 52, 53, 56, 58} },
        { "强迫症状", new List<int> {3, 9, 10, 28, 38, 45, 46, 51, 55, 65} },
        { "人际关系敏感", new List<int> {6, 21, 34, 36, 37, 41, 61, 69, 73} },
        { "抑郁", new List<int> {5, 14, 15, 20, 22, 26, 29, 30, 31, 32, 54, 71, 79} },
        { "焦虑", new List<int> {2, 17, 23, 33, 39, 57, 72, 78, 80, 86} },
        { "敌对", new List<int> {11, 24, 63, 67, 74, 81} },
        { "恐怖", new List<int> {13, 25, 47, 50, 70, 75, 82} },
        { "偏执", new List<int> {8, 18, 43, 68, 76, 83} },
        { "精神病性", new List<int> {7, 16, 35, 62, 77, 84, 85, 87, 88, 90} },
        { "其他", new List<int> {19, 44, 59, 60, 64, 66, 89} }
    };
    // 定义因子参考范围
    // 定义因子参考范围（根据平均值 ± 标准差计算）
    private Dictionary<string, (float min, float max)> factorRanges = new Dictionary<string, (float, float)>
{
    { "躯体化", (0.89f, 1.85f) },
    { "强迫症状", (1.10f, 2.14f) },
    { "人际关系敏感", (1.04f, 2.26f) },
    { "抑郁", (0.91f, 2.09f) },
    { "焦虑", (0.96f, 1.82f) },
    { "敌对", (0.91f, 2.01f) },
    { "恐怖", (0.82f, 1.64f) },
    { "偏执", (0.86f, 2.00f) },
    { "精神病性", (0.87f, 1.71f) }
};



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
    public void SaveChoicesToFile(string fileName)
    {

        string path = Path.Combine(Application.dataPath, "Questionnaire", fileName);
        // 创建一个新的文件，如果文件存在则覆盖
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            // 遍历所有选择结果并写入文件
            foreach (var entry in choicesResults)
            {

                // 写入每个问题的索引和玩家的选择
                writer.WriteLine($"QuestionIndex: {entry.Key}, ChoiceIndex: {entry.Value}");
            }
        }

        // 提示用户保存成功
        Debug.Log($"选择结果已保存到: {path}");
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
        else if (index == 91)
        {
            choicePanel.SetActive(false); // 隐藏选项面板
            SaveChoicesToFile("choicesResults.txt");
            // 对话结束，隐藏对话框
            CalculateAndSaveFactorAverageScores("ScoreResults.txt");
            index++;
        }
        else
        {
            Debug.Log(index);
            EndDialogue();
            promptUI.SetActive(false); // 显示交互提示UI

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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

    }
    // 显示或隐藏与NPC互动的提示
    public void ShowInteractionPrompt(bool show)
    {
        promptUI.SetActive(show);
    }



    public void CalculateAndSaveFactorAverageScores(string filePath)
    {
        // 创建一个用于保存每个因子得分和项目的字符串
        string result = "通过此问卷,你的初步评判结果为:\n";

        foreach (var factor in factorItems)
        {
            // 计算当前因子的平均分
            float averageScore = CalculateFactorAverageScore(factor.Key);

            // 获取该因子的参考范围
            if (factorRanges.ContainsKey(factor.Key))
            {
                var range = factorRanges[factor.Key];
                Debug.Log(range);
                Debug.Log(averageScore);
                Debug.Log(range.min);

                // 判断该因子的平均分是否在参考范围内
                if (averageScore >= range.min && averageScore <= range.max)
                {
                    // 将该因子及其类型添加到 result
                    result += $"{factor.Key}\n";
                }
            }
            
            Debug.Log(result);
           
        }
        if (result == "通过此问卷,你的初步评判结果为:\n")
        {
            textLabel.text = "通过此问卷,你的初步评判结果为:\n所有项目正常\n";
        }
        else
        {
            // 显示结果到 textLabel
            textLabel.text = result;
        }

 

        // 保存结果到指定路径的txt文件
        SaveToFile(result, filePath);
    }

    // 根据因子的项目ID计算该因子的平均分
    private float CalculateFactorAverageScore(string factorName)
    {
        List<int> itemIDs = factorItems[factorName];
        float sum = 0;
        int itemCount = itemIDs.Count;

        // 计算所有项目得分的总和
        foreach (int itemID in itemIDs)
        {
            // choicesResults 存储的是每个问题的选择（从0开始的索引），所以需要转换为1-5分
            if (choicesResults.ContainsKey(itemID - 1))  // itemID - 1 用于从0开始的索引
            {
                int choiceIndex = choicesResults[itemID - 1]; // 获取玩家选择的索引
                sum += GetScoreFromChoiceIndex(choiceIndex);  // 获取选择的分数并加到总分
            }
        }

        // 计算并返回平均分
        return sum / itemCount;
    }

    // 将选择的索引（0-4）映射到对应的分数（1-5）
    private int GetScoreFromChoiceIndex(int choiceIndex)
    {
        switch (choiceIndex)
        {
            case 0: return 1;  // 从无
            case 1: return 2;  // 很轻
            case 2: return 3;  // 中等
            case 3: return 4;  // 偏重
            case 4: return 5;  // 严重
            default: return 0; // 默认返回0，表示错误
        }
    }
    // 将结果保存到文件
    private void SaveToFile(string content, string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, "Questionnaire", fileName);
        try
        {
            // 创建一个 StreamWriter 用于写入文件
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(content);
                Debug.Log("文件保存成功！");
            }
        }
        catch (IOException ex)
        {
            Debug.LogError($"文件保存失败: {ex.Message}");
        }
    }
}
