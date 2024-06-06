using UnityEngine;
using System.IO;
using System.Diagnostics;

//for capturing screenshots in play and edit mode
[ExecuteInEditMode]
public class QuickScreenCapture : MonoBehaviour
{
    public bool quickCapture = false;
    public bool autoCapture = false;
    public bool openOnCapture = false;
    public bool debugOpenCapureFolder = false;
    public int autoCaptureInterval = 10;
    public int readOnlyAutoCaptureCounter;
    public string folder = "media";
    public string fileName = "Screen shot";
    public bool autoIndex = true;
    public int shotIndex = 0;
    public string extention = ".png";

    Camera m_camera;
    string m_currentFile = "";
    bool m_shotTaken = false;
    KeyCode m_captureKey = KeyCode.C;

    internal void Capture()
    {
        quickCapture = true;
    }

    private void ResetState()
    {
        m_shotTaken = false;

        m_currentFile = "";
    }

    public void CaptureQuick()
    {
        quickCapture = false;

        string _path = Application.dataPath + "/" + folder + "/" + string.Concat(fileName,"_",shotIndex.ToString(),extention);

        ScreenCapture.CaptureScreenshot(_path);

        m_currentFile = _path;

        if (autoIndex)
            shotIndex++;

        m_shotTaken = true;
    }

    public void RunLogic()
    {
        if (quickCapture && !m_shotTaken)
        {
            CaptureQuick();
            quickCapture = false;
        }

        if (openOnCapture && File.Exists(m_currentFile))
            Process.Start(m_currentFile);

        ResetState();
    }
    
    void Update()
    {
        if (Application.isPlaying)
        {
            if (Input.GetKeyUp(m_captureKey))
                quickCapture = true;
            
            if (autoCapture)
            {
                readOnlyAutoCaptureCounter++;

                if (readOnlyAutoCaptureCounter < autoCaptureInterval)
                    quickCapture = false;
                else
                {
                    readOnlyAutoCaptureCounter = 0;
                    quickCapture = true;
                }
            }
            RunLogic();
        }

        if(debugOpenCapureFolder)
        {
            string _path = Application.dataPath + "/" + folder;

            Process.Start(_path);

            debugOpenCapureFolder = false;
        }
    }
}