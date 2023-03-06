using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]

public class TrackedImageInfoMultipleManager : MonoBehaviour
{
    [SerializeField]
    private Text DebugLog;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private Text imageTrackedText;

    [SerializeField]
    private GameObject[] arObjectsToPlace;

    [SerializeField]
    private GameObject Youtube;

    [SerializeField]
    private GameObject Twitter;

    [SerializeField]
    private GameObject AndroidCall;

    [SerializeField]
    private GameObject Uijin;

    [SerializeField]
    private GameObject Hagyeong;

    [SerializeField]
    private Vector3 scaleFactor = new Vector3(0.01f, 1f, 0.01f);

    [SerializeField]
    private  ARTrackedImageManager m_TrackedImageManager;

    private readonly Dictionary<string, GameObject> arObjects = new();

    private readonly Dictionary<string, GameObject> arObjects4card = new();
    private readonly Dictionary<string, GameObject> arObjects4HGcard = new();

    private Vector3 newPosition;
    //bool IsCard;
    //private GameObject a;
    private Vector2 touchPosition = default;
    
    private bool touchYoutube;
    private bool touchTwitter;
    private bool touchAndroid;

    private void Awake() {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
        //Youtube.SetActive(false);
        //IsCard = false;
    }

    private void Start() {
        //a = Instantiate(Youtube, Vector3.zero, Quaternion.identity);
        touchYoutube = false;
        touchTwitter = false;
        touchAndroid = false;
    }

    private void Update() {
        
        if(touchYoutube)
        {
            DebugLog.text = "YOUTUBE TOUCHED";
            Application.OpenURL("http://www.youtube.com/channel/UCmwT0R0CAhvev4MA4Qamejw");
            touchYoutube = false;
        }

        if(touchTwitter)
        {
            DebugLog.text = "TWITTER TOUCHED";
            Application.OpenURL("http://www.instagram.com/u1zz1n/");
            touchTwitter = false;
        }

        if(touchAndroid)
        {
            DebugLog.text = "ANDROID TOUCHED";
            Application.OpenURL("tel://[+12066949766");
            touchAndroid = false;
        }

        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            touchPosition = touch.position;

            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (touch.phase == TouchPhase.Began)
            {
                DebugLog.text = "TOUCHED";

                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject))
                {
                    if(hitObject.transform.gameObject.name == "Youtube(Clone)")
                        touchYoutube = true;
                    if(hitObject.transform.gameObject.name == "Twitter(Clone)")
                        touchTwitter = true;
                    if(hitObject.transform.gameObject.name == "Android(Clone)")
                        touchAndroid = true;
                }
            }

        }
        
        // if(IsCard)
        // {
        //     DebugLog.text = "Recognized";
        //     //a.SetActive(true);
        // }
        // else{
        //     DebugLog.text = "Not Recognized";
        //     //a.SetActive(false);
        // }
    }
    void OnEnable(){
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable(){
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach(var trackedImage in eventArgs.added)
        {
            imageTrackedText.text = trackedImage.referenceImage.name;

            var name = trackedImage.referenceImage.name;
            foreach(var curPrefab in arObjectsToPlace)
            {
                if(string.Compare(curPrefab.name, name, StringComparison.Ordinal) == 0 &&
                !arObjects.ContainsKey(name))
                {
                    var newARObject = Instantiate(curPrefab, trackedImage.transform);
                    arObjects[name] = newARObject;
                    if(name == "StudentID")
                    {    
                        newPosition = new Vector3(0, 0, 0.35f);
                        //IsCard = true;
                        //other icons for business card
                        var obj = Instantiate(Youtube, trackedImage.transform);
                        arObjects4card["Youtube"] = obj;
                        arObjects4card["Youtube"].transform.position = trackedImage.transform.position + new Vector3(0.35f, 0, 0.2f);

                        obj = Instantiate(Twitter, trackedImage.transform);
                        arObjects4card["Twitter"] = obj;
                        arObjects4card["Twitter"].transform.position = trackedImage.transform.position + new Vector3(-0.3f, 0, 0.05f);

                        obj = Instantiate(AndroidCall, trackedImage.transform);
                        arObjects4card["Android"] = obj;
                        arObjects4card["Android"].transform.position = trackedImage.transform.position + new Vector3(0.3f, 0, -0.1f);

                        obj = Instantiate(Uijin, trackedImage.transform);
                        arObjects4card["Uijin"] = obj;
                        arObjects4card["Uijin"].transform.position = trackedImage.transform.position + new Vector3(0, 0.05f, 0.35f);

                        //DebugLog.text = arObjects4card["Youtube"].name + arObjects4card["Twitter"].name + arObjects4card["Android"].name;
                    }
                    else if(name == "GHStudentID")
                    {
                        newPosition = new Vector3(0, 0, -0.3f);
                        var objHG = Instantiate(Hagyeong, trackedImage.transform);
                        arObjects4HGcard["Hagyeong"] = objHG;
                        arObjects4HGcard["Hagyeong"].transform.position = trackedImage.transform.position + new Vector3(0, 0, -0.3f);
                    }
                    else
                    {
                        newPosition = new Vector3(0, 0, 0);
                    }
                    arObjects[name].transform.position = trackedImage.transform.position + newPosition;
                    //a.transform.position = trackedImage.transform.position;

                    ShowAndroidToastMessage("Instantiated!");

                }
            }
        }

        foreach(var trackedImage in eventArgs.updated)
        {
            arObjects[trackedImage.referenceImage.name].SetActive(trackedImage.trackingState == TrackingState.Tracking);

            if(!arObjects["StudentID"].activeSelf)
            {
                //IsCard = false;
                arObjects4card["Youtube"].SetActive(false);
                arObjects4card["Twitter"].SetActive(false);
                arObjects4card["Android"].SetActive(false);
                arObjects4card["Uijin"].SetActive(false);
            }
            else{
                arObjects4card["Youtube"].SetActive(true);
                arObjects4card["Twitter"].SetActive(true);
                arObjects4card["Android"].SetActive(true);
                arObjects4card["Uijin"].SetActive(true);
            }

            if(!arObjects["GHStudentID"].activeSelf)
            {
                arObjects4HGcard["Hagyeong"].SetActive(false);
            }
            else{
                arObjects4HGcard["Hagyeong"].SetActive(true);
            }

            if(trackedImage.trackingState == TrackingState.Tracking)
            {
                imageTrackedText.text = trackedImage.referenceImage.name;
                if(trackedImage.referenceImage.name/*name*/ == "StudentID")
                {
                    newPosition = new Vector3(0, 0, 0.35f);
                    //IsCard = true;
                }
                else if(trackedImage.referenceImage.name == "GHStudentID")
                {
                    newPosition = new Vector3(0, 0, -0.3f);
                }
                else
                {
                    newPosition = new Vector3(0, 0, 0);
                }
                
                arObjects[trackedImage.referenceImage.name/*name*/].transform.position = trackedImage.transform.position + newPosition;
                arObjects4card["Youtube"].transform.position = trackedImage.transform.position+ new Vector3(0.35f, 0, 0.2f);
                arObjects4card["Twitter"].transform.position = trackedImage.transform.position + new Vector3(-0.3f, 0, 0.05f);
                arObjects4card["Android"].transform.position = trackedImage.transform.position + new Vector3(0.3f, 0, -0.1f);
                arObjects4card["Uijin"].transform.position = trackedImage.transform.position + new Vector3(0, 0.05f, 0.35f);

                arObjects4HGcard["Hagyeong"].transform.position = trackedImage.transform.position + new Vector3(0, 0, -0.3f);

            }
        }

        foreach(var trackedImage in eventArgs.removed)
        {
            if(trackedImage.referenceImage.name == "StudentID")
            {
                //IsCard = false;
                Destroy(arObjects4card["Youtube"]);
                arObjects4card.Remove("Youtube");

                Destroy(arObjects4card["Twitter"]);
                arObjects4card.Remove("Twitter");

                Destroy(arObjects4card["Android"]);
                arObjects4card.Remove("Android");

                Destroy(arObjects4card["Uijin"]);
                arObjects4card.Remove("Uijin");
            }

            if(trackedImage.referenceImage.name == "GHStudentID")
            {
                Destroy(arObjects4card["Hagyeong"]);
                arObjects4card.Remove("Hagyeong");
            }

            Destroy(arObjects[trackedImage.referenceImage.name]);
            arObjects.Remove(trackedImage.referenceImage.name);
        }
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private static void ShowAndroidToastMessage(string message)
    {
#if UNITY_ANDROID
        using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        if (unityActivity == null) return;
        var toastClass = new AndroidJavaClass("android.widget.Toast");
        unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            // Last parameter = length. Toast.LENGTH_LONG = 1
            using var toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText",
                unityActivity, message, 1);
            toastObject.Call("show");
        }));
#endif
    }
}
