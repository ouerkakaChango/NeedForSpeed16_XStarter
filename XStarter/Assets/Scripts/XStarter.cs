using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using UnityEngine;
using SFB;

public class WinKeyboard
{

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    private static extern short GetKeyState(int keyCode);

    [DllImport("user32.dll")]
    private static extern int GetKeyboardState(byte[] lpKeyState);

    [DllImport("user32.dll", EntryPoint = "keybd_event")]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    private const byte VK_NUMLOCK = 0x90;

    private const uint KEYEVENTF_KEYDOWN = 0;
    private const uint KEYEVENTF_KEYHOLD = 1;
    private const uint KEYEVENTF_KEYUP = 2;

    public static bool GetNumLock()
    {
        return (((ushort)GetKeyState(VK_NUMLOCK)) & 0xffff) != 0;
    }

    public static void PressNumLk()
    {
        keybd_event(VK_NUMLOCK, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_NUMLOCK, 0, KEYEVENTF_KEYHOLD, 0);
        keybd_event(VK_NUMLOCK, 0, KEYEVENTF_KEYUP, 0);
    }

    //####################################################

    //public void SetNumLock(bool bState)
    //{
    //    if (GetNumLock() != bState)
    //    {
    //        keybd_event(VK_NUMLOCK, 0x45, KEYEVENTF_KEYHOLD | KEYEVENTF_KEYDOWN, 0);
    //        keybd_event(VK_NUMLOCK, 0x45, KEYEVENTF_KEYHOLD | KEYEVENTF_KEYUP, 0);
    //    }
    //}
}
[System.Serializable]
public struct XStarterInfo
{
    public string targetPath;
}
public class XStarter : MonoBehaviour
{
    XStarterInfo dataRoot;
    //public string targetPath = "E:\\XunLei\\3DMGAME_Need_for_Speed_The_Run.CHT.Green\\GamestWar_Need for Speed 16 The Run Limited Edition\\Need For Speed The Run.exe";
    public string stateStr = "正常";
    string textpath;
    void Start()
    {
        textpath = Application.dataPath + "\\Resources\\xstarter.json";
        if (WinKeyboard.GetNumLock())
        {
            UnityEngine.Debug.Log("小键盘原先开启");
            WinKeyboard.PressNumLk();
        }
        else
        {
            UnityEngine.Debug.Log("小键盘原先关闭");
        }
        var content = File.ReadAllText(textpath);
        UnityEngine.Debug.Log(content);
        dataRoot = JsonUtility.FromJson<XStarterInfo>(content);      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width * 0.0f, Screen.height * 0.5f, Screen.width * 1.0f, Screen.height * 0.3f),"当前目标路径：" + dataRoot.targetPath);
        GUI.Label(new Rect(Screen.width * 0.0f, Screen.height * 0.6f, Screen.width * 1.0f, Screen.height * 0.3f), "state：" + stateStr);
        if (GUI.Button(new Rect(Screen.width * 0.2f, Screen.height * 0.6f, Screen.width*0.1f, Screen.height * 0.1f), "路径"))
        {
            var path = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false)[0];
            //UnityEngine.Debug.Log(path);
            dataRoot.targetPath = path;
            string infostr = JsonUtility.ToJson(dataRoot);
            
            File.WriteAllText(textpath, infostr);
            UnityEngine.Debug.Log("保存成功");
        }
        if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.6f, Screen.width * 0.2f, Screen.height * 0.2f), "启动"))
        {
            CallExe();
        }
    }

    void CallExe()
    {
        try
        {
            Process myProcess = new Process();
            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            {
                //myProcess.StartInfo.FileName = Application.dataPath +"/CmdExe/AutoCS.exe";
                //myProcess.StartInfo.FileName = "C:\\Personal\\ParticleToy\\x64\\Debug\\ParticleToy.exe";
                //myProcess.StartInfo.FileName = "E:\\XunLei\\3DMGAME_Need_for_Speed_The_Run.CHT.Green\\GamestWar_Need for Speed 16 The Run Limited Edition\\Need For Speed The Run.exe";
                myProcess.StartInfo.FileName = dataRoot.targetPath;
            }
            myProcess.StartInfo.WorkingDirectory = Application.dataPath;
            //myProcess.StartInfo.Arguments = FullPath(outTaskFilePath);
            //Debug.Log(FullPath(outTaskFilePath));
            myProcess.EnableRaisingEvents = true;
            myProcess.Start();
            myProcess.WaitForExit();
            int ExitCode = myProcess.ExitCode;
            UnityEngine.Debug.Log("目标程序启动" + ExitCode);
        }
        catch (Exception e)
        {
            print(e);
        }
    }

    public string GetResult(string[] paths)
    {
        if (paths.Length == 0)
        {
            return "";
        }

        string re = "";
        foreach (var p in paths)
        {
            re += p + "\n";
        }
        return re;
    }
}
