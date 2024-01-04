using System.IO;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Controller in charge of settings of the application
/// </summary>
public class SettingsController : Controller
{
    /// <summary>
    /// Settings data container
    /// </summary>
    [SerializeField]
    private Settings settings = default;

    /// <summary>
    /// Loads settings saved in a file
    /// </summary>
    public void LoadSettings()
    {
        if (!(File.Exists(Application.persistentDataPath + "/settings.data"))) return;
        string path = Application.persistentDataPath + "/settings.data";
        StreamReader reader = new StreamReader(path);
        FieldInfo[] fields = settings.GetType().GetFields();
        foreach (FieldInfo fi in fields)
        {
            var data = reader.ReadLine();
            if (fi.FieldType == typeof(float))
            {
                var single = float.Parse(data.Split(':')[1]);
                fi.SetValue(settings, single);
            }
            else if (fi.FieldType == typeof(bool))
            {
                var field = bool.Parse(data.Split(':')[1]);
                fi.SetValue(settings, field);
            }
        }
        reader.Close();
    }

    /// <summary>
    /// Save settings to a file
    /// </summary>
    public void SaveSettings()
    {
        string path = Application.persistentDataPath + "/settings.data";
        StreamWriter writer = new StreamWriter(path, false);
        FieldInfo[] fields = settings.GetType().GetFields();
        foreach (FieldInfo fi in fields)
        {
            writer.WriteLine(fi.Name + ":" + fi.GetValue(settings));
        }
        writer.Close();
    }


    /// <summary>
    /// Change volume of sounds
    /// </summary>
    /// <param name="up">Should the volume change up?</param>
    public void ChangeVolume(bool up)
    {
        if (up)
        {
            settings.volume += 0.1f;
            if (settings.volume > 1f) settings.volume = 1f;
        }
        else
        {
            settings.volume -= 0.1f;
            if (settings.volume < 0f) settings.volume = 0f;
        }
    }
}
