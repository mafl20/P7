using UnityEngine;

public class ElementCreator : MonoBehaviour
{
    public static ElementCreator instance;
    [SerializeField] GameObject elementPrefab;

    public byte[] activeElements;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Returns an element GameObject with a specified number of protons, position, and parent.
    /// </summary>
    public GameObject CreateElement(byte numberOfProtons, Vector2 newPosition, Transform newParent)
    {
        var newElement = Instantiate(elementPrefab, transform);
        newElement.GetComponent<ElementProperties>().SetProtons(numberOfProtons);
        newElement.GetComponent<ElementProperties>().UpdatePropertiesFromProtons();
        newElement.GetComponent<ElementGraphics>().UpdateGraphics(newPosition, newParent);

        return newElement;
    }

    /// <summary>
    /// Returns an element GameObject with a specified number of protons and parent.
    /// </summary>
    public GameObject CreateElement(byte numberOfProtons, Transform newParent)
    {
        return CreateElement(numberOfProtons, GetComponent<RectTransform>().position, newParent);
    }

    /// <summary>
    /// Returns an element GameObject with a specified number of protons and position.
    /// </summary>
    public GameObject CreateElement(byte numberOfProtons, Vector2 newPosition)
    {
        return CreateElement(numberOfProtons, newPosition, transform);
    }

    /// <summary>
    /// Returns an element GameObject with a specified number of protons.
    /// </summary>
    public GameObject CreateElement(byte numberOfProtons)
    {
        return CreateElement(numberOfProtons, GetComponent<RectTransform>().position);
    }

    /// <summary>
    /// Returns a random element GameObject with a specified position and parent.
    /// </summary>
    public GameObject CreateRandomElement(Vector2 newPosition, Transform newParent)
    {
        var numberOfProtons = PickRandomNumberOfProtonsFromActiveElements();
        return CreateElement(numberOfProtons, newPosition, newParent);
    }

    /// <summary>
    /// Returns a random element GameObject with a specified position.
    /// </summary>
    public GameObject CreateRandomElement(Vector2 newPosition)
    {
        var numberOfProtons = PickRandomNumberOfProtonsFromActiveElements();
        return CreateElement(numberOfProtons, newPosition, transform);
    }

    /// <summary>
    /// Returns a random element GameObject with a specified parent.
    /// </summary>
    public GameObject CreateRandomElement(Transform newParent)
    {
        return CreateRandomElement(GetComponent<RectTransform>().position, newParent);
    }

    /// <summary>
    /// Returns a random element GameObject.
    /// </summary>
    public GameObject CreateRandomElement()
    {
        return CreateRandomElement(transform);
    }

    byte PickRandomNumberOfProtonsFromActiveElements()
    {
        byte randomIndex = (byte)Random.Range(1, activeElements.Length);
        byte randomNumberOfProtons = activeElements[randomIndex];
        return randomNumberOfProtons;
    }
}