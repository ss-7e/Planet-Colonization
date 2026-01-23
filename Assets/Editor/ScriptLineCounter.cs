// Assets/Editor/ScriptLineCounter.cs
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class ScriptLineCounter : EditorWindow
{
    private int totalLines = 0;
    private int totalScripts = 0;
    private Vector2 scrollPosition;

    [MenuItem("Tools/统计脚本代码行数")]
    public static void ShowWindow()
    {
        GetWindow<ScriptLineCounter>("脚本行数统计");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("开始统计", GUILayout.Height(30)))
        {
            CountAllScripts();
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField($"脚本文件数: {totalScripts}");
        EditorGUILayout.LabelField($"总代码行数: {totalLines}");

        GUILayout.Space(20);

        if (GUILayout.Button("导出统计报告", GUILayout.Height(30)))
        {
            ExportReport();
        }
    }

    private void CountAllScripts()
    {
        totalLines = 0;
        totalScripts = 0;

        // 获取所有C#脚本
        string[] scriptFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        foreach (string file in scriptFiles)
        {
            // 排除编辑器脚本（可选）
            if (file.Contains("\\Editor\\") || file.Contains("/Editor/"))
                continue;

            int lines = CountLinesInFile(file);
            totalLines += lines;
            totalScripts++;

            // 在控制台显示每个文件的信息（可选）
            Debug.Log($"文件: {Path.GetFileName(file)} - 行数: {lines}");
        }

        Debug.Log($"统计完成！\n脚本总数: {totalScripts}\n总行数: {totalLines}");
    }

    private int CountLinesInFile(string filePath)
    {
        int lineCount = 0;
        bool inBlockComment = false;

        try
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // 跳过空行
                if (string.IsNullOrEmpty(trimmedLine))
                    continue;

                // 处理块注释
                if (inBlockComment)
                {
                    if (trimmedLine.Contains("*/"))
                    {
                        inBlockComment = false;
                    }
                    continue;
                }

                // 检查块注释开始
                if (trimmedLine.StartsWith("/*"))
                {
                    inBlockComment = true;
                    continue;
                }

                // 跳过单行注释
                if (trimmedLine.StartsWith("//") || trimmedLine.StartsWith("#"))
                    continue;

                lineCount++;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"读取文件 {filePath} 时出错: {e.Message}");
        }

        return lineCount;
    }

    private void ExportReport()
    {
        string reportPath = EditorUtility.SaveFilePanel("保存统计报告", "", "ScriptLineReport.txt", "txt");

        if (!string.IsNullOrEmpty(reportPath))
        {
            string report = $"脚本代码行数统计报告\n";
            report += $"生成时间: {System.DateTime.Now}\n";
            report += $"脚本文件总数: {totalScripts}\n";
            report += $"总代码行数: {totalLines}\n";
            report += $"平均每个脚本行数: {(totalScripts > 0 ? totalLines / totalScripts : 0)}\n";

            File.WriteAllText(reportPath, report);
            EditorUtility.RevealInFinder(reportPath);
        }
    }
}