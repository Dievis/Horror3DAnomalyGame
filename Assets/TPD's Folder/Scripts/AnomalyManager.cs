using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnomalyManager : MonoBehaviour
{
    public Image Image; // Hình ảnh biểu thị anomaly

    void Start()
    {
        // Tắt UI ban đầu
        if (Image != null) Image.enabled = false;
    }

    // Gọi khi người chơi nhìn vào anomaly
    public void OnLookAt()
    {
        if (Image != null) Image.enabled = true;
    }

    // Gọi khi người chơi rời khỏi anomaly
    public void OnLookAway()
    {
        if (Image != null) Image.enabled = false;
    }
}