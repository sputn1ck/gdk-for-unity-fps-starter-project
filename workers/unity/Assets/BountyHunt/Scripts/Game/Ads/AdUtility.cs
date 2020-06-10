using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class AdUtility : MonoBehaviour
{
    public Material defaultSquareBillboardMaterial;

    public static AdUtility instance;

    private void Awake()
    {
        instance = this;
    }

    public VideoPlayer GetNewVideoPlayer()
    {
        VideoPlayer vp = gameObject.AddComponent<VideoPlayer>();
        vp.source = VideoSource.Url;
        vp.isLooping = true;
        vp.renderMode = VideoRenderMode.RenderTexture;
        vp.audioOutputMode = VideoAudioOutputMode.None;
        return vp;
    }

    public static async Task<(List<Texture2D>, List<string>)> getTexturesFromURLList(List<string> urls)
    {

        List<Texture2D> textures = new List<Texture2D>();
        List<string> videoUrls = new List<string>();
        if (urls == null)
        {
            Debug.LogWarning("url list is null");
            return (textures, videoUrls);
        }
        foreach (string url in urls)
        {
            string extension = Path.GetExtension(url);
            switch (extension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".exr":
                case ".hdr":
                case ".iff":
                case ".pict":
                case ".psd":
                case ".tga":
                case ".tiff":
                case ".gif":


                    Texture2D tex = await AwaitRequestTexture.SendAsyncWebRequest(PlayerServiceConnections.instance, url);

                    if (tex == null)
                    {
                        continue;
                    }

                    Color backgroundColor = Color.white;
                    var pixels = tex.GetPixels();

                    for (int i = 0; i < pixels.Length; i++)
                    {
                        float a = pixels[i].a;
                        pixels[i] = Color.Lerp(backgroundColor, pixels[i], a);
                        pixels[i].a = a;
                    }
                    tex.SetPixels(pixels);
                    tex.Apply();
                    textures.Add(tex);

                    break;

                case ".dv":
                case ".m4v":
                case ".mov":
                case ".mp4":
                case ".mpg":
                case ".mpeg":
                case ".ogv":
                case ".vp8":
                case ".webm":

                    videoUrls.Add(url);

                    break;

                default:
                    Debug.LogError("File Type not " + extension + "supportet!");
                    break;
            }


        }

        return (textures, videoUrls);
    }

    private void OnApplicationQuit()
    {
        UrlMemory.TryOpenAllUrls();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            UrlMemory.TryOpenAllUrls();
        }
    }

}

public static class UrlMemory
{
    public static bool DoNotOpenAllLinksNextTime;

    static List<string> urlQueue = new List<string>();

    public static void AddUrl(string url)
    {
        if (!urlQueue.Contains(url))
        {
            urlQueue.Add(url);
        }
    }

    public static bool UrlInQueue(string url)
    {
        return urlQueue.Contains(url);
    }

    public static void TryOpenAllUrls()
    {
        if (DoNotOpenAllLinksNextTime)
        {
            DoNotOpenAllLinksNextTime = false;
            return;
        }

        foreach (string link in urlQueue)
        {
            Application.OpenURL(link);
        }
        urlQueue.Clear();
    }

}

