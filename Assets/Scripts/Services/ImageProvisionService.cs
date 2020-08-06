using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace Services
{
    public sealed class ImageResolution
    {
        private bool _resolved;
        private Texture2D _result;
        private List<Action<Texture2D>> _callbacks = new List<Action<Texture2D>>();

        public void OnResolved(Texture2D resolved)
        {
            _resolved = true;
            _result = resolved;
            CheckForResolution();
        }
        
        public void AddCallback(Action<Texture2D> callback)
        {
            _callbacks.Add(callback);
            CheckForResolution();
        }

        private void CheckForResolution()
        {
            if (_resolved)
            {
                foreach (var callback in _callbacks)
                {
                    callback.Invoke(_result);
                }

                _callbacks.Clear();
            }
        }
    }
    
    public class ImageProvisionService : MonoBehaviour
    {
        private Dictionary<string, ImageResolution> _resolutions = new Dictionary<string, ImageResolution>();

        public void ResolveImage(string url, Action<Texture2D> OnComplete)
        {
            if (String.IsNullOrEmpty(url))
            {
                OnComplete?.Invoke(null);
                return;
            }
            
            if (_resolutions.TryGetValue(url, out var resolution))
            {
                resolution.AddCallback(OnComplete);
            }
            else
            {
                var res = new ImageResolution();
                res.AddCallback(OnComplete);
                StartCoroutine(GetTexture(url, res.OnResolved));
                _resolutions[url] = res;
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
                tex.name = url;

                OnComplete.Invoke(tex);
            }
        }
    }
}