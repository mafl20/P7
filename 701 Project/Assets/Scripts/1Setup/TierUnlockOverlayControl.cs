using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TierUnlockOverlayControl : MonoBehaviour
{
    public static TierUnlockOverlayControl instance;

    bool isPlaying;

    [SerializeField] float radius;
    [SerializeField] float speed;
    [SerializeField] int mult;
    float angle, stepSize, radians;
    Vector2 position;

    [SerializeField] List<GameObject> imagesGO;
    [SerializeField] GameObject imagePrefab;
    [SerializeField] Transform rotatingRingTransform;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        angle = 0;
    }

    void Update()
    {
        if(isPlaying)
        {
            MoveInCircle();
            //MoveInFigureEight();
        }
    }

    void MoveInCircle()
    {
        for (int i = 0; i < imagesGO.Count; i++)
        {
            angle = (360 / imagesGO.Count) * i + Time.time * speed;
            angle *= Mathf.Deg2Rad;

            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle * mult) * radius;

            position = new Vector2(x, y);
            imagesGO[i].GetComponent<RectTransform>().localPosition = position;
        }
    }

    void MoveInFigureEight()
    {
        // Calculate the new position for each image in a figure-8 pattern
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    Transform child = transform.GetChild(i);
        //    float angle = i * 360f / transform.childCount; // Spread images evenly
        //    float radians = Mathf.Deg2Rad * angle;

        //    float x = width * Mathf.Sin(radians);     // Use sine for x-coordinate
        //    float y = height * Mathf.Sin(2 * radians); // Use a multiple of sine for y-coordinate

        //    Vector3 newPosition = new Vector3(x, y, 0);
        //    child.position = newPosition;
        //}

        // Rotate the entire container to create a continuous movement
        //transform.Rotate(Vector3.forward, speed * Time.deltaTime);

        for (int i = 0; i < imagesGO.Count; i++)
        {
            angle = i * 360f / imagesGO.Count;
            radians = Mathf.Deg2Rad * angle;

            float width = radius;
            float height = radius * 2;

            float x = width * Mathf.Sin(radians);
            float y = height * Mathf.Sin(2 * radians);

            position = new Vector2(x, y);
            imagesGO[i].GetComponent<RectTransform>().localPosition = position;

            transform.Rotate(Vector3.forward, speed * Time.deltaTime);
        }
    }

    public void SpawnImages(byte amount, Sprite[] sprites)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newImage = Instantiate(imagePrefab, rotatingRingTransform);

            newImage.GetComponent<Image>().sprite = sprites[i];

            imagesGO.Insert(i, newImage);
        }
    }

    public void DeleteImages()
    {
        for (int i = 0; i < imagesGO.Count; i++)
        {
            Destroy(imagesGO[i]);
        }
        imagesGO.Clear();
    }

    public void SetIsPlaying(bool newBool)
    {
        isPlaying = newBool;
    }
}