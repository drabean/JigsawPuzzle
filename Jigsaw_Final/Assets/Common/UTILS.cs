using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public enum DIFFICULTY
{
    EASY,
    NORMAL,
    HARD,
    MASTER
}
public enum GAMETYPE
{
    DRAW,
    CAMERA,
    SELECT
}
public class UTILS : MonoBehaviour
{
    /// <summary>
    /// Texture ũ�� ��ŭ�� Sprite�� ����
    /// </summary>
    /// <param name="SpriteTexture"></param>
    /// <returns></returns>
    public static Sprite CreateSpriteFromTexture2D(Texture2D SpriteTexture)
    {
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.Tight);
        return NewSprite;
    }

    /// <summary>
    /// Texture���� ������ ���߾� Sprite ����
    /// </summary>
    /// <param name="SpriteTexture"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="PixelsPerUnit"></param>
    /// <param name="spriteType"></param>
    /// <returns></returns>
    public static Sprite CreateSpriteFromTexture2D(Texture2D SpriteTexture, int x, int y, int width, int height, float PixelsPerUnit, SpriteMeshType spriteType)
    {
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(x, y, width, height), new Vector2(0.5f, 0.5f), PixelsPerUnit, 0, spriteType);
        return NewSprite;
    }

    /// <summary>
    /// �ؽ����� ����� �������� ����Ͽ� ��ȯ
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetWidth"></param>
    /// <param name="targetHeight"></param>
    /// <returns></returns>
    public static Texture2D RescaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    public static Texture2D RescaleTextureByWidth(Texture2D source, int targetWidth)
    {
        int targetHeight = (int)(targetWidth * ((float)source.texelSize.x / source.texelSize.y));
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    /// <summary>
    /// �ش� SpriteRender�� ȭ��󿡼� �����ϴ� ������ ��ȯ�մϴ�
    /// [0]: �ּ���, [1]: �ִ���
    /// </summary>
    /// <returns></returns>
    public static Vector3[] getSpritesArea(SpriteRenderer sp)
    {
        Vector3[] vecs = new Vector3[2];

        vecs[0] = sp.bounds.min;
        vecs[1] = sp.bounds.max;

        return vecs;
    }

    /// <summary>
    /// List�� �Է¹޾� �ߺ��Ǵ� ��ҵ��� ������ List�� ��ȯ�մϴ�.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<T> removeDuplicates<T>(List<T> list)
    {
        return new HashSet<T>(list).ToList();
    }

    public static void savePicture(Texture2D tex)
    {

#if UNITY_EDITOR
        string fileLocation = "Assets/Captures/";   // ������ ��� ����
#elif UNITY_ANDROID
        string fileLocation = $"/storage/emulated/0/DCIM/{Application.productName}/";   // ������ ��� ����
#endif
        string timeName = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");          // ��¥ ����
        string fileName = "Picture" + timeName + ".png";                                // ������ �̸� ����
                                                                                        // string filePath = fileLocation + fileName;
        string filePath = fileLocation + fileName;

        if (!Directory.Exists(fileLocation)) Directory.CreateDirectory(fileLocation);

        byte[] imageData = tex.EncodeToPNG();
#if UNITY_EDITOR
        File.WriteAllBytes(filePath, imageData);

#elif UNITY_ANDROID
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(imageData, Application.productName, fileName, (success, path) => Debug.Log("Media save result: " + success + " " + path));
#endif
    }


}
