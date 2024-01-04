using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for representing Gif images in Unity
/// </summary>
[System.Serializable]
public class Gif
{
    /// <summary>
    /// List of Sprites in gif
    /// </summary>
    [SerializeField]
    private List<Sprite> sprites;
    /// <summary>
    /// Amount of frames that passes in a second of gametime
    /// </summary>
    public int framesPerSecond = 5;
    /// <summary>
    /// Reference to currently shown Sprite
    /// </summary>
    private Sprite sprite;

    /// <summary>
    /// Default construction requiring a list of Sprites to build a Gif
    /// </summary>
    /// <param name="sprites">List of Sprites</param>
    public Gif(List<Sprite> sprites)
    {
        this.sprites = sprites;
    }

    /// <summary>
    /// Method for determining what frame of Gif should be shown at current time
    /// </summary>
    /// <returns>Currently shown Sprite</returns>
    public Sprite Tick()
    {
        var index = Time.time * framesPerSecond;
        index = index % sprites.Count;
        sprite = sprites[(int)index];
        return sprite;
    }

    /// <summary>
    /// Determines if Gif has been initialized and sprites have been loaded
    /// </summary>
    /// <returns>Is Gif ready to be shown?</returns>
    internal bool HasSpritesLoaded()
    {
        return sprites.Count > 0;
    }
}
