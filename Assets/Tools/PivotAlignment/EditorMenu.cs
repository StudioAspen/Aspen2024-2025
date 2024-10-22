using UnityEngine;
using UnityEditor;

public class EditorMenu
{
    [MenuItem("Dreamscape/PivotAlignment")]
    public static void InitPivotAlignmentTool()
    {
        PivotAlignment_window.InitWindow();
    }
}
