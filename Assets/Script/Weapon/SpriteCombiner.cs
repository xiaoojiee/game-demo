using UnityEngine;

public static class SpriteCombiner
{
    public static Sprite Combine(Sprite blade, Sprite handle, Sprite guard)
    {
        var first = blade ?? handle ?? guard;
        if (first == null) return null;

        int w = (int)first.textureRect.width;
        int h = (int)first.textureRect.height;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        Color[] result = new Color[w * h];
        for (int i = 0; i < result.Length; i++) result[i] = Color.clear;

        Overlay(result, guard,  w, h);
        Overlay(result, handle, w, h);
        Overlay(result, blade,  w, h);

        tex.SetPixels(result);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
    }

    static void Overlay(Color[] target, Sprite sprite, int tw, int th)
    {
        if (sprite == null) return;
        try
        {
            var srcTex = sprite.texture;
            var r = sprite.textureRect;
            int sx = (int)r.x, sy = (int)r.y, sw = (int)r.width, sh = (int)r.height;
            Color[] src = srcTex.GetPixels(sx, sy, sw, sh);

            for (int y = 0; y < th; y++)
            {
                for (int x = 0; x < tw; x++)
                {
                    int si = y * sw + x;
                    if (si < src.Length && src[si].a > 0)
                        target[y * tw + x] = src[si];
                }
            }
        }
        catch (UnityException) { }
    }
}
