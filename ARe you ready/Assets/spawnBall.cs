using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class spawnBall : MonoBehaviour
{
    [SerializeField]
    GameObject ball;

    [SerializeField]
    public Text debugLog;

    private void Awake() {
        PlacingAndDragging.spawnable = false;
        swipeBall.rollable = false;
    }
    public void Spawn() //UI ball
    {
        SoundManager.instance.PlaySfx("UI_Press");
        //debugLog.text = "Ready to spawn ball"; 
        PlacingAndDragging.secondTimerCheck = false;
        PlacingAndDragging.spawnable = true;

        //Instantiate(ball, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity);
    }
    
    public void roll()
    {
        SoundManager.instance.PlaySfx("UI_Press");
        //debugLog.text = "Ready to roll ball";
        PlacingAndDragging.spawnable = false;
        swipeBall.rollable = true;
    }

    public void restart()
    {
        SoundManager.instance.PlaySfx("UI_Press");

        if (SceneManager.GetActiveScene().name == "ObjectTracking")
        {
            SceneManager.LoadScene("ObjectTracking");
        }
        else
        {
            SceneManager.LoadScene("RollBall");
        }
    }
}
