using UnityEngine;
using UnityEngine.UI;

public class PageSwitcher : MonoBehaviour
{
    public GameObject[] pages; // Array to hold references to all canvas pages.
    private int currentPageIndex = 0; // Current page index.

    // Start is called before the first frame update
    void Start()
    {
        // Disable all pages except the first one (assuming index 0 is the first page).
        for (int i = 1; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
    }

    // Function to switch to the next page.
    public void NextPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {
            ChangePage(currentPageIndex + 1);
        }
    }

    // Function to switch to the previous page.
    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            ChangePage(currentPageIndex - 1);
        }
    }

    // Helper function to enable/disable pages and update the current page index.
    public void ChangePage(int newIndex)
    {
        // Disable the current page.
        pages[currentPageIndex].SetActive(false);

        // Update the current page index.
        currentPageIndex = newIndex;

        // Enable the new current page.
        pages[currentPageIndex].SetActive(true);
    }

    public void GoToLastPage()
    {
        // Disable the current page.
        pages[currentPageIndex].SetActive(false);

        // Update the current index to the last value
        currentPageIndex = pages.Length-1;

        // Enable the new current page.
        pages[currentPageIndex].SetActive(true);
    }
}
