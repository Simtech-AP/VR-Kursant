using UnityEngine;
using System.Xml;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Net;

/// <summary>
/// Enumerative list of available languages
/// </summary>
public enum Language
{
    Polski,
    English,
    Deutsch
}

/// <summary>
/// Structure to hold languge texts and audio path
/// </summary>
[System.Serializable]
public struct LangugeElement
{
    public string id;
    public string title;
    public string text;
    public string audioClipPath;
    public string mediaPath;
}

/// <summary>
/// Controller in charge of languge management
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class LanguageController : Controller
{
    /// <summary>
    /// Flags used to tell if audio and text is loaded
    /// </summary>
    public bool textLoaded = false, audioLoaded = false, imageLoaded = false;
    /// <summary>
    /// Default language
    /// </summary>
    [SerializeField]
    private Language defaultLanguage = Language.Polski;
    /// <summary>
    /// Currently loaded language
    /// </summary>
    private Language currentLanguage = default;
    /// <summary>
    /// Path containing language files
    /// </summary>
    private string path = default;
    /// <summary>
    /// Loaded XML text file
    /// </summary>
    private XmlDocument xml = default;
    /// <summary>
    /// Audio source used in playing audio files
    /// </summary>
    [SerializeField]
    private AudioSource lectorAudio = default;
    /// <summary>
    /// List of all languge texts and its audio files
    /// </summary>
    public List<LangugeElement> languageElements = new List<LangugeElement>();
    /// <summary>
    /// Folder containing audio, image and movie files
    /// </summary>
    [SerializeField]
    private string mediaDirectory = "MediaFiles";
    /// <summary>
    /// List of loaded sprites to show in tutorials
    /// </summary>
    public List<Sprite> Sprites { get; private set; } = default;
    /// <summary>
    /// URL link to video to show in tutorials
    /// </summary>
    public string Url { get; private set; } = default;
    /// <summary>
    /// Audio clip to play in tutorials
    /// </summary>
    public AudioClip AudioClip { get; private set; } = default;

    /// <summary>
    /// Setup audio source
    /// Set up default language
    /// Get all texts in language file
    /// </summary>
    protected override void Awake()
    {
        ChangeLanguage(defaultLanguage);
        StateModel.OnModuleChanged.AddListener(GetScenariosTitles);
        StateModel.OnModuleChanged.AddListener(SendLanguages);
    }

    /// <summary>
    /// Changes currently loaded language
    /// </summary>
    /// <param name="language">new laguage</param>
    public void ChangeLanguage(Language language)
    {
        currentLanguage = language;
        GetAllTexts();
        ControllersManager.Instance.GetController<InstructionController>().LoadInstruction(false);
        ControllersManager.Instance.GetController<AidesController>().RefreshTexts();
    }

    public void ChangeLectorVolume(float _volume)
    {
        lectorAudio.volume = _volume;
    }

    /// <summary>
    /// Gets text by id
    /// </summary>
    /// <param name="name">Text id in elements collection</param>
    public LangugeElement GetByName(string name)
    {
        return languageElements.Find(x => x.id == name);
    }

    /// <summary>
    /// Gets all the data stored in XML file
    /// </summary>
    [ContextMenu("Look up texts")]
    private void GetAllTexts()
    {
        path = Application.dataPath + "/Texts/" + currentLanguage.ToString();
        path += "/" + (currentLanguage + ".xml");
        xml = new XmlDocument();
        xml.Load(path);

        languageElements.Clear();

        LoadStepTexts();
        LoadHintTexts();
    }

    private void LoadStepTexts()
    {
        XmlNode stepsNode = xml.SelectSingleNode("body").SelectSingleNode("steps");
        var stepsChildNodes = stepsNode.ChildNodes;

        for (int i = 0; i < stepsChildNodes.Count; i++)
        {
            LangugeElement element = new LangugeElement();
            element.id = stepsChildNodes.Item(i).Name;
            foreach (XmlNode node in stepsChildNodes.Item(i).ChildNodes)
            {
                if (node.Name == "Title")
                    element.title = node.InnerXml;
                if (node.Name == "Text")
                {
                    var res = WebUtility.HtmlDecode(node.InnerText);
                    element.text = res;
                }
                if (node.Name == "Audio")
                    element.audioClipPath = node.InnerXml;
                if (node.Name == "Picture")
                    element.mediaPath = node.InnerXml;
            }
            languageElements.Add(element);
        }
    }

    private void LoadHintTexts()
    {
        XmlNode hintsNode = xml.SelectSingleNode("body").SelectSingleNode("hints");
        var hintsChildNodes = hintsNode.ChildNodes;

        for (int i = 0; i < hintsChildNodes.Count; i++)
        {
            var parentObject = hintsChildNodes.Item(i).Name;
            var hintTargetNodes = hintsChildNodes.Item(i).ChildNodes;

            foreach (XmlNode hintTarget in hintTargetNodes)
            {
                LangugeElement element = new LangugeElement();
                element.id = parentObject + hintTarget.Name;
                foreach (XmlNode node in hintTarget.ChildNodes)
                {
                    if (node.Name == "Title")
                        element.title = node.InnerXml;
                    if (node.Name == "Text")
                    {
                        var res = WebUtility.HtmlDecode(node.InnerText);
                        element.text = res;
                    }
                }
                languageElements.Add(element);
            }
        }
    }

    /// <summary>
    /// Gets audio file from language element
    /// </summary>
    /// <param name="id">ID of audio file</param>
    public void GetAudio(string id)
    {
        string directory = id.Substring(0, 3) + "/" + id.Substring(3, 3) + "/" + id.Substring(6, 3);
        string audioDirectoryPath = Application.dataPath + "/" + mediaDirectory + "/" + directory + "/Audio/";
        DirectoryInfo d = new DirectoryInfo(audioDirectoryPath);
        FileInfo[] audioFiles = d.GetFiles().Where(s => s.FullName.EndsWith(".ogg")).ToArray();
        var audioFile = audioFiles.ToList().Find(x => x.FullName.EndsWith(currentLanguage.ToString() + ".ogg"));
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioFile.FullName, AudioType.OGGVORBIS);
        var operation = www.SendWebRequest();
        operation.completed += (op) => { AudioRequestFinished(op, www); };
    }

    /// <summary>
    /// Gets media file from language element
    /// </summary>
    /// <param name="id">ID of texture file</param>
    public void GetMedia(string id)
    {
        Sprites = new List<Sprite>();
        Url = null;
        imageLoaded = false;
        string directory = id.Substring(0, 3) + "/" + id.Substring(3, 3) + "/" + id.Substring(6, 3);
        string mediaDirectoryPath = Application.dataPath + "/" + mediaDirectory + "/" + directory + "/" + "Picture";
        DirectoryInfo d = new DirectoryInfo(mediaDirectoryPath);
        FileInfo[] imageFiles = d.GetFiles().Where(s => s.FullName.EndsWith(".jpg") || s.FullName.EndsWith(".png")).ToArray();
        FileInfo[] videoFiles = d.GetFiles().Where(s => s.FullName.EndsWith(".mp4") || s.FullName.EndsWith(".avi")).ToArray();
        if (imageFiles.Length > 0 && videoFiles.Length == 0)
        {
            LoadImages(imageFiles);
        }
        else if (videoFiles.Length > 0 && imageFiles.Length == 0)
        {
            LoadVideo(videoFiles);
        }
        else
        {
            Debug.LogWarning("NO IMAGE OR VIDEO FOR THIS STEP");
        }
    }

    /// <summary>
    /// Loads images 
    /// </summary>
    /// <param name="files"></param>
    private void LoadImages(FileInfo[] files)
    {
        foreach (FileInfo file in files)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(file.FullName);
            var operation = www.SendWebRequest();
            operation.completed += (op) => { TextureRequestFinished(op, www); };
        }
    }

    /// <summary>
    /// Loads video to urls list
    /// </summary>
    /// <param name="files">Files to add to list</param>
    private void LoadVideo(FileInfo[] files)
    {
        Url = files[0].FullName;
    }


    /// <summary>
    /// Callback when audio was loaded
    /// </summary>
    /// <param name="operation">Currenlty ongoid auiod download operation</param>
    /// <param name="www">Request containing audio file</param>
    private void AudioRequestFinished(AsyncOperation operation, UnityWebRequest www)
    {
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            AudioClip = DownloadHandlerAudioClip.GetContent(www);
            Play();
        }
        www.Dispose();
    }

    /// <summary>
    /// Callback when audio was loaded
    /// </summary>
    /// <param name="operation">Currenlty ongoid auiod download operation</param>
    /// <param name="www">Request containing audio file</param>
    private void TextureRequestFinished(AsyncOperation operation, UnityWebRequest www)
    {
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(www);
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            sprite.name = www.uri.LocalPath;
            Sprites.Add(sprite);
            Sprites.Sort(delegate (Sprite x, Sprite y)
            {
                return x.name.CompareTo(y.name);
            });
            imageLoaded = true;
        }
        www.Dispose();
    }

    /// <summary>
    /// Plays loaded audio file 
    /// </summary>
    public void Play()
    {
        lectorAudio.clip = AudioClip;
        lectorAudio.Play();
    }

    /// <summary>
    /// Gets all scenarios titles and concatenates it into a string
    /// </summary>
    /// <param name="module">Module template to be able to connect to module change</param>
    public void GetScenariosTitles(Module module)
    {
        string scenariosTitlesMessage = "scenarios,";
        foreach (LangugeElement le in languageElements)
        {
            if (le.id.EndsWith("S01"))
            {
                scenariosTitlesMessage += le.id.Replace("M0", "")[0] + "-";
                var scenarioNumber = le.id.Substring(4)[0] != '0' ? le.id.Substring(4, 2) : le.id.Substring(5, 1);
                scenariosTitlesMessage += scenarioNumber + ": ";
                scenariosTitlesMessage += le.title + ",";
            }
        }
        scenariosTitlesMessage = scenariosTitlesMessage.TrimEnd(',');
        FindObjectOfType<ConnectionController>().SendScenariosTitles(scenariosTitlesMessage);
    }

    public void SendLanguages(Module module)
    {
        var values = Enum.GetValues(typeof(Language));
        string sendString = "languages,";
        foreach (Language language in values)
        {
            sendString += language.ToString() + ",";
        }
        FindObjectOfType<ConnectionController>().SendLanguages(sendString);
    }
}
