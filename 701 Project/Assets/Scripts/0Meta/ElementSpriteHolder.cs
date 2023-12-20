using UnityEngine;
using System.Collections.Generic;

public class ElementSpriteHolder : MonoBehaviour
{
    public static ElementSpriteHolder instance;

    public Sprite[] elementSprites;
    private void Awake()
    {
        instance = this;
    }

    public Sprite GetSpriteFromProtons(byte protons)
    {
        return elementSprites[protons];
    }
}