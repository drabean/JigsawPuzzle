using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public Image circlePalette;
    public Image picker;
    public Color selectedColor;

    private Vector2 sizeOfPalette;
    private CircleCollider2D paletteCollider;

    public IColorPicker manager;

    public GameObject Panel_ColorPicker;
    void Start()
    {
        paletteCollider = circlePalette.GetComponent<CircleCollider2D>();

        sizeOfPalette = new Vector2(circlePalette.GetComponent<RectTransform>().rect.width, circlePalette.GetComponent<RectTransform>().rect.height);


    }

    public void setIColorPicker(IColorPicker manager)
    {
        this.manager = manager;
    }

    public void mousePointerDown()
    {
        Vector3 offset = Input.mousePosition - transform.position;
        Vector3 diff = Vector3.ClampMagnitude(offset, paletteCollider.radius);

        picker.transform.position = transform.position + diff;
    }

    public void mouseDrag()
    {
        Vector3 offset = Input.mousePosition - transform.position;
        Vector3 diff = Vector3.ClampMagnitude(offset, paletteCollider.radius);

        picker.transform.position = transform.position + diff;
    }


    public void mouseUp()
    {
        selectColor();
    }


    private Color getColor()
    {
        Vector2 circlePalettePosition = circlePalette.transform.position;
        Vector2 pickerPosition = picker.transform.position;

        Vector2 position = pickerPosition - circlePalettePosition + sizeOfPalette * 0.5f;
        Vector2 normalized = new Vector2((position.x / (circlePalette.GetComponent<RectTransform>().rect.width)), (position.y / (circlePalette.GetComponent<RectTransform>().rect.height)));

        Texture2D texture = circlePalette.mainTexture as Texture2D;
        Color circularSelectedColor = texture.GetPixelBilinear(normalized.x, normalized.y);

        return circularSelectedColor;
    }

    private void selectColor()
    {
        Vector3 offset = Input.mousePosition - transform.position;
        Vector3 diff = Vector3.ClampMagnitude(offset, paletteCollider.radius);

        picker.transform.position = transform.position + diff;

        selectedColor = getColor();


        manager.setPickerColor(selectedColor);
        CloseColorPIcker();
    }


    public void OpenColorPicker()
    {
        Panel_ColorPicker.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void CloseColorPIcker()
    {
        Panel_ColorPicker.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
