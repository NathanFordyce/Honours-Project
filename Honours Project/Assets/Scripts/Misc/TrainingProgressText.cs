using UnityEngine;
using UnityEngine.UI;

public static class TrainingProgressText
{
    public static int Episode = 0;
    public static int Success = 0;
    public static int Fail = 0;
    public static float Reward = 0;
    private static Text txtDebug = null;

    public static void ScreenText()
    {
        float percentage = Success / (float)(Success + Fail) * 100;     // Calculate success / fail percentage
        if (txtDebug == null) // If text object is not set
        {
            txtDebug = GameObject.Find("DebugText").gameObject.GetComponent<Text>();    // Find and store text object
        }
        else
        {
            // Update progress overlay text
            txtDebug.text = string.Format("Episode={0}, Success={1}, Fail={2} %{3}, Reward={4}", 
                Episode, 
                Success, 
                Fail,
                percentage.ToString("0"),
                Reward.ToString("F2"));
        }
    }
}
