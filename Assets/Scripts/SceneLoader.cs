using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Load Experiments Scene
    public void LoadExperiments()
    {
        SceneManager.LoadScene("Experiments");
    }

    //Load Pendulum Scene
    public void LoadPendulum()
    {
        SceneManager.LoadScene("Pendulum");
    }
    // Load Virtaul Lab Scene
    public void LoadVirtualLab()
    {
        SceneManager.LoadScene("Virtual Lab");
    }
    // Load Results Scene
    public void LoadResults()
    {
        SceneManager.LoadScene("Results");
    }
    public void LoadGraph()
    {
        SceneManager.LoadScene("Graph");
    }
    public void LoadConcepts()
    {
        SceneManager.LoadScene("Concepts");
    }
}