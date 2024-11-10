using UnityEngine;

public class CountChildren : MonoBehaviour
{
    void Start()
    {
        int childCount = transform.childCount;
        Debug.Log("Number of children: " + childCount);
    }
}