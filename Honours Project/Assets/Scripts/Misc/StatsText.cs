using UnityEngine;
using UnityEngine.UI;

public class StatsText : MonoBehaviour
{
    [HideInInspector] public int Episode = 0;
    [HideInInspector] public int Success = 0;
    [HideInInspector] public int Fail = 0;
    [HideInInspector] public float Reward = 0; 
    [SerializeField] private Text txtStats = null;

    // Update is called once per frame
    void Update()
    {
        // Updates statistic overlay
        txtStats.text = string.Format("Episode={0}, Success={1}, Fail={2}, Reward={3}", 
            Episode, 
            Success, 
            Fail,
            Reward.ToString("F2"));
    }
}
