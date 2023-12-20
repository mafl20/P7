using UnityEngine;

public class HandbookPtableButtonData : MonoBehaviour
{
    public Sprite elementSprite, chargeSprite;

    public string chemicalName;
    public string damage, health, charge;

    public void HandbookUpdateGraphics()
    {
        Handbook.instance.__UpdateGraphics(elementSprite, chargeSprite, chemicalName, damage, health, charge);
    }
}