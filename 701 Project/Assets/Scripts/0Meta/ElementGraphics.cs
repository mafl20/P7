using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElementGraphics : MonoBehaviour
{
    Sprite elementSprite;
    public Image elementImage;
    public TMP_Text symbolText;
    public TMP_Text strengthText;
    public TMP_Text healthText;

    public bool isIon;

    /// <summary>
    /// Update the graphics, position, and parent of the element GameObject.
    /// </summary>
    public void UpdateGraphics(Vector2 newPosition, Transform newParent)
    {
        SetPosition(newPosition);
        SetParent(newParent);
        UpdatePropertiesGraphics();
    }

    /// <summary>
    /// Update the graphics and name of the element GameObject to match the properties of the attached ElementProperties.cs.
    /// </summary>
    public void UpdatePropertiesGraphics()
    {
        var elementProperties = GetComponent<ElementProperties>();

        elementSprite = ElementSpriteHolder.instance.GetSpriteFromProtons(elementProperties.GetProtons());
        elementImage.sprite = elementSprite;

        symbolText.text = elementProperties.GetSymbol();

        if (isIon == true) { strengthText.text = elementProperties.GetIonDamage().ToString(); }
        else               { strengthText.text = elementProperties.GetAtomDamage().ToString(); }

        healthText.text = elementProperties.GetHealth().ToString();

        gameObject.name = elementProperties.GetChemicalName();
    }

    public void SetPosition(Vector2 newPosition) { GetComponent<RectTransform>().position = newPosition; }
    public void SetParent(Transform newParent) { GetComponent<RectTransform>().SetParent(newParent); }
}