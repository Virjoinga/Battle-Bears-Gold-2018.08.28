using System.Collections;
using UnityEngine;

public class DownloadObb : MonoBehaviour
{
    private string _mainPath;

    private string _expPath;

    private void Start()
    {
#if UNITY_ANDROID
        if (!GooglePlayDownloader.RunningOnAndroid())
        {
            Debug.Log("Not running on Android, loading Bootloader.");
            Application.LoadLevel("Bootloader");
            return;
        }

        _expPath = GooglePlayDownloader.GetExpansionFilePath();
        if (_expPath == null)
        {
            Debug.Log("External storage is not available!");
            Application.LoadLevel("Bootloader");
            return;
        }

        _mainPath = GooglePlayDownloader.GetMainOBBPath(_expPath);
        if (_mainPath == null)
        {
            GooglePlayDownloader.FetchOBB();
        }

        StartCoroutine(CoroutineLoadLevel());
#else


        Application.LoadLevel("Bootloader");
#endif
    }

    protected IEnumerator CoroutineLoadLevel()
    {
        while (string.IsNullOrEmpty(_mainPath))
        {
            _mainPath = GooglePlayDownloader.GetMainOBBPath(_expPath);
            yield return new WaitForSeconds(2f);
        }
        Debug.Log("Main Path is: " + _mainPath);
        WWW loading = WWW.LoadFromCacheOrDownload("file://" + _mainPath, 0);
        yield return loading;
        if (loading.error != null)
        {
            Debug.LogError("WWW Error: " + loading.error);
        }
        else
        {
            Application.LoadLevel("Bootloader");
        }
    }
}
