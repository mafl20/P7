using System;
using UnityEngine;
using System.Collections.Generic;

 [Serializable]
 public struct UnitSprite
{
    public string name;
    public Sprite sprite;
}

public class UnitSpriteHolder : MonoBehaviour
{
    public static UnitSpriteHolder instance;

    public UnitSprite[] unitSprites;

    Dictionary<string,Sprite> unitSpritesDictionary = new Dictionary<string, Sprite>();
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (byte i = 0; i < unitSprites.Length; i++)
        {
            var unitName   = unitSprites[i].name;
            var unitSprite = unitSprites[i].sprite;
            unitSpritesDictionary.Add(unitName, unitSprite);
        }
    }

    public Sprite GetElementSpriteFromNumberOfProtons(byte numberOfProtons)
    {
        string chemicalName = ElementDataReader.instance.GetChemicalNameFromNumberOfProtons(numberOfProtons);
        return GetSpriteFromName(chemicalName);
    }

    public Sprite GetSpriteFromName(string name)
    {
        return unitSpritesDictionary[name];
    }
}