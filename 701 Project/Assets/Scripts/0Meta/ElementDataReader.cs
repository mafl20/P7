using UnityEngine;
using System.IO;

public class ElementDataReader : MonoBehaviour
{
    public static ElementDataReader instance;
    public string[] elements;
    public bool isBuild;

    private void Awake()
    {
        instance = this;

        if (isBuild == true) { ReadDataInBuild(); }
        else                 { ReadDataInEditor(); }
    }

    void ReadDataInEditor()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "../Data/elements_data_DK.txt");

        if (File.Exists(filePath))
        {
            elements = File.ReadAllLines(filePath);
            
            /*
            foreach (string element in elements)
            {
                string[] data = element.Split(','); // Now, data[0] contains the AtomicNumber, data[1] contains the Symbol, and so on...
            }*/

            //int randomNumber = Random.Range(1, elements.Length);
            //Debug.Log(elements[randomNumber]);
        }
        else { Debug.LogError("Could not find the element data file."); }
    }

    void ReadDataInBuild()
    {
        elements = File.ReadAllLines(Application.dataPath + "/Resources/elements_data_DK.txt");
    }

    public string[] GetElementPropertiesFromProtons(int index)
    {
        string[] data = elements[index].Split(',');
        return data;
    }

    public string[] GetElementPropertiesFromChemicalName(string chemicalName)
    {
        int index = SearchForWordInFile(chemicalName);
        //Debug.Log("element with chemical name " + chemicalName + " has index " + index);
        return GetElementPropertiesFromProtons(index);
    }

    public string GetChemicalNameFromNumberOfProtons(int numberOfProtons)
    {
        return GetElementPropertiesFromProtons(numberOfProtons)[1];
    }

    byte SearchForWordInFile(string chemicalName)
    {
        byte i = 0;
        string[] lines = elements;

        foreach (string line in lines)
        {
            if(line.Contains(chemicalName))
            {
                //Debug.Log("Searching for element with chemical name " + chemicalName);
                return i;
            }

            i++;
        }

        return 0;
    }
}