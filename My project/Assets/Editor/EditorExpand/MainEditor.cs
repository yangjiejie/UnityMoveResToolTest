using System;
using System.IO;
using Launcher;
using Runtime.Core.Constants;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;

[CustomEditor(typeof(Main))]
public class MainEditor : Editor
{
    bool SymbleDefine = true;



    string[] defineArray = new string[]
    {
        "",
        "DEV_MODE", 
        "FRUIT_DEBUG",
        "DEV_DEBUG_NET", // 网络调试   
        "VIP_DEBUG", // 测试vip   
        "UNITASK_DOTWEEN_SUPPORT", // DoTween UniTask
        "HOT_UPDATE_TEST", // 测试热更新   
        "DELIVERY_DEV_1_1_3",// 分包开发 
        "DEBUG_REDPACK",
        "REMOTE_DEBUG", // 远程测试  
        "TEST_INPUT" , // 测试输入系统 
        "TEST_WELCOMEUI" , // 测试欢迎界面 
        "TEST_RANKMATCH" , // 测试排位赛
    };

    private void OnEnable()
    {
        int nBit = 0;
        if(defineArray.Length >= 32)
        {
            Debug.LogError("警告位已经不够用了");
           
        }
        for(int i = 0; i < defineArray.Length; i++)
        {
            if (string.IsNullOrEmpty(defineArray[i])) continue;
            if(EasyUseEditorFuns.HasDebugSymble(defineArray[i]))
            {
                nBit |=  GetBit(nBit, i);
            }
        }
        (target as Main).DefineBit = nBit;
        
        //c# read json 

    }

    static int GetBit(int number, int bitPosition)
    {
        // 检查位是否有效
        if (bitPosition < 0 || bitPosition >= sizeof(int) * 8)
        {
            throw new ArgumentOutOfRangeException(nameof(bitPosition), "位位置无效");
        }

        // 生成掩码：将 1 左移 bitPosition 位，然后取反
        int mask = (1 << bitPosition);

        // 使用按位与操作干掉指定位
        return number | mask;
    }
    static int ClearBit(int number, int bitPosition)
    {
        // 检查位是否有效
        if (bitPosition < 0 || bitPosition >= sizeof(int) * 8)
        {
            throw new ArgumentOutOfRangeException(nameof(bitPosition), "位位置无效");
        }

        // 生成掩码：将 1 左移 bitPosition 位，然后取反
        int mask = ~(1 << bitPosition);

        // 使用按位与操作干掉指定位
        return number & mask;
    }

    static bool HasBit(int number, int bitPosition)
    {
        // 检查位是否有效
        if (bitPosition < 0 || bitPosition >= sizeof(int) * 8)
        {
            throw new ArgumentOutOfRangeException(nameof(bitPosition), "位位置无效");
        }

        // 判断指定位是否为 1
        return (number & (1 << bitPosition)) != 0;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.BeginHorizontal();
        if(!Application.isPlaying &&  GUILayout.Button("启动", GUILayout.Height(50)) || 
            Application.isPlaying && GUILayout.Button("退出", GUILayout.Height(50)))
        {
            EditorBuildSettingsScene[] tempScenes = new UnityEditor.EditorBuildSettingsScene[1];
            tempScenes[0] = new EditorBuildSettingsScene("Assets/scenes/GameStart.unity", true);
            EditorBuildSettings.scenes = tempScenes;
            if (!Application.isPlaying)
            {
                EditorApplication.isPlaying = true;
            }
            else
            {
                EditorApplication.ExecuteMenuItem("Edit/Play");
            }

            
        }
        if(GUILayout.Button("暂停", GUILayout.Height(50)))
        {
            EditorApplication.ExecuteMenuItem("Edit/Pause");
        }
        if(GUILayout.Button("编译c#", GUILayout.Height(50)))
        {
            EditorUtility.RequestScriptReload();
        }
        if (GUILayout.Button("清理dlc缓存", GUILayout.Height(50)))
        {
            SafeDeleteUnityResHook.forbidHook = true;

            foreach(var dlcName in YooAssetsDefine.allDlc)
            {
                PlayerPrefs.DeleteKey($"{dlcName}_DownLoadProgress");
            }
            
            
            foreach (var item in YooAssetsDefine.allDlc)
            {
                EasyUseEditorFuns.DelFolderAllContens(Path.Combine(System.Environment.CurrentDirectory, $"BuildinFiles/{item}"), true);
                EasyUseEditorFuns.DelFolderAllContens(Application.streamingAssetsPath + $"/BuildinFiles/{item}", true);
            }
            SafeDeleteUnityResHook.forbidHook = false;
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("打开c#",GUILayout.Height(50)))
        {
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(System.Environment.CurrentDirectory + "/Assets/Launcher/Main.cs",1);
            //Application.OpenURL(System.Environment.CurrentDirectory +  "/Assets/Launcher/Main.cs");   
        }

        if (GUILayout.Button("移除monohook插件", GUILayout.Height(50)))
        {
            if(Directory.Exists(Application.dataPath + "/MonoHook"))
            {
                EasyUseEditorFuns.DelFolderAllContens(Application.dataPath + "/MonoHook", true);
                if(File.Exists(Application.dataPath + "/MonoHook.meta"))
                {
                    File.Delete(Application.dataPath + "/MonoHook.meta");
                }
            }
            AssetDatabase.Refresh();
        }

        
        GUILayout.EndHorizontal();

        Main global = target as Main;
        SymbleDefine = EditorGUILayout.Foldout(SymbleDefine, new GUIContent("宏定义"));
        if (SymbleDefine)
        {
            if(GUILayout.Button("关闭所有宏定义"))
            {

            }
            for(int i = 1; i < defineArray.Length; i++)
            {
                if (global && HasBit(global.DefineBit, i) && GUILayout.Button($"关闭宏{defineArray[i]}"))
                {
                    global.DefineBit = ClearBit(global.DefineBit, i);
                    EasyUseEditorFuns.SetUnitySymbleDefine(false, defineArray[i]);
                }
                if (global && !HasBit(global.DefineBit, i) && GUILayout.Button($"开启宏{defineArray[i]}"))
                {
                    global.DefineBit = GetBit(global.DefineBit, i);
                    EasyUseEditorFuns.SetUnitySymbleDefine(true, defineArray[i]);
                }
            }
            
            
        }
    }
}