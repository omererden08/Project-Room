using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/*
SubtitleManager subtitleManager = FindObjectOfType<SubtitleManager>(); veya event sistemle
EvntManager.triggerEvent("subID", 1);
subtitleManager.DisplaySubtitleById(1) veya (1,süre); // ID 1 olan altyaziyi gösterir
subtitleManager.ClearSubtitle(); // Altyazıyı temizler
*/
[System.Serializable]
public class Subtitle
{
    public string id;
    public string text;
    public string speaker;
}

[System.Serializable]
public class SubtitleData
{
    public List<Subtitle> subtitles;
}

public class SubtitleManager : MonoBehaviour
{
    
    private TextMeshProUGUI subtitleText;
    private List<Subtitle> subtitles = new List<Subtitle>();
    public float defaultDuration = 3f;

    void Start()
    {
        EvntManager.StartListening<string>("subID", DisplaySubtitleById);

        subtitleText = GameObject.FindGameObjectWithTag("subt").GetComponent<TextMeshProUGUI>();

        LoadSubtitles();
    }


    void LoadSubtitles()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("subtitles");
        if (jsonFile == null)
        {
            Debug.LogError("JSON dosyası bulunamadı!");
            return;
        }
        try
        {
            SubtitleData data = JsonUtility.FromJson<SubtitleData>(jsonFile.text);
            if (data != null && data.subtitles != null)
            {
                subtitles = data.subtitles;
            }
            else
            {
                Debug.LogError("JSON parse hatası: Veri boş!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON parse hatası: {e.Message}");
        }
    }
    public void DisplaySubtitleById(string id)
    {
        Subtitle subtitle = subtitles.FirstOrDefault(s => s.id == id);
        if (subtitle != null)
        {
            if (subtitleText != null)
            {
                //subtitleText.text = $"[{subtitle.speaker}]: {subtitle.text}";
                subtitleText.text = $"{subtitle.text}";
            }
            Debug.Log($"[{subtitle.speaker}]: {subtitle.text}");
            Invoke("ClearSubtitle", defaultDuration);
        }
        else
        {
            Debug.LogWarning($"ID {id} ile altyazı bulunamadı!");
            ClearSubtitle();
        }
    }
    public void DisplaySubtitleById(string id, int duration)
    {
        Subtitle subtitle = subtitles.FirstOrDefault(s => s.id == id);
        if (subtitle != null)
        {
            if (subtitleText != null)
            {
                subtitleText.text = $"[{subtitle.speaker}]: {subtitle.text}";
            }
            Debug.Log($"[{subtitle.speaker}]: {subtitle.text}");
            Invoke("ClearSubtitle", duration);
        }
        else
        {
            Debug.LogWarning($"ID {id} ile altyazı bulunamadı!");
            ClearSubtitle();
        }
    }


    public void ClearSubtitle()
    {
        if (subtitleText != null)
        {
            subtitleText.text = "";
        }
        Debug.Log("Altyazı temizlendi.");
    }
}