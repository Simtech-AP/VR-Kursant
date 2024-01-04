using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Contans logic for controlling media playing and loading proper assets
/// </summary>
public class MediaController : Controller
{
    /// <summary>
    /// Reference to language controller
    /// </summary>
    [SerializeField]
    private LanguageController languageController = default;

    /// <summary>
    /// Reference to video player
    /// </summary>
    [SerializeField]
    private VideoPlayer videoPlayer = default;

    /// <summary>
    /// Reference to image used when showing a gif
    /// </summary>
    [SerializeField]
    private Image textureImage = default;

    /// <summary>
    /// Reference to video image used when showing a video
    /// </summary>
    [SerializeField]
    private GameObject videoTargetObject = default;

    /// <summary>
    /// Loads media to show and sets up proper method to show it
    /// </summary>
    /// <param name="stepName">Step name (id) to get media from</param>
    public void LoadMediaStream(string stepName)
    {
        languageController.GetMedia(languageController.GetByName(stepName).id);
        languageController.GetAudio(languageController.GetByName(stepName).id);
        Action act = languageController.Url != null ? act = () => PlayVideo(languageController.Url) : () => StartCoroutine(AwaitImageLoad());
        act.Invoke();
    }

    private IEnumerator AwaitImageLoad()
    {
        while (languageController.imageLoaded == false)
        {
            yield return null;
        }

        ShowImage(languageController.Sprites[0]);
    }

    private void ShowImage(Sprite image)
    {
        textureImage.sprite = null;
        videoPlayer.targetTexture.Release();
        textureImage.gameObject.SetActive(true);
        videoTargetObject.SetActive(false);
        textureImage.sprite = image;
    }

    /// <summary>
    /// Plays loaded video using player
    /// </summary>
    /// <param name="url">Url of video</param>
    private void PlayVideo(string url)
    {
        StopGif();
        videoPlayer.targetTexture.Release();
        textureImage.gameObject.SetActive(false);
        videoTargetObject.SetActive(true);
        videoPlayer.url = url;
    }

    /// <summary>
    /// Plays "gif" from loaded images
    /// </summary>
    /// <param name="sprites">List of images loaded as a "gif"</param>
    private void PlayGif(List<Sprite> sprites)
    {
        Gif gif = new Gif(sprites);
        textureImage.sprite = null;
        videoPlayer.targetTexture.Release();
        textureImage.gameObject.SetActive(true);
        videoTargetObject.SetActive(false);
        StartCoroutine(LoopGif(gif));
    }

    /// <summary>
    /// Stops playing of gif
    /// </summary>
    private void StopGif()
    {
        StopAllCoroutines();
        textureImage.sprite = null;
    }

    /// <summary>
    /// Main coroutine for playing gif
    /// </summary>
    /// <param name="gif">Gif to play</param>
    /// <returns>Handle to coroutine</returns>
    private IEnumerator LoopGif(Gif gif)
    {
        yield return new WaitUntil(() => gif.HasSpritesLoaded());
        while (true)
        {
            textureImage.sprite = gif.Tick();
            yield return new WaitForEndOfFrame();
        }
    }
}
