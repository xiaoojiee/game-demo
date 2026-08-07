using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 武器截图预览 — 用独立摄像头渲染一帧武器，存成 Sprite 显示在 Image 上。
/// </summary>
public class WeaponPreview : MonoBehaviour
{
    public Camera previewCamera;
    public Transform previewStage;      // 武器临时放这里
    public int imageSize = 128;

    private RenderTexture rt;

    void Start()
    {
        rt = new RenderTexture(imageSize, imageSize, 16);
        if (previewCamera != null) previewCamera.targetTexture = rt;
    }

    /// <summary>给武器截图，返回 Sprite</summary>
    public Sprite Capture(GameObject weaponPrefab)
    {
        Clear();

        var clone = Instantiate(weaponPrefab, previewStage);
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.Euler(0, 180, 0);
        clone.SetActive(true);

        // 渲染一帧
        previewCamera.Render();

        // 读像素
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(imageSize, imageSize, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, imageSize, imageSize), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        Destroy(clone);

        return Sprite.Create(tex, new Rect(0, 0, imageSize, imageSize), Vector2.one * 0.5f);
    }

    void Clear()
    {
        foreach (Transform child in previewStage)
            Destroy(child.gameObject);
    }

    void OnDestroy()
    {
        if (rt != null) rt.Release();
    }
}
