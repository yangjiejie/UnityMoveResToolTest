
namespace TextureTool
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;


    internal class TextureViewerWindow : EditorWindow
    {
       
        private static readonly GUILayoutOption[] TreeViewLayoutOptions = new GUILayoutOption[]
        {
            GUILayout.ExpandHeight(true)
        };

        [SerializeField] private TextureTreeViewState treeViewState = null; 
        [SerializeField] private TextureColumnHeaderState headerState = null; 
        [SerializeField] private SearchState[] columnSearchStates = new SearchState[0]  ; 
        [System.NonSerialized] private Texture2D[] textures = new Texture2D[0]; 
        [System.NonSerialized] private TextureImporter[] textureImporters = new TextureImporter[0];
        private TextureTreeView treeView = null; // TreeView 
        private SearchField searchField = null; 

        [SerializeField] MultiColumnHeaderState columnHeaderState;
        private bool isLoadingTexture = false;
        private bool isCreatingTreeView = false;

     
        [MenuItem("Tools/Texture Viewer")]
        private static void OpenWindow()
        {
            var window = GetWindow<TextureViewerWindow>();
            window.titleContent = ToolConfig.WindowTitle;

            var position = window.position;
            position.width = ToolConfig.InitialHeaderTotalWidth + 50f;
            position.height = 400f;
            window.position = position;

            window.CreateTreeView(); 
        }

     
        private void OnGUI()
        {
            MyStyle.CreateGUIStyleIfNull();
            if (treeView == null)
            {
                CreateTreeView();
            }

            DrawHeader();
            DrawTreeView();

        }

     
        private void DrawTreeView()
        {
            var rect = EditorGUILayout.GetControlRect(false, GUILayout.ExpandHeight(true));

            if (isCreatingTreeView)
            {
                // 显示"Creating..." 文本  
                EditorGUI.BeginDisabledGroup(true);
                treeView?.OnGUI(rect);
                EditorGUI.EndDisabledGroup();

                rect.position += MyStyle.LoadingLabelPosition;
                EditorGUI.LabelField(rect, ToolConfig.CreatingMessage, MyStyle.LoadingLabel);
            }
            else
            if (isLoadingTexture)// 显示图片view 
            {
                // 方法用于开始一个禁用组，在这个组内的所有 GUI 控件（如按钮、文本框等）都会被禁用 
                EditorGUI.BeginDisabledGroup(true);
                treeView?.OnGUI(rect);
                EditorGUI.EndDisabledGroup();

                rect.position += MyStyle.LoadingLabelPosition;
                EditorGUI.LabelField(rect, ToolConfig.LoadingMessage, MyStyle.LoadingLabel);
            }
            else
            {
                treeView?.OnGUI(rect);
            }
        }

      
        private void DrawHeader()
        {
            var defaultColor = GUI.backgroundColor;

            // 検索窓を描画
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUI.backgroundColor = Color.green;
                DrawReloadButton();
                GUI.backgroundColor = defaultColor;

                GUILayout.Space(100);

                GUILayout.FlexibleSpace();
            }

            GUI.backgroundColor = defaultColor;
        }

     
        private void DrawReloadButton()
        {
            if (treeView == null) { return; }

            if (GUILayout.Button("Reload", EditorStyles.toolbarButton))
            {
                CreateTreeView();
                //加载所有贴图 
                ReloadTexture();

                headerState.ResetSearch();
                //设置一行的数据  
                treeView.SetTexture(textures, textureImporters);
                treeView.Reload(); // Reloadを呼ぶとBuildRootが実行され、次にBuildRowsが実行されます。


                EditorApplication.delayCall += () => treeView.searchString = TextureTreeView.defaultSearchString;
            }
        }

    
        private void CreateTreeView()
        {
            if (treeView != null) { return; }
            if (isCreatingTreeView) { return; }
            isCreatingTreeView = true;
            Repaint();

            EditorApplication.delayCall += () =>
            {
                if (columnSearchStates == null || columnSearchStates.Length != ToolConfig.HeaderColumnNum)
                {
                    columnSearchStates = new SearchState[ToolConfig.HeaderColumnNum];
                    for (int i = 0; i < ToolConfig.HeaderColumnNum; i++)
                    {
                        columnSearchStates[i] = new SearchState();
                    }
                }


                treeViewState = treeViewState ?? new TextureTreeViewState();
                headerState = headerState ?? new TextureColumnHeaderState(ToolConfig.HeaderColumns, columnSearchStates);
                headerState.ResetSearch();

                // TreeView作成
                treeView = treeView ?? new TextureTreeView(treeViewState, headerState);
                treeView.searchString = TextureTreeView.defaultSearchString;
                treeView.Reload(); // Reloadを呼ぶとBuildRootが実行され、次にBuildRowsが実行されます。

                // SearchFieldを初期化
                searchField = new SearchField();
                searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;

                isCreatingTreeView = false;
            };
        }

    
        public static IEnumerable<string> GetAssetPaths(string[] directories, string filter = "")
        {
            for (int i = 0; i < directories.Length; i++)
            {
                var directory = directories[i];
                if (directory[directory.Length - 1] == '/')
                {
                    directory = directory.Substring(0, directory.Length - 1);
                }
            }

            var paths = AssetDatabase.FindAssets(filter, directories)
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Where(x => !string.IsNullOrEmpty(x))
                .OrderBy(x => x);

            return paths;
        }


    
        private void OnFocus()
        {
            if (treeView != null)
            {
                treeView.UpdateDataSize();
            }
        }

 
        public void ReloadTexture()
        {
            if (isLoadingTexture) { return; }

            isLoadingTexture = true;
            CustomUI.DisplayProgressLoadTexture();

           
            var paths = GetAssetPaths(ToolConfig.TargetDirectories, "t:texture2d");
            var textureList = new List<Texture2D>();
            var importerList = new List<TextureImporter>();
            foreach (var path in paths)
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture == null) continue;
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                textureList.Add(texture);
                importerList.Add(importer);
            }
            textures = textureList.ToArray();
            textureImporters = importerList.ToArray();

            EditorUtility.ClearProgressBar();

            isLoadingTexture = false;
        }
    }
}