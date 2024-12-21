using UnityEngine;
using TMPro;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject matchInfoPanel;
    [SerializeField] private GameObject hostLeftPanel; // Thêm hostLeftPanel
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

    [PunRPC]
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

    public void ShowHostLeftPanel()
    {
        if (hostLeftPanel != null)
        {
            hostLeftPanel.SetActive(true);
        }
    }

    public void ToggleMatchInfoPanel()
    {
        matchInfoPanel.SetActive(!matchInfoPanel.activeSelf);
    }

    

    public void ResetUI()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (matchInfoPanel != null) matchInfoPanel.SetActive(false);
        if (hostLeftPanel != null) hostLeftPanel.SetActive(false); // Reset hostLeftPanel
    }
}
