using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FileUploader : MonoBehaviour
{
    [Header("Configuration")]
    public string serverUrl = "https://junction-equipment-scanner-1013092067188.europe-north1.run.app/reposition";
    public string filePath = "Scripts/dic.json";

    [Header("UI Elements")]
    public Button uploadButton; 

    private void Start()
    {
        // Hook up the button click event
        if (uploadButton != null)
        {
            uploadButton.onClick.AddListener(OnUploadButtonClicked);
        }
    }

    private void OnUploadButtonClicked()
    {
        // Get the full path to the file in the Unity project
        string fullPath = Application.dataPath + "/" + filePath;
        if (System.IO.File.Exists(fullPath))
        {
            StartCoroutine(UploadFile(fullPath));
        }
        else
        {
            Debug.LogError("File not found at path: " + fullPath);
        }
    }

    private IEnumerator UploadFile(string fullPath)
    {
        // Read the file bytes
        byte[] fileData = System.IO.File.ReadAllBytes(fullPath);
        if (fileData == null || fileData.Length == 0)
        {
            Debug.LogError("File is empty or unreadable: " + fullPath);
            yield break;
        }

        // Create a UnityWebRequest for the file upload
        UnityWebRequest request = new UnityWebRequest(serverUrl, UnityWebRequest.kHttpVerbPOST);
        UploadHandler uploadHandler = new UploadHandlerRaw(fileData);
        uploadHandler.contentType = "application/json"; // Assuming JSON file
        request.uploadHandler = uploadHandler;

        // Optionally set a download handler to get a response
        request.downloadHandler = new DownloadHandlerBuffer();

        Debug.Log("Uploading file to: " + serverUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("File uploaded successfully!");
            Debug.Log("Server response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Failed to upload file. Error: " + request.error);
        }
    }
}