using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro; // For TextMeshPro Input Fields
using Newtonsoft.Json;
using System; // For Random string generation
using UnityEngine.Networking;
using System.Collections;

public class JSONParser : MonoBehaviour
{
    [Header("JSON File Settings")]
    public string jsonFilePath = "Assets/dic.json"; // Path to JSON input file

    [Header("Input Fields")]
    public TMP_InputField assessmentDateInput;
    public TMP_InputField certificationsInput;
    public TMP_InputField deviceTypeInput;
    public TMP_InputField installationDateInput;
    public TMP_InputField locationInput;
    public TMP_InputField manufacturerInput;
    public TMP_InputField modelInput;
    public TMP_InputField modelNumberInput;
    public TMP_InputField nextAssessmentDateInput;
    public TMP_InputField productionYearInput;
    public TMP_InputField serialNumberInput;
    public TMP_InputField additionalInfoInput;

    [Header("Server Settings")]
    public string serverUrl = "https://junction-equipment-scanner-1013092067188.europe-north1.run.app/add"; // Change to your server address

    public string jsonFile;

    public TagManager tagManager;

    [System.Serializable]
    public class DataModel
    {
        public string Id; // New field for random ID
        public Dictionary<string, string> AdditionalInfo;
        public string AssessmentDate;
        public string Certifications;
        public string DeviceType;
        public string InstallationDate;
        public string Location;
        public string Manufacturer;
        public string Model;
        public string ModelNumber;
        public string NextAssessmentDate;
        public string ProductionYear;
        public string SerialNumber;
    }

    private DataModel data;

    public void Parse()
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"JSON file not found at path: {jsonFilePath}");
            return;
        }

        string jsonContent = File.ReadAllText(jsonFilePath);
        jsonContent = jsonFile;
        data = JsonConvert.DeserializeObject<DataModel>(jsonContent);

        if (data != null)
        {
            Debug.Log("JSON parsed successfully!");
            AssignValuesToInputFields();
        }
        else
        {
            Debug.LogError("Failed to parse JSON!");
        }
    }

    void AssignValuesToInputFields()
    {
        if (data.AdditionalInfo != null)
        {
            additionalInfoInput.text = ParseAdditionalInfo(data.AdditionalInfo);
        }

        assessmentDateInput.text = data.AssessmentDate;
        certificationsInput.text = data.Certifications;
        deviceTypeInput.text = data.DeviceType;
        installationDateInput.text = data.InstallationDate;
        locationInput.text = data.Location;
        manufacturerInput.text = data.Manufacturer;
        modelInput.text = data.Model;
        modelNumberInput.text = data.ModelNumber;
        nextAssessmentDateInput.text = data.NextAssessmentDate;
        productionYearInput.text = data.ProductionYear;
        serialNumberInput.text = data.SerialNumber;

        Debug.Log("Assigned JSON values to input fields!");
    }

    string ParseAdditionalInfo(Dictionary<string, string> additionalInfo)
    {
        string parsedInfo = "";

        foreach (var keyValue in additionalInfo)
        {
            parsedInfo += $"{keyValue.Key}: {keyValue.Value}\n";
        }

        return parsedInfo.Trim();
    }

    public void ConvertFieldsToJSON()
    {
        data = new DataModel
        {
            Id = GenerateRandomId(18), // Generate random 18-character ID
            AssessmentDate = assessmentDateInput.text,
            Certifications = certificationsInput.text,
            DeviceType = deviceTypeInput.text,
            InstallationDate = installationDateInput.text,
            Location = locationInput.text,
            Manufacturer = manufacturerInput.text,
            Model = modelInput.text,
            ModelNumber = modelNumberInput.text,
            NextAssessmentDate = nextAssessmentDateInput.text,
            ProductionYear = productionYearInput.text,
            SerialNumber = serialNumberInput.text,
            AdditionalInfo = ParseAdditionalInfoFromField(additionalInfoInput.text)

        };
        //tagManager.UpdateTagText(deviceTypeInput.text);
        string jsonOutput = JsonConvert.SerializeObject(data, Formatting.Indented);
        Debug.Log("Converted data to JSON: " + jsonOutput);

        File.WriteAllText(jsonFilePath, jsonOutput);
        Debug.Log("JSON saved to: " + jsonFilePath);
    }

    private Dictionary<string, string> ParseAdditionalInfoFromField(string additionalInfoText)
    {
        Dictionary<string, string> additionalInfo = new Dictionary<string, string>();

        string[] lines = additionalInfoText.Split('\n');
        foreach (string line in lines)
        {
            string[] keyValue = line.Split(new[] { ": " }, System.StringSplitOptions.None);
            if (keyValue.Length == 2)
            {
                additionalInfo[keyValue[0].Trim()] = keyValue[1].Trim();
            }
        }

        return additionalInfo;
    }

    private string GenerateRandomId(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] id = new char[length];
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            id[i] = chars[random.Next(chars.Length)];
        }

        return new string(id);
    }

    // Method to send JSON data to the server
    public void PostToServer()
    {
        ConvertFieldsToJSON(); // First, ensure JSON data is updated

        StartCoroutine(PostJSONCoroutine());
    }

    private IEnumerator PostJSONCoroutine()
    {
        Debug.Log("Coroutine Started");
        string jsonOutput = JsonConvert.SerializeObject(data, Formatting.Indented);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonOutput);

        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Sending JSON data to server...");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Server response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error sending JSON to server: " + request.error);
        }
    }
}
