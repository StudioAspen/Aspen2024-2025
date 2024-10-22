using UnityEngine;
using UnityEditor;

public class PivotAlignment_window : EditorWindow
{
    private static PivotAlignment_window win;
    private GameObject gameObject;
    private MeshRenderer renderer;
    private string[] options = new string[] { "Original", "Center", "Min", "Max" };
    private int selectedIndexX = 0;
    private int selectedIndexY = 0;
    private int selectedIndexZ = 0;
    private float xPos;
    private float yPos;
    private float zPos;
    private float xOffset;
    private float yOffset;
    private float zOffset;

    public static void InitWindow()
    {
        win = EditorWindow.GetWindow<PivotAlignment_window>("Pivot Alignment");
        win.Show();
        // Subscribe to selection changed event
        Selection.selectionChanged += win.OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        Repaint();
    }

    private void OnGUI()
    {
        // Instantiate GameObject and Renderer and print errors
        gameObject = Selection.activeGameObject;
        if (gameObject == null)
        {
            EditorGUILayout.LabelField("No GameObject is selected");
            return;
        }
        if (gameObject.transform.parent == null)
        {
            EditorGUILayout.LabelField("Selected GameObject has no parent");
            return;
        }
        renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            EditorGUILayout.LabelField("Selected GameObject has no mesh renderer");
            return;
        }

        // DropDown Menus
        EditorGUILayout.LabelField("Alignment Options", EditorStyles.boldLabel);
        selectedIndexX = EditorGUILayout.Popup(new GUIContent("X", "Choose the alignment for X"), selectedIndexX, options);
        xOffset = EditorGUILayout.FloatField("X Offset:", xOffset);
        selectedIndexY = EditorGUILayout.Popup(new GUIContent("Y", "Choose the alignment for Y"), selectedIndexY, options);
        yOffset = EditorGUILayout.FloatField("Y Offset:", yOffset);
        selectedIndexZ = EditorGUILayout.Popup(new GUIContent("Z", "Choose the alignment for Z"), selectedIndexZ, options);
        zOffset = EditorGUILayout.FloatField("Z Offset:", zOffset);

        // Move GameObject Button
        if (GUILayout.Button("Apply Alignment", GUILayout.Height(35), GUILayout.ExpandWidth(true)))
        {
            gameObject.transform.position = gameObject.transform.parent.position;
            xPos = DropDownSelection(selectedIndexX).x;
            yPos = DropDownSelection(selectedIndexY).y;
            zPos = DropDownSelection(selectedIndexZ).z;
            gameObject.transform.position = new Vector3(xPos + xOffset, yPos + yOffset, zPos + zOffset);
        }
    }

    private Vector3 DropDownSelection(int selectedIndex)
    {
        if (selectedIndex == 1)
        {
            return 2*gameObject.transform.parent.position - renderer.bounds.center;
        }
        if (selectedIndex == 2)
        {
            return 2*gameObject.transform.parent.position - renderer.bounds.center + renderer.bounds.size/2;
        }
        if (selectedIndex == 3)
        {
            return 2*gameObject.transform.parent.position - renderer.bounds.center - renderer.bounds.size/2;
        }
        return gameObject.transform.parent.position;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the selection changed event when the window is closed
        Selection.selectionChanged -= OnSelectionChanged;
    }
}