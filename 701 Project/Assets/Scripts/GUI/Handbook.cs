using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Handbook : MonoBehaviour
{
    public Image elementImage, chargeImage;
    public TMP_Text chemicalNameTMP, damageTMP, healthTMP, chargeTMP;

    public void __UpdateGraphics(Sprite elementSprite, Sprite chargeSprite, string chemicalName, string damage, string health, string charge)
    {
        elementImage.sprite = elementSprite;
        chargeImage.sprite = chargeSprite;
        chemicalNameTMP.text = chemicalName;
        damageTMP.text = damage;
        healthTMP.text = health;
        chargeTMP.text = charge;
    }


    //------------------------------------

    public List<Button> period3;
    public List<Button> period4;
    public List<Button> period5;
    private List<Button> period;
    public Color colorPrevious = Color.white;

    public static Handbook instance;

    private void Awake() { instance = this; }

    private void Start()
    {
        __DisableAll();
    }

    public void __EnablePeriod(int periodToEnable)
    {
        switch (periodToEnable)
        {
            case 3:
                period = period3;
                break;
            case 4:
                period = period4;
                break;
            case 5:
                period = period5;
                break;
            default:
                period = period4;
                break;
        }

        foreach (Button b in period)
        {
            ChangeColor(b, colorPrevious);
        }
    }

    public void __DisableAll()
    {
        foreach (Button b in period3)
        {
            ChangeColor(b, Color.grey);
        }
        foreach (Button b in period4)
        {
            ChangeColor(b, Color.grey);
        }
        foreach (Button b in period5)
        {
            ChangeColor(b, Color.grey);
        }
    }

    private void ChangeColor(Button b, Color c) 
    {
        ColorBlock cb = b.colors;
        cb.normalColor = c;
        b.colors = cb;
    }

}
