using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWin : MonoBehaviour
{
    public ScreenNoiseEffect screenNoiseEffect;

    private bool hasWon = false;

    void Update()
    {
        if (hasWon == true)
        {
            // Xử lý khi người chơi thắng game
            // ...

            // Kích hoạt hiệu ứng nhiễu màn hình và hiển thị menu end game
            screenNoiseEffect.StartNoiseEffect();
            Invoke("StartFadeEffect", 1f);
            Invoke("ShowEndGameMenu", 8f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hour")
        {
            Debug.Log("Chuc mung ban da thang tro choi!");
            hasWon = true;
        }
    }

    void StartFadeEffect()
    {
        screenNoiseEffect.StartFadeEffect();
    }

    void ShowEndGameMenu()
    {
        screenNoiseEffect.ShowEndGameMenu();
    }
}
