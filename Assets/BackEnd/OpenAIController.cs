using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json; // Add Newtonsoft.Json via NuGet or Unity Package Manager

public class OpenAIController : MonoBehaviour
{
    public TMP_Text textField;
    public TMP_InputField inputField;
    public Button sendButton;

    private string apiKey = "sk-proj-5JwO37hN5QFxfUzxHngkT3BlbkFJki4tveLZQ9LQZMj5vNVi"; // Replace with your API key
    private string apiEndpoint = "https://api.openai.com/v1/chat/completions";

    private List<ChatMessage> messages;
    private string systemMessageBase = "You are a virtual assistant that handles inquiries. You keep your responses short and to the point.";
    private string fullSystemMessage;
    private string additionalInfo = "";

    void Start()
    {
        // Initialize messages
        messages = new List<ChatMessage>();

        // Load additionalInfo from PlayerPrefs
        additionalInfo = PlayerPrefs.GetString("AdditionalInfo", "");

        // Initialize fullSystemMessage with the base message
        SetAdditionalInfo(additionalInfo);  // Initialize with the current additionalInfo

        sendButton.onClick.AddListener(GetResponseCoroutine);
    }

    public void UpdateSystemMessage(string additionalInfo)
    {
        if (systemMessageBase == null)
        {
            Debug.LogError("systemMessageBase is not set.");
            return;
        }

        if (messages == null)
        {
            Debug.LogError("messages is not initialized.");
            return;
        }

        Debug.Log("Updated AI Chatbot.");

        fullSystemMessage = systemMessageBase;
        if (!string.IsNullOrEmpty(additionalInfo))
        {
            fullSystemMessage += " " + additionalInfo;
        }

        if (messages.Count > 0 && messages[0].role == "system")
        {
            messages[0].content = fullSystemMessage;
        }
        else
        {
            messages.Insert(0, new ChatMessage("system", fullSystemMessage));
        }

        // Check if we are in Play mode or WebGL
        if (Application.isPlaying || Application.platform == RuntimePlatform.WebGLPlayer)
        {
            StartConversation();  // Ensure StartConversation uses the updated fullSystemMessage
        }
    }



    public void SetAdditionalInfo(string info)
    {
        additionalInfo = info;
        Debug.Log("Setting additionalInfo: " + additionalInfo);
        PlayerPrefs.SetString("AdditionalInfo", additionalInfo); // Save additionalInfo to PlayerPrefs
        UpdateSystemMessage(additionalInfo);
    }

    private void StartConversationCoroutine()
    {
        StartCoroutine(StartConversation());
    }

    private IEnumerator StartConversation()
    {
        if (textField == null || inputField == null || sendButton == null)
        {
            Debug.LogError("UI elements not assigned in the Inspector.");
            yield break;
        }

        Debug.Log(fullSystemMessage);
        messages = new List<ChatMessage> {
            new ChatMessage("system", fullSystemMessage)
        };

        inputField.text = "";
        string startString = "Welcome to our product showcase! I'm your virtual assistant here to help. Feel free to ask me anything.";
        textField.text = startString;
        Debug.Log(startString);
    }

    private void GetResponseCoroutine()
    {
        StartCoroutine(GetResponse());
    }

    private IEnumerator GetResponse()
    {
        if (inputField.text.Length < 1)
        {
            yield break;
        }

        // Disable the Send button
        sendButton.interactable = false;

        // Fill the user message from the input field
        ChatMessage userMessage = new ChatMessage
        {
            role = "user", // Use string "user"
            content = inputField.text.Length > 100 ? inputField.text.Substring(0, 100) : inputField.text
        };
        Debug.Log($"{userMessage.role}: {userMessage.content}");

        // Add the message to the list
        messages.Add(userMessage);

        // Update the text field with the user message
        textField.text = $"You: {userMessage.content}";

        // Clear the input field
        inputField.text = "";

        // Send the entire chat to OpenAI to get the next message
        var chatRequest = new ChatRequest
        {
            model = "gpt-4o", // Use a valid model name
            temperature = 0.1f,
            max_tokens = 50,
            messages = messages
        };

        // Pass userMessage to SendChatRequest
        yield return StartCoroutine(SendChatRequest(chatRequest, userMessage));
    }

    private IEnumerator SendChatRequest(ChatRequest chatRequest, ChatMessage userMessage)
    {
        string jsonData = JsonConvert.SerializeObject(chatRequest);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (var request = new UnityWebRequest(apiEndpoint, "POST"))
        {
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Body: {request.downloadHandler.text}");
            }
            else
            {
                // Parse the response and update the UI
                var result = JsonConvert.DeserializeObject<ChatResponse>(request.downloadHandler.text);
                ChatMessage responseMessage = new ChatMessage
                {
                    role = result.Choices[0].Message.role,
                    content = result.Choices[0].Message.content
                };
                Debug.Log($"{responseMessage.role}: {responseMessage.content}");

                // Add the response to the list of messages
                messages.Add(responseMessage);

                // Update the text field with the response
                textField.text = $"You: {userMessage.content}\n\nAssistant: {responseMessage.content}";
            }

            // Re-enable the Send button
            sendButton.interactable = true;
        }
    }

    [System.Serializable]
    public class ChatRequest
    {
        public string model; // Ensure field names match API requirements
        public float temperature;
        public int max_tokens;
        public List<ChatMessage> messages; // Use lowercase 'messages'
    }

    [System.Serializable]
    public class ChatResponse
    {
        public List<Choice> Choices;
    }

    [System.Serializable]
    public class Choice
    {
        public ChatMessage Message;
    }

    [System.Serializable]
    public class ChatMessage
    {
        public string role; // Use string instead of enum
        public string content;

        public ChatMessage() { }

        public ChatMessage(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }
}
