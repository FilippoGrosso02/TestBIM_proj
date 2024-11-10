using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Include if using TextMeshPro

public class TagManager : MonoBehaviour
{
    public GameObject lastPlacedPrefab; // Reference to the last placed prefab

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method to set the reference to the last placed prefab
    public void SetLastPlacedPrefab(GameObject prefab)
    {
        lastPlacedPrefab = prefab;
        Debug.Log("Stored reference to the last placed prefab.");
    }

    // Function to place a new prefab, store it, and set it as the last placed one

    // Function to update the text of the last placed prefab
    public void UpdateTagText(string newText)
    {
        if (lastPlacedPrefab == null)
        {
            Debug.LogWarning("No prefab has been placed yet.");
            return;
        }

        // Try to find a TextMeshPro or Text component on the last placed prefab
        TMP_Text textComponent = lastPlacedPrefab.GetComponentInChildren<TMP_Text>(); // For TextMeshPro
        if (textComponent != null)
        {
            textComponent.text = newText;
            Debug.Log("Tag text updated to: " + newText);
        }
        else
        {
            Debug.LogWarning("No Text component found on the last placed prefab.");
        }
    }
}
