using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Services
{
    public class ImageProvisionService : MonoBehaviour
    {
        private Dictionary<string, Texture2D> _cachedImages = new Dictionary<string, Texture2D>();

        public void ResolveImage(string url, Action<Texture2D> OnComplete)
        {
            if (_cachedImages.TryGetValue(url, out var texture))
            {
                OnComplete.Invoke(texture);
            }
            else
            {
                StartCoroutine(GetTexture(url, OnComplete));
            }
        }
        
        IEnumerator GetTexture(string url, Action<Texture2D> OnComplete) {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError($"Error while getting texture from URL {url}: {www.error}");
                OnComplete.Invoke(null);
            }
            else 
            {
                var tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                _cachedImages[url] = tex;
                OnComplete.Invoke(tex);
            }
        }
    }
}