using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using TMPro; // Import TextMeshPro namespace
using System;

public class WebcamCapture : MonoBehaviour
{
    [Header("UI Elements")]
    public RawImage rawImage; // Live camera feed display
    public RawImage previewImage; // Preview captured image
    public GameObject confirmationDialog; // Dialog box for confirmation
    public Button saveButton; // Save/Send button
    public Button cancelButton; // Cancel button
    public Button exportButton; // Export data button
    public TMP_InputField resultInput; // Result TMP_InputField (editable)

    [Header("Python Server URL")]
    public string pythonServerUrl = "https://junction-equipment-scanner-1013092067188.europe-north1.run.app/image";

    [Header("Image Settings")]
    public int maxDimension = 1500; // Max width/height for the image

    private WebCamTexture webcamTexture;
    private Texture2D capturedImage;
    private string savedFilePath;

    // Session data storage
    private List<SessionData> sessionDataList = new List<SessionData>();

    public ImageSender imageSender;

    void Start()
    {
        // Initialize the webcam
        StartWebcam();

        // Add listeners for buttons
        saveButton.onClick.AddListener(SaveAndSendImage);
        cancelButton.onClick.AddListener(CancelImage);
        exportButton.onClick.AddListener(ExportSessionData);
    }

    void StartWebcam(int cameraIndex = 1)
    {
        // Stop the current webcam if it’s already running
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }

        // Get the list of available devices (cameras)
        WebCamDevice[] devices = WebCamTexture.devices;

        // Check if the specified camera index is within the available range
        if (cameraIndex < 0 || cameraIndex >= devices.Length)
        {
            Debug.LogError("Invalid camera index. Please provide a valid camera index.");
            return;
        }

        // Initialize the webcam with the specified camera device
        webcamTexture = new WebCamTexture(devices[cameraIndex].name);
        rawImage.texture = webcamTexture;
        rawImage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
        confirmationDialog.SetActive(false);
        Debug.Log($"Webcam started using camera: {devices[cameraIndex].name}");
    }

    public void CaptureAndPreviewImage()
    {
        if (webcamTexture.isPlaying)
        {
            //webcamTexture.Stop();
        }

        if (webcamTexture.width <= 16 || webcamTexture.height <= 16)
        {
            Debug.LogError("Webcam texture data not available. Ensure webcam is active.");
            return;
        }

        capturedImage = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
        Debug.Log(webcamTexture.width);
        capturedImage.SetPixels(webcamTexture.GetPixels());
        if (webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
        capturedImage.Apply();

        if (capturedImage != null)
        {
            Debug.Log($"Image captured successfully: {capturedImage.width}x{capturedImage.height}");
        }
        else
        {
            Debug.LogError("Failed to capture image.");
            return;
        }

        capturedImage = ResizeImage(capturedImage, maxDimension);
        Debug.Log($"Resized Image captured successfully: {capturedImage.width}x{capturedImage.height}");
        confirmationDialog.SetActive(true);
        previewImage.texture = capturedImage;
    }

    public void SaveAndSendImage()
    {
        CaptureAndPreviewImage();
        if (capturedImage == null)
        {
            Debug.LogError("No image captured to save.");
            return;
        }

        byte[] imageBytes =capturedImage.EncodeToJPG(50);
        savedFilePath = Path.Combine(Application.persistentDataPath, "CapturedImage_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");
        File.WriteAllBytes(savedFilePath, imageBytes);

        Debug.Log($"Image saved locally at: {savedFilePath}");

        //StartCoroutine(SendImageToPythonServer(savedFilePath));
        SendImageToPythonServer();
        RestartWebcam();
    }

    void SendImageToPythonServer()
    {
        imageSender.imagePath = savedFilePath;
        imageSender.SendImage();
        /*
         if (!File.Exists(filePath))
        {
            Debug.LogError("Image file not found. Cannot send to server.");
            yield break;
        }

        byte[] imageBytes = File.ReadAllBytes(filePath);
        string base64Image = Convert.ToBase64String(imageBytes);

        // Create JSON payload
        string jsonPayload = JsonUtility.ToJson(new ImageRequest { encoding = base64Image });
        Debug.Log(jsonPayload);

        // Create the UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(pythonServerUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        Debug.Log("Raw Byte Data: " + BitConverter.ToString(bodyRaw));
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Sending image data to Python server...");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Response from server: {request.downloadHandler.text}");
            
            // Print the entire JSON response
            Debug.Log($"Full JSON Response: {request.downloadHandler.text}");

            // Populate the result TMP_InputField with the server response
            resultInput.text = request.downloadHandler.text;

            // Save session data
            sessionDataList.Add(new SessionData(savedFilePath, request.downloadHandler.text));
        }
        else
        {

            Debug.LogError($"Error sending data to Python server: {request.error}");
            Debug.LogError($"HTTP Status Code: {request.responseCode}");
            Debug.LogError($"Response: {request.downloadHandler.text}");

        } 
        */
    }

    public void CancelImage()
    {
        if (!string.IsNullOrEmpty(savedFilePath) && File.Exists(savedFilePath))
        {
            File.Delete(savedFilePath);
            Debug.Log("Unsaved image deleted.");
        }

        RestartWebcam();
    }

    void RestartWebcam()
    {
        StartWebcam();
    }

    Texture2D ResizeImage(Texture2D original, int maxDimension)
    {
        int width = original.width;
        int height = original.height;

        if (width > maxDimension || height > maxDimension)
        {
            float scale = (float)maxDimension / Mathf.Max(width, height);
            width = Mathf.RoundToInt(width * scale);
            height = Mathf.RoundToInt(height * scale);
        }

        // Create a RenderTexture with the target width and height
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        RenderTexture.active = renderTexture;

        // Copy the original texture to the RenderTexture
        Graphics.Blit(original, renderTexture);

        // Create a new Texture2D with the desired size and read the RenderTexture data
        Texture2D resized = new Texture2D(width, height, TextureFormat.RGB24, false);
        resized.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resized.Apply();

        // Clean up
        RenderTexture.active = null;
        renderTexture.Release();

        return resized;
    }


    public void ExportSessionData()
    {
        Debug.Log("Exporting session data...");
        foreach (var data in sessionDataList)
        {
            Debug.Log($"Image Path: {data.ImagePath}, Server Response: {data.ServerResponse}");
        }
    }

    [Serializable]
    public class ImageRequest
    {
        public string encoding;
    }

    [Serializable]
    public class SessionData
    {
        public string ImagePath;
        public string ServerResponse;

        public SessionData(string imagePath, string serverResponse)
        {
            ImagePath = imagePath;
            ServerResponse = serverResponse;
        }
    }
}
