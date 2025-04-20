using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

namespace Assets.Editor.EditorExpand
{
    public class DragableTextLable 
    {
        public string folderShowField;
        public bool UseMultiyContent { get; set; } = false;
        private string title;
        private void OnGUI()
        {
            DrawDragTextField();
        }

        public void Init(string title)
        {
            this.title = title;
        }
     

        private void DrawDragTextField()
        {

            folderShowField = string.Join(";", targetDirectories);

            folderShowField  = folderShowField.Replace("Assets/", "");

            folderShowField = EditorGUILayout.TextField(title, folderShowField);

            // 拖拽区域提示
            var dropRect = GUILayoutUtility.GetLastRect();
            GUI.Box(dropRect, GUIContent.none);
            HandleDragAndDrop(dropRect);

        }
        List<string> targetDirectories;
        private void HandleDragAndDrop(Rect dropArea)
        {
            Event evt = Event.current;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        if (DragAndDrop.paths.Length > 0)
                        {
                            foreach (string path in DragAndDrop.paths)
                            {
                               
                                if (Directory.Exists(path) && !targetDirectories.Contains(path))
                                {
                                    targetDirectories.Add(path);
                                }
                                else if (File.Exists(path))
                                {
                                    string dir = Path.GetDirectoryName(path);
                                    if (!targetDirectories.Contains(dir))
                                    {
                                        targetDirectories.Add(dir);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}