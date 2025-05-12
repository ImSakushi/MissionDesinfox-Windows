using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DataDownloader : MonoBehaviour
{
    public string linkReplace = "gviz/tq?tqx=out:csv&sheet=";
    public string[] sheetNames;
    public string sheetName;
    public int sheetIndex;
    public int row;
    public int col;
    public string sheetToLoad;

    // Dictionnaire où chaque CSV est stocké sous la clé "sheetName"
    public static Dictionary<string, string> datas = new Dictionary<string, string>();

    /// <summary>
    /// Sous-dossier dans Assets/Resources où l’on lit/écrit les CSV.
    /// • En play → Resources.Load($"Data/{sheet}")
    /// • En éditeur → Assets/Resources/Data/{sheet}.csv
    /// </summary>
    [SerializeField]
    public string path = "Data";

    public string url;
    public int lineAmount = 0;

    public delegate void OnFinishedLoading();
    public OnFinishedLoading onDownloadFinish;

    #region Parse CSV
    public virtual void Load()
    {
        Debug.Log($"[DataDownloader] Load() appelé pour path='Resources/{path}' avec {sheetNames.Length} sheet(s), sheetToLoad='{sheetToLoad}'");
        for (int i = 0; i < sheetNames.Length; ++i)
        {
            var sheet = sheetNames[i];
            sheetIndex = i;
            Debug.Log($"[DataDownloader] → itération i={i}, sheet='{sheet}'");

            if (!string.IsNullOrEmpty(sheetToLoad) && sheet != sheetToLoad) {
                Debug.Log($"[DataDownloader]    skip sheet '{sheet}' (on ne charge que '{sheetToLoad}')");
                continue;
            }


            string text;
            if (datas.ContainsKey(sheet))
            {
                text = datas[sheet];
            }
            else
            {
                // Resources.Load ne prend pas l'extension
                var resourcePath = $"{path}/{Path.GetFileNameWithoutExtension(sheet)}";
                var textAsset = Resources.Load<TextAsset>(resourcePath);
                if (textAsset == null)
                {
                    Debug.LogError($"CSV local non trouvé : Resources/{resourcePath}.csv");
                    continue;
                }
                text = textAsset.text;
            }

            sheetName = sheet;
            lineAmount = fgCSVReader.GetLineAmount(text);
            fgCSVReader.LoadFromString(text, new fgCSVReader.ReadLineDelegate(GetCell));
        }
        Debug.Log("[DataDownloader] Fin de Load() → appel FinishLoading()");

        FinishLoading();
        
    }

    public virtual void FinishLoading()
    {
        // Surchargeable
    }

    public virtual void GetCell(int rowIndex, List<string> cells)
    {
        row = rowIndex;
        // Surchargeable
    }
    #endregion

    #region Download depuis Google Sheets
    /// <summary>
    /// Lance le téléchargement de tous les CSV.
    /// </summary>
    public void DownloadCSVs()
    {
        _ = StartCoroutine(DownloadsCSVs());
    }

    /// <summary>
    /// Coroutine interne pour télécharger tous les CSV.
    /// </summary>
    private IEnumerator DownloadsCSVs()
    {
        // Petit yield pour précharger (inutile en build)
        yield return null;

        for (var i = 0; i < sheetNames.Length; i++)
        {
            sheetIndex = i;
            var editIndex = url.IndexOf("edit");
            if (editIndex != -1)
            {
                var tmpUrl = url.Remove(editIndex) + linkReplace + sheetNames[sheetIndex];
                yield return DownloadCSV(tmpUrl, sheetNames[sheetIndex]);
            }
            else
            {
                Debug.LogError("No index for edit in link: " + url);
            }
        }

#if UNITY_EDITOR
        AssetDatabase.Refresh();
        Debug.Log($"Finished Downloading & Importing all sheets");
#endif

        // Log de tout le contenu pour debug
        string allCsvLog = "";
        foreach (var kvp in datas)
        {
            allCsvLog += $"=== Contenu du CSV \"{kvp.Key}\" ===\n";
            allCsvLog += kvp.Value + "\n\n";
        }
        Debug.Log(allCsvLog);

        onDownloadFinish?.Invoke();
    }

    /// <summary>
    /// Coroutine pour télécharger un seul CSV (utilisée par l’éditeur).
    /// </summary>
    public IEnumerator DownloadsCSV(int sheetIndex)
    {
        var editIndex = url.IndexOf("edit");
        if (editIndex != -1)
        {
            var tmpUrl = url.Remove(editIndex) + linkReplace + sheetNames[sheetIndex];
            yield return DownloadCSV(tmpUrl, sheetNames[sheetIndex]);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
            Debug.Log($"Importing {sheetNames[sheetIndex]}");
#endif
        }
        else
        {
            Debug.LogError("No index for edit in link: " + url);
        }
    }

    /// <summary>
    /// Coroutine interne pour fetch un CSV depuis tmpUrl.
    /// </summary>
    private IEnumerator DownloadCSV(string tmpUrl, string sheetName)
    {
        _ = Time.realtimeSinceStartup + 10f;
        var www = UnityWebRequest.Get(tmpUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error requesting CSV (code: {www.responseCode})");
            Debug.LogError(www.error);
        }
        else
        {
            if (Application.isPlaying)
            {
                // On garde en mémoire pour le runtime
                if (datas.ContainsKey(sheetName))
                    datas[sheetName] = www.downloadHandler.text;
                else
                    datas.Add(sheetName, www.downloadHandler.text);
            }
            else
            {
                // En mode éditeur, on écrit dans Assets/Resources/Data/
                var folder = Path.Combine(Application.dataPath, "Resources", path);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var filepath = Path.Combine(folder, $"{sheetName}.csv");
                File.WriteAllText(filepath, www.downloadHandler.text);
            }
        }
    }
    #endregion

    #region Helpers
    private static string GetCollumnName(int columnNumber)
    {
        var columnName = "";
        while (columnNumber > 0)
        {
            var rem = columnNumber % 26;
            if (rem == 0)
            {
                columnName += "Z";
                columnNumber = (columnNumber / 26) - 1;
            }
            else
            {
                columnName += (char)(rem - 1 + 'A');
                columnNumber /= 26;
            }
        }
        return Reverse(columnName);
    }

    public static string Reverse(string s)
    {
        var charArray = s.ToCharArray();
        System.Array.Reverse(charArray);
        return new string(charArray);
    }

    public string GetCellName(int row, int cell)
    {
        return GetCollumnName(cell) + (row + 1);
    }
    #endregion
}
