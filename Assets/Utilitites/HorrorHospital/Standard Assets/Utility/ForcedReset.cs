using System;
using UnityEngine;
using UnityEngine.SceneManagement; // Để load scene
using UnityEngine.UI; // Import UI hệ mới

public class ForcedReset : MonoBehaviour
{
    private void Update()
    {
        // Nếu nút reset được nhấn
        if (Input.GetButtonDown("ResetObject"))
        {
            // Load lại scene hiện tại
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }
}
