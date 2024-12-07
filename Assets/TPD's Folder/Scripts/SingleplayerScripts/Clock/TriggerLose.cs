using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLose : MonoBehaviour
{
    public CutScreenEnd csE;
    public bool _hasLose;

    private void Start()
    {
        _hasLose = false;
    }
    void Update()
    {
        if (_hasLose == true)
        {
            // Xử lý khi người chơi thắng game
            // ...

            // Kích hoạt hiệu ứng nhiễu màn hình và hiển thị menu end game
            csE.ShowCutScreen();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hour")
        {
            Debug.Log("Chuc mung ban da thua! Ga vcl!");
            _hasLose = true;
        }
    }
}