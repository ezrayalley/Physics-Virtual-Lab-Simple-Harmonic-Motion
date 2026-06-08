using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Text;

public class AIFeedback : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text responseText;

    [Header("Experiment Data")]
    public float gValue;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;

    [Header("Gemini API")]
    [SerializeField]
    private string apiKey = "AQ.Ab8RN6KplX-TGHziFiHFqhY4bOJU7LTOl2o2TsI7Es77qu29mg";

    private string apiURL =>
        $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

    private bool isRequestRunning = false;
    private string aiResponseText = "";

    // =====================================================
    // BUTTON
    // =====================================================
    public void GetFeedback()
    {
        if (isRequestRunning)
            return;

        StopAllCoroutines();
        StartCoroutine(GenerateFeedback());
    }

    // =====================================================
    // MAIN FEEDBACK
    // =====================================================
    IEnumerator GenerateFeedback()
    {
        isRequestRunning = true;

        responseText.text = "Thinking...";

        string prompt =
$@"You are an AI Physics Tutor.

A student has performed a VIRTUAL SIMULATION experiment involving a simple pendulum where values for length and number of oscillations inputted,pendulum bob is displaced and time recorded.

The value of gravitational acceleration obtained from the simulation is:

g = {gValue:F2} m/s²

The accepted value is 9.81 m/s².

Provide feedback on:
1. The accuracy of the result.
2. Possible reasons for differences from the accepted value considering angle of displacement, length and number of oscillations.
3. Suggestions for improving the experiment.

Keep the explanation concise, friendly and suitable for undergraduate students.
Response should not exceed 8 lines. Avoid markdown symbols.";

        yield return StartCoroutine(SendGeminiRequest(prompt));

        if (!string.IsNullOrEmpty(aiResponseText))
        {
            yield return StartCoroutine(TypeText(aiResponseText));
        }
        else
        {
            string fallback = GetFallbackResponse();
            yield return StartCoroutine(TypeText(fallback));
        }

        isRequestRunning = false;
    }

    // =====================================================
    // GEMINI REQUEST
    // =====================================================
    IEnumerator SendGeminiRequest(string prompt)
    {
        aiResponseText = "";

        GeminiRequest requestData = new GeminiRequest
        {
            contents = new Content[]
            {
                new Content
                {
                    parts = new Part[]
                    {
                        new Part
                        {
                            text = prompt
                        }
                    }
                }
            }
        };

        string json = JsonUtility.ToJson(requestData);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(apiURL, "POST"))
        {
            UploadHandlerRaw uploadHandler = new UploadHandlerRaw(bodyRaw);
            DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();

            request.uploadHandler = uploadHandler;
            request.downloadHandler = downloadHandler;

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("SUCCESS");
                Debug.Log(request.downloadHandler.text);

                aiResponseText = ParseGeminiResponse(request.downloadHandler.text);

                Debug.Log("Parsed response:");
                Debug.Log(aiResponseText);
            }
            else
            {
                Debug.Log("REQUEST FAILED");
                Debug.Log(request.error);
                Debug.Log("Response code: " + request.responseCode);
                Debug.Log(request.downloadHandler.text);
            }

            uploadHandler.Dispose();
            downloadHandler.Dispose();
        }
    }

    // =====================================================
    // RESPONSE PARSER
    // =====================================================
    string ParseGeminiResponse(string json)
    {
        try
        {
            GeminiResponse response =
                JsonUtility.FromJson<GeminiResponse>(json);

            if (response != null &&
                response.candidates != null &&
                response.candidates.Length > 0 &&
                response.candidates[0].content.parts.Length > 0)
            {
                Debug.Log("Candidate found!");
                Debug.Log(response.candidates[0].content.parts[0].text);
                return response.candidates[0].content.parts[0].text;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Parser error: " + e.Message);
        }

        return "";
    }

    // =====================================================
    // FALLBACK SYSTEM
    // =====================================================
    string GetFallbackResponse()
    {
        if (float.IsNaN(gValue) || float.IsInfinity(gValue))
        {
            return "The calculated value of g is invalid. Please verify the graph and repeat the simulation.";
        }

        if (gValue > 12f)
        {
            return $"Your calculated value of g = {gValue:F2} m/s² is considerably higher than the accepted value. This may be due to large dispacement angles, wrong timings or parameter settings. Check for consistent values of length and ensure a fixed number of oscillations is used thorughout the experiment.";
        }

        if (gValue > 11f)
        {
            return $"The value of g obtained ({gValue:F2} m/s²) is above the expected range. Excessive oscillation amplitude or inconsistent simulation settings may have affected the result. Consider checking the length and time whiles ensuring that the pendulum oscillate with small angle.";
        }

        if (gValue < 8.5f)
        {
            return $"The value of g obtained ({gValue:F2} m/s²) is below the expected range. Errors in displacement, timing or parameter selection may have influenced the result.Repeating the experiment with more controlled oscillations may improve accuracy";
        }

        return $"Your calculated value of g = {gValue:F2} m/s² is close to the accepted value of 9.81 m/s², indicating careful timing and reliable experimental accuracy.";
    }

    // =====================================================
    // TYPEWRITER EFFECT
    // =====================================================
    IEnumerator TypeText(string text)
    {
        responseText.text = "";

        foreach (char c in text)
        {
            responseText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // =====================================================
    // JSON CLASSES
    // =====================================================

    [System.Serializable]
    public class GeminiRequest
    {
        public Content[] contents;
    }

    [System.Serializable]
    public class Content
    {
        public Part[] parts;
    }

    [System.Serializable]
    public class Part
    {
        public string text;
    }

    [System.Serializable]
    public class GeminiResponse
    {
        public Candidate[] candidates;
    }

    [System.Serializable]
    public class Candidate
    {
        public ResponseContent content;
    }

    [System.Serializable]
    public class ResponseContent
    {
        public Part[] parts;
    }
}