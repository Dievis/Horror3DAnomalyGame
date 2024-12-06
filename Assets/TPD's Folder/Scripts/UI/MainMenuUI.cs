using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button SingleplayerBtn;
    [SerializeField] private Button MultiplayerBtn;
    [SerializeField] private Button ExitBtn;


    private void Awake()
    {
        MultiplayerBtn.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.ConnectToServer);
        });
        SingleplayerBtn.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.SingleplayerScene);
        });
        ExitBtn.onClick.AddListener(() => {
            Application.Quit();
        });

        Time.timeScale = 1f;
    }
}
