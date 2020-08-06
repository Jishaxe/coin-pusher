using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CoinMarkingService
{
    private class CachedMarkedMaterial
    {
        public Texture2D OriginalTexture;
        public Material BaseMaterial;
        public Texture2D ConvertedTexture;
        public Material MarkedMaterial;
    }

    private Dictionary<Texture2D, Texture2D> _cachedNormalMaps = new Dictionary<Texture2D, Texture2D>();
    private List<CachedMarkedMaterial> _cache = new List<CachedMarkedMaterial>();

    public Material GetMarkedMaterial(Material baseMaterial, Texture2D texture)
    {
        var cached = GetCached(baseMaterial, texture);
        if (cached != null)
        {
            return cached.MarkedMaterial;
        }

        var newCache = new CachedMarkedMaterial
        {
            BaseMaterial = baseMaterial,
            OriginalTexture = texture
        };
        
        var markedMaterial = new Material(baseMaterial);
        var convertedTexture = GetOrCreateNormalMap(texture);

        markedMaterial.EnableKeyword ("_NORMALMAP");
        markedMaterial.SetTexture("_BumpMap", convertedTexture);
        
        newCache.MarkedMaterial = markedMaterial;
        newCache.ConvertedTexture = convertedTexture;
        _cache.Add(newCache);

        return newCache.MarkedMaterial;
    }

    private Texture2D GetOrCreateNormalMap(Texture2D original)
    {
        if (_cachedNormalMaps.TryGetValue(original, out var normal))
        {
            return normal;
        }
        else
        {
            var newNormal = CreateNormalTexture(original);
            _cachedNormalMaps[original] = newNormal;
            return newNormal;
        }
    }

    private Texture2D CreateNormalTexture(Texture2D source)
    {
        Texture2D normalTexture = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);
        Color theColour = new Color();
        for (int x = 0; x < source.width; x++)
        {
            for (int y = 0; y < source.height; y++)
            {
                theColour.r = 0;
                theColour.g = source.GetPixel(x, y).g;
                theColour.b = 0;
                theColour.a = source.GetPixel(x, y).r;
                normalTexture.SetPixel(x, y, theColour);
            }
        }
        normalTexture.Apply();
        
        return normalTexture;
    }

    private CachedMarkedMaterial GetCached(Material baseMaterial, Texture2D texture)
    {
        return _cache.FirstOrDefault((cached) =>
            cached.BaseMaterial == baseMaterial && cached.OriginalTexture == texture);
    }
}
