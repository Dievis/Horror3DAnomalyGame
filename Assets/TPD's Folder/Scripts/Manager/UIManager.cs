using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject matchInfoPanel;
    [SerializeField] private TMP_Text anomalyCountText;
    [SerializeField] private TMP_Text timerText;

    public void UpdateAnomalyCountUI(int anomaliesFound, int totalAnomalies)
    {
        anomalyCountText.text = $"Anomaly: {anomaliesFound}/{totalAnomalies}";
    }

    public void UpdateTimerUI(float timer)
    {
        timerText.text = $"Time: {Mathf.CeilToInt(timer)}s";
    }

    public void ShowEndGameUI(bool victory)
    {
        if (victory)
        {
            winPanel.SetActive(true);
        }
        else
        {
            losePanel.SetActive(true);
        }
    }

    public void ToggleMatchInfoPanel()
    {
        matchInfoPanel.SetActive(!matchInfoPanel.activeSelf);
    }
}
