using System.Collections;
using UnityEngine;
using TMPro;

public class AITutor : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text responseText;

    [Header("Experiment Data")]
    public float gValue;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    // =====================================================
    // PUBLIC BUTTON FUNCTION
    // =====================================================
    public void GetFeedback()
    {
        StopAllCoroutines();
        StartCoroutine(GenerateFeedback());
    }

    //Paste purchased API key and request here
    // =====================================================
    // GENERATE LOCAL AI-LIKE RESPONSE
    // =====================================================
    IEnumerator GenerateFeedback()
    {
        responseText.text = "Thinking...";

        // Small delay for realism
        yield return new WaitForSeconds(2f);

        string feedback = GetFallbackResponse();

        yield return StartCoroutine(TypeText(feedback));
    }

    // =====================================================
    // FALLBACK RESPONSE FUNCTION
    // =====================================================
    string GetFallbackResponse()
    {
        string fallback = "";
        int variant = Random.Range(0, 3);

        // ----------------------------
        // INVALID VALUE
        // ----------------------------
        if (float.IsNaN(gValue) || float.IsInfinity(gValue))
        {
            if (variant == 0)
            {
                fallback = "The calculated value of g is invalid. This may be due to incomplete data, or incorrect graph calculations. Please repeat the experiment carefully.";
            }
            else if (variant == 1)
            {
                fallback = "Unable to determine a valid value for gravitational acceleration. Ensure that all measurements and calculations were entered correctly before plotting the graph.";
            }
            else
            {
                fallback = "The experiment produced an undefined result for g. Check your full experiment data, timing values,and ensure the graph was plotted correctly.";
            }
        }

        // ----------------------------
        // VERY HIGH VALUE
        // ----------------------------
        else if (gValue > 12f)
        {
            if (variant == 0)
            {
                fallback = $"Your calculated value of g = {gValue:F2} m/s² is significantly higher than the accepted value of 9.8 m/s². This may indicate large timing errors, excessive oscillation amplitude, or incorrect graph interpretation.";
            }
            else if (variant == 1)
            {
                fallback = $"The value obtained for gravitational acceleration appears unusually high. Consider checking the time measurements and ensuring the pendulum oscillated with a small angle.";
            }
            else
            {
                fallback = $"Your result exceeds the standard value of g by a considerable margin. Repeating the experiment with more controlled oscillations may improve accuracy.";
            }
        }

        // ----------------------------
        // HIGH VALUE
        // ----------------------------
        else if (gValue > 11f)
        {
            if (variant == 0)
            {
                fallback = $"Your value of g = {gValue:F2} m/s² is slightly higher than the accepted value of 9.8 m/s². This suggests possible timing inaccuracies or large oscillation angles.";
            }
            else if (variant == 1)
            {
                fallback = $"The value of g obtained ({gValue:F2} m/s²) is above the expected range. Human reaction time or inconsistent oscillations may have affected the result.";
            }
            else
            {
                fallback = $"Your result of g = {gValue:F2} m/s² is higher than expected. Ensure that the pendulum swings with a small amplitude and timing measurements are precise.";
            }
        }

        // ----------------------------
        // LOW VALUE
        // ----------------------------
        else if (gValue < 8.5f)
        {
            if (variant == 0)
            {
                fallback = $"Your calculated value of g = {gValue:F2} m/s² is lower than the expected 9.8 m/s². This may indicate errors in measuring pendulum length or timing.";
            }
            else if (variant == 1)
            {
                fallback = $"The value obtained ({gValue:F2} m/s²) falls below the accepted range. Consider verifying the measured length and repeating the oscillation timing.";
            }
            else
            {
                fallback = $"Your result is lower than expected. Ensure the pendulum length was measured from the pivot to the center of the bob and that timing was done carefully.";
            }
        }

        // ----------------------------
        // ACCEPTABLE RANGE
        // ----------------------------
        else
        {
            if (variant == 0)
            {
                fallback = $"Your calculated value of g = {gValue:F2} m/s² is close to the accepted value of 9.8 m/s², indicating good experimental accuracy.";
            }
            else if (variant == 1)
            {
                fallback = $"The result obtained ({gValue:F2} m/s²) lies within an acceptable experimental range. This suggests careful timing and reliable measurements.";
            }
            else
            {
                fallback = $"Your value of g = {gValue:F2} m/s² agrees reasonably well with the standard gravitational acceleration. The experiment appears to have been conducted accurately.";
            }
        }

        return fallback;
    }

    // =====================================================
    // SHOW SHM GUIDANCE
    // =====================================================
    public void ShowSHMGuidance(string experimentName)
    {
        string msg = "";
        int variant = Random.Range(0, 3);

        if (variant == 0)
        {
            msg = "Before proceeding, it is advisable to be familiar with the simple pendulum. Mastering its principles provides a strong foundation for understanding other SHM systems such as the helical spring and cantilever.";
        }
        else if (variant == 1)
        {
            msg = "You are encouraged to first study the simple pendulum. It clearly demonstrates the core principles of simple harmonic motion needed for understanding this experiment.";
        }
        else
        {
            msg = "It is recommended to begin with the simple pendulum. A solid understanding of its motion will make this experiment easier to interpret.";
        }

        StopAllCoroutines();
        StartCoroutine(TypeText(msg));
    }

    // =====================================================
    // TYPEWRITER EFFECT
    // =====================================================
    IEnumerator TypeText(string text)
    {
        responseText.text = "";

        foreach (char letter in text)
        {
            responseText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}