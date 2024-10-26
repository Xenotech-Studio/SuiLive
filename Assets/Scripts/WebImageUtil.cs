using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using WebP;

public static class WebImageUtil
{
    private static int maxCacheSize = 50;
    private static int maxTextureSize = 256;

    private static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
    private static List<string> errorCache = new List<string>();

    private static Queue<string> spriteQueue = new Queue<string>();
    private static Queue<string> textureQueue = new Queue<string>();

    public static void SetMaxCacheSize(int size)
    {
        maxCacheSize = size;
        ManageCacheSize();
    }

    public static void SetMaxTextureSize(int size)
    {
        maxTextureSize = size;
    }

    public static async void GetSpriteByUrl(string url, System.Action<Sprite> callback)
    {
        if (spriteCache.ContainsKey(url) && spriteCache[url] != null)
        {
            callback?.Invoke(spriteCache[url]);
            return;
        }
        
        if (errorCache.Contains(url))
        {
            callback?.Invoke(null);
            return;
        }

        Texture2D texture = await DownloadTexture(url);
        if (texture != null)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            AddToCache(url, sprite);

            callback?.Invoke(sprite);
        }
        else
        {
            Debug.LogError($"await DownloadTexture returns null");
            errorCache.Add(url);
            callback?.Invoke(null);
        }
    }

    public static async void GetTexture2DByUrl(string url, System.Action<Texture2D> callback)
    {
        if (textureCache.ContainsKey(url))
        {
            callback?.Invoke(textureCache[url]);
            return;
        }
        
        if (errorCache.Contains(url))
        {
            callback?.Invoke(null);
            return;
        }

        Texture2D texture = await DownloadTexture(url);
        if (texture != null)
        {
            AddToCache(url, texture);
            callback?.Invoke(texture);
        }
        else
        {
            errorCache.Add(url);
            callback?.Invoke(null);
        }
    }

    private static async System.Threading.Tasks.Task<Texture2D> DownloadTexture(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            var asyncOperation = uwr.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await System.Threading.Tasks.Task.Yield();
            }

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                if (url.EndsWith(".webp"))
                {
                    byte[] imageData = uwr.downloadHandler.data;
                    Error lError;
                    Texture2D texture = Texture2DExt.CreateTexture2DFromWebP(imageData, lMipmaps: true, lLinear: false, lError: out lError);

                    if (lError == Error.Success)
                    {
                        return ResizeTexture(texture, maxTextureSize, maxTextureSize);
                    }
                    else
                    {
                        Debug.LogError($"WebP Load Error: {lError}");
                        return null;
                    }
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                    return ResizeTexture(texture, maxTextureSize, maxTextureSize);
                }
            }
            else
            {
                Debug.LogError($"Failed to download texture from {url}: {uwr.error}");
                return null;
            }
        }
    }

    private static void AddToCache(string url, Sprite sprite)
    {
        if (spriteQueue.Count >= maxCacheSize)
        {
            string oldestUrl = spriteQueue.Dequeue();
            if (spriteCache.ContainsKey(oldestUrl))
            {
                Object.Destroy(spriteCache[oldestUrl].texture);
                spriteCache.Remove(oldestUrl);
            }
        }

        spriteCache[url] = sprite;
        spriteQueue.Enqueue(url);
    }

    private static void AddToCache(string url, Texture2D texture)
    {
        if (textureQueue.Count >= maxCacheSize)
        {
            string oldestUrl = textureQueue.Dequeue();
            if (textureCache.ContainsKey(oldestUrl))
            {
                Object.Destroy(textureCache[oldestUrl]);
                textureCache.Remove(oldestUrl);
            }
        }

        textureCache[url] = texture;
        textureQueue.Enqueue(url);
    }

    private static void ManageCacheSize()
    {
        while (spriteQueue.Count > maxCacheSize)
        {
            string oldestUrl = spriteQueue.Dequeue();
            if (spriteCache.ContainsKey(oldestUrl))
            {
                Object.Destroy(spriteCache[oldestUrl].texture);
                spriteCache.Remove(oldestUrl);
            }
        }

        while (textureQueue.Count > maxCacheSize)
        {
            string oldestUrl = textureQueue.Dequeue();
            if (textureCache.ContainsKey(oldestUrl))
            {
                Object.Destroy(textureCache[oldestUrl]);
                textureCache.Remove(oldestUrl);
            }
        }
    }

    private static Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        Graphics.Blit(source, rt);
        RenderTexture.active = rt;

        Texture2D newTexture = new Texture2D(newWidth, newHeight);
        newTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        newTexture.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return newTexture;
    }
}
