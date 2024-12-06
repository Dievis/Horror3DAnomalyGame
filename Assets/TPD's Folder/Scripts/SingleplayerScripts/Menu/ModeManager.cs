using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ModeManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public void PlayModeDe()
    {
        SceneManager.LoadScene(1);
    }
    public void PlayModeTrungBinh()
    {
        SceneManager.LoadScene(2);
    }
    public void PlayModeKho()
    {
        SceneManager.LoadScene(3);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
