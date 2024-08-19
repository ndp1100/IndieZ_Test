using UnityEngine;

public class ScreenshotMaker : MonoBehaviour
{
    private const string _path = "/Screenshot/Screen";
    private const string _extension = ".png";

    private string _date;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            MakeScreenshot();
    }

    void MakeScreenshot()
    {
        _date = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        ScreenCapture.CaptureScreenshot(Application.dataPath + _path + _date + _extension);
    }
}
