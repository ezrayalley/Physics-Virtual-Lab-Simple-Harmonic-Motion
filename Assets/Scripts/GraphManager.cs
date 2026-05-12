using UnityEngine;
using TMPro;

public class GraphManager : MonoBehaviour
{
    public Transform graphContainer;
    public GameObject pointPrefab;
    public TMP_Text resultText;
    public AITutor aiTutor;

    public float width = 600f;
    public float height = 300f;

    void Start()
    {
        DrawGraph();
    }

    void DrawGraph()
    {
        int count = ExperimentData.lengths.Count;

        if (count == 0)
        {
            resultText.text = "No data available";
            return;
        }

        float[] xVals = new float[count];
        float[] yVals = new float[count];

        float maxX = 0f;
        float maxY = 0f;

        // 🔹 Prepare data
        for (int i = 0; i < count; i++)
        {
            float L = ExperimentData.lengths[i];
            float t = ExperimentData.times[i];
            int n = ExperimentData.oscillations[i];

            float T = t / n;
            float T2 = T * T;

            xVals[i] = L;
            yVals[i] = T2;

            if (L > maxX) maxX = L;
            if (T2 > maxY) maxY = T2;
        }

        // 🔹 Plot points
        for (int i = 0; i < count; i++)
        {
            float x = (xVals[i] / maxX) * width - width / 2f;
            float y = (yVals[i] / maxY) * height - height / 2f;

            GameObject point = Instantiate(pointPrefab, graphContainer);
            point.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }

        // 🔹 Calculate slope
        float slope = CalculateSlope(xVals, yVals);

        // 🔹 Calculate g
        float g = (4 * Mathf.PI * Mathf.PI) / slope;

        aiTutor.gValue = g;

        resultText.text = "g = " + g.ToString("F2");

        // 🔹 Display
        resultText.text =
            "Slope = " + slope.ToString("F4") +
            "\n g = " + g.ToString("F2") + " m/s²";
    }

    float CalculateSlope(float[] x, float[] y)
    {
        int n = x.Length;

        float sumX = 0, sumY = 0, sumXY = 0, sumXX = 0;

        for (int i = 0; i < n; i++)
        {
            sumX += x[i];
            sumY += y[i];
            sumXY += x[i] * y[i];
            sumXX += x[i] * x[i];
        }

        return (n * sumXY - sumX * sumY) /
               (n * sumXX - sumX * sumX);
    }
}