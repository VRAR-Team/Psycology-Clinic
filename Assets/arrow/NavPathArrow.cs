using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 导航箭头
/// </summary>
public class NavPathArrow : MonoBehaviour
{
    public MeshRenderer meshRenderer;//箭头3D对象Quad
    public List<Transform> points = new List<Transform>();//路径点
    private List<MeshRenderer> lines = new List<MeshRenderer>();//显示的路径

    public float xscale = 1f;//缩放比例
    public float yscale = 1f;

    void Start()
    {
        //箭头宽度缩放值
        xscale = meshRenderer.transform.localScale.x;
        //箭头长度缩放值
        yscale = meshRenderer.transform.localScale.y;
    }

    //画路径
    public void DrawPath()
    {
        if (points == null || points.Count <= 1)
            return;
        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawLine(points[i].position, points[i + 1].position, i);
        }
    }

    //画路径 参数为路径点数组
    public void DrawPath(Vector3[] points)
    {
        if (points == null || points.Length <= 1)
            return;
        for (int i = 0; i < points.Length - 1; i++)
        {
            DrawLine(points[i], points[i + 1], i);
        }
    }

    //隐藏路径
    public void HidePath()
    {
        for (int i = 0; i < lines.Count; i++)
            lines[i].gameObject.SetActive(false);
    }

    //画路径
    private void DrawLine(Vector3 start, Vector3 end, int index)
    {
       // Debug.Log(transform.gameObject.name);
        MeshRenderer mr;
        if (index >= lines.Count)
        {
            mr = Instantiate(meshRenderer);
            lines.Add(mr);
        }
        else
        {
            mr = lines[index];
        }

        var tran = mr.transform;
        var length = Vector3.Distance(start, end);
        tran.localScale = new Vector3(xscale, length, 1);
        tran.position = (start + end) / 2;
        //指向end
        tran.LookAt(end);
        //旋转偏移
        tran.Rotate(0, 90, 90);
        mr.material.mainTextureScale = new Vector2(1, length * yscale);
        mr.gameObject.SetActive(true);
    }

    void OnGUI()
    {
       DrawPath();
    }
}