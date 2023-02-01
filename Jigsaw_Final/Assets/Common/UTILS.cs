using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// Texture 크기 만큼의 Sprite를 생성
    /// </summary>
    /// <param name="SpriteTexture"></param>
    /// <returns></returns>
    public static Sprite CreateSpriteFromTexture2D(Texture2D SpriteTexture)
    {
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.Tight);
        return NewSprite;
    }

    /// <summary>
    /// Texture에서 설정에 맞추어 Sprite 생성
    /// </summary>
    /// <param name="SpriteTexture"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="PixelsPerUnit"></param>
    /// <param name="spriteType"></param>
    /// <returns></returns>
    public static Sprite CreateSpriteFromTexture2D(Texture2D SpriteTexture, int x, int y, int width, int height, float PixelsPerUnit = 1f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(x, y, width, height), new Vector2(0.5f, 0.5f), PixelsPerUnit, 0, spriteType);
        return NewSprite;
    }

    /// <summary>
    /// 텍스쳐의 사이즈를 보간법을 사용하여 변환
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
    /// 해당 SpriteRender가 화면상에서 차지하는 영역을 반환합니다
    /// [0]: 최소점, [1]: 최대점
    /// </summary>
    /// <returns></returns>
    public static Vector3[] getSpritesArea(SpriteRenderer sp)
    {
        Vector3[] vecs = new Vector3[2];

        vecs[0] = sp.bounds.min;
        vecs[1] = sp.bounds.max;

        return vecs;
    }

}
