using UnityEngine;
using TMPro;

public class ResultsManager : MonoBehaviour
{
    public Transform tableContent;
    public GameObject rowPrefab;

    void Start()
    {
        BuildTable();
    }

    void BuildTable()
    {
        for (int i = 0; i < ExperimentData.lengths.Count; i++)
        {
            GameObject row = Instantiate(rowPrefab, tableContent);

            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();

            float L = ExperimentData.lengths[i];
            float t = ExperimentData.times[i];
            int n = ExperimentData.oscillations[i];

            float T = t / n;
            float T2 = T * T;

            texts[0].text = (i + 1).ToString();              // Trial
            texts[1].text = (L * 100f).ToString("F1");       // cm
            texts[2].text = t.ToString("F2");                // time
            texts[3].text = T.ToString("F2");                // period
            texts[4].text = T2.ToString("F2");               // T²
        }
    }
}