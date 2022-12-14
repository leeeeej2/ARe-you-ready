using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManageScene : MonoBehaviour
{
    float time = 0f;
    static bool digipenLogo = false;

    [SerializeField]
    public Image digipen;

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start() {
        
    }
    private void Update() {
        if(!digipenLogo)
        {
            time += Time.deltaTime;
            if(time > 2f)
            {
                digipenLogo = true;
            }
        }
        else{
            digipen.gameObject.SetActive(false);
        }
    }

    public void TechDemo()
    {
        SceneManager.LoadScene("SampleScene");
        SoundManager.instance.PlaySfx("UI_Press");
    }

    public void BowlingDemo()
    {
        SceneManager.LoadScene("RollBall");
        SoundManager.instance.PlaySfx("UI_Press");
    }

    public void mainMenu()
    {
        SoundManager.instance.PlaySfx("UI_Press");
        SceneManager.LoadScene("Menu");
    }

    public void Credit()
    {
        SceneManager.LoadScene("Credit");
    }
}
