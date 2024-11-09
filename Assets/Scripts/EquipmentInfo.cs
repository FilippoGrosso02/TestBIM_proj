using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInfo : MonoBehaviour
{
    // This method returns the local position of the GameObject as a Vector3
    public Vector3 GetLocalPosition()
    {
        return transform.localPosition;
    }

    // Example of calling GetLocalPosition in Update
    void Start()
    {
        // Print local position to the console every frame
        Vector3 localPos = GetLocalPosition();
        Debug.Log("Local Position: " + localPos);
    }
}