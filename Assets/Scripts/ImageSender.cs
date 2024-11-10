using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System; // Add this for Convert and other system utilities

public class ImageSender : MonoBehaviour
{
    [Header("Server Settings")]
    public string pythonServerUrl = "https://junction-equipment-scanner-1013092067188.europe-north1.run.app/image"; // Server URL

    [Header("Image Settings")]
    public string imagePath = "Assets/photo_5868219542172451590_y.jpg";

    [Header("Debug Settings")]
    public bool sendOnStart = false; // If true, sends the image automatically when the script starts

    public JSONParser jsonParser;

    private void Start()
    {
        if (sendOnStart)
        {
            SendImage();
        }
    }

    public void SendImage()
    {
        if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
        {
            Debug.LogError("Invalid image path. Please set a valid file path in the Unity Editor.");
            return;
        }

        StartCoroutine(SendImageToServer(imagePath));
    }

    private IEnumerator SendImageToServer(string filePath)
    {
        //jsonParser.Parse();
        yield return null;

        // Read image data
        byte[] imageBytes = File.ReadAllBytes(filePath);
        string base64Image = Convert.ToBase64String(imageBytes);

        // Create JSON payload
        string jsonPayload = JsonUtility.ToJson(new ImageRequest { encoding = base64Image });
        Debug.Log("JSON Payload: " + jsonPayload);

        // Create UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(pythonServerUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Sending image data to the server...");
        yield return request.SendWebRequest();

        

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Response from server: {request.downloadHandler.text}");
            HandleServerResponse(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Error sending data to server: {request.error}");
        }

    }
    private void HandleServerResponse(string jsonResponse)
    {
        // Define the path to save the file
        string filePath = Path.Combine(Application.persistentDataPath, "ServerResponse.json");

        try
        {
            // Save the JSON response to the file
            File.WriteAllText(filePath, jsonResponse);
            Debug.Log("Server response saved to " + filePath);

            jsonParser.jsonFile = jsonResponse;

            jsonParser.Parse();
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save JSON response: " + ex.Message);
        }
    }


    [System.Serializable]
    private class ImageRequest
    {
        public string encoding;
    }

    // Example structure for deserialization (adjust according to your JSON response)
    [System.Serializable]
    private class ResponseData
    {
        public string someField;
        // Add other fields here as per the JSON structure
    }
}
