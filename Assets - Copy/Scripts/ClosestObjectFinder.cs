using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestObjectFinder : MonoBehaviour
{
    public Transform parentObject; // Parent object containing children to search through
    public Vector3 targetPosition; // Position to compare against

    void Start()
    {
        //FindObject();
    }

    public void FindObject()
    {
        targetPosition = transform.position;
        GameObject closestObject = FindClosestChild(targetPosition, parentObject);

        if (closestObject != null)
        {
            Debug.Log("Closest object: " + closestObject.name);
        }
        else
        {
            Debug.Log("No child objects found.");
        }
    }

    GameObject FindClosestChild(Vector3 targetPos, Transform parent)
    {
        GameObject closestObject = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform child in parent)
        {
            Vector3 center = child.GetComponent<Renderer>().bounds.center; // Get child's center position
            float distance = Vector3.Distance(targetPos, center);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestObject = child.gameObject;
            }
        }

        return closestObject;
    }
}
