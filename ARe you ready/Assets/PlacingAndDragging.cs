using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;

public class PlacingAndDragging : MonoBehaviour
{
    [SerializeField] AudioSource scanSfx;
    [SerializeField] AudioSource scanCompleteSfx;

    static public bool spawnable = false; //when spawn UI pressed, true.
    bool LimitBall = false;
    bool changeColor = false;
    bool destroyAll = false;
    float timeToRestart = 0f;
    static public bool playsfxcheck = false;
    static public bool playsfxcheck2 = false;

    [SerializeField]
    public Text debugLog;

    public ARRaycastManager arRaycastManager;
    public ARSessionOrigin aRSessionOrigin;
    public ARPlaneManager aRPlaneManager;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private GameObject bowingBall;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Vector2 touchPosition = default;

    [SerializeField]
    private GameObject placedPrefab;

    ///////select object
    [SerializeField]
    private Color activeColor = Color.red;

    [SerializeField]
    private Color inactiveColor = Color.gray;

    [SerializeField]
    private Color OriginPinColor = Color.white;

    [SerializeField]
    private Material OriginBallColor;

    [SerializeField]
    private bool displayOverlay = false;
    private PlacementObject lastSelectedObject;
    private bool onTouchHold = false;

    public Text checkPlaneLog;

    ARPlane curPlane;

    private GameObject PlacedPrefab
    {
        get
        {
            return placedPrefab;
        }
        set
        {
            placedPrefab = value;
        }
    }

    private void Awake() {
        spawnable = false;
        changeColor = false;
        LimitBall = false;
        destroyAll = false;
        timeToRestart = 0f;
        playsfxcheck = false;
        playsfxcheck2 = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        debugLog.text = "Wait until detecting the plane\n keep moving your camera to scane your plane";
        foreach (ARPlane plane in aRPlaneManager.trackables)
        {
            Destroy(plane);
        }

        FilteredPlane.isBig = false;
        //OriginPinColor =  placedPrefab.GetComponent<MeshRenderer>().material.color;
    }

    void Init()
    {
        playsfxcheck = false;
        spawnable = false;
        changeColor = false;
        LimitBall = false;
        destroyAll = false;
        timeToRestart = 0f;
        lastSelectedObject = null;

        //debugLog.text = "Restart and init";
    }

    // Update is called once per frame
    void Update()
    {
        if(!playsfxcheck && !scanSfx.isPlaying)
        {
            scanSfx.Play();
            //SoundManager.instance.PlaySfx("ScanningPlane");
        }
        var cameraForward = arCamera.transform.forward;
        //camera direction text
        //debugLog.text = cameraForward.x +", " + cameraForward.y + ", " + cameraForward.z;

        checkPlaneLog.text = FilteredPlane.isBig.ToString();

        if (FilteredPlane.isBig)
        {
            foreach (ARPlane plane in aRPlaneManager.trackables)
            {
                if (plane.extents.x * plane.extents.y >= FilteredPlane.dismenstionsForBigPlanes.x * FilteredPlane.dismenstionsForBigPlanes.y)
                {
                    aRPlaneManager.enabled = false;
                    playsfxcheck = true;
                    
                    checkPlaneLog.text = "Done!";
                    //SoundManager.instance.PlaySfx("ScanningComplete");
                }
                else
                {
                    aRPlaneManager.enabled = false;
                    plane.gameObject.SetActive(aRPlaneManager.enabled);

                }
            }

            if(!scanCompleteSfx.isPlaying && !playsfxcheck2)
            {
                playsfxcheck2 = true;
                scanSfx.Stop();
                scanCompleteSfx.Play();
                debugLog.text = "put pins and dragging on your plane, then press Ball button";
            }

            curPlane = FindObjectOfType<ARPlane>();

            if (destroyAll)
            {
                timeToRestart += Time.deltaTime;
                if(timeToRestart > 6f)
                {
                    //Init();
                    //swipeBall.Init();
                    SceneManager.LoadScene("RollBall");
                }
            }
            if(swipeBall.toBeDestroy)
            {
                PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();
                foreach (PlacementObject placementObject in allOtherObjects)
                {
                    Destroy(placementObject.gameObject, 3f);
                }
            
                destroyAll = true;
                //SceneManager.LoadScene("RollBall");
            }

            if (swipeBall.rollable && !changeColor)
            {
                changeColor = true;
                PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();
                foreach (PlacementObject placementObject in allOtherObjects)
                {

                    MeshRenderer meshRenderer = placementObject.GetComponent<MeshRenderer>();
                    //placementObject.Selected = false;

                    if (placementObject.transform.name == "pin(Clone)")
                    {
                        //debugLog.text = "Pin";
                        meshRenderer.material.color = OriginPinColor;
                    }
                    else if (placementObject.transform.name == "Sphere(Clone)")
                    {
                        //debugLog.text = "Spehre";
                        meshRenderer.material = OriginBallColor;
                    }
                }
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }

                touchPosition = touch.position;
                List<ARRaycastHit> hits = new List<ARRaycastHit>();

                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = arCamera.ScreenPointToRay(touch.position);
                    RaycastHit hitObject;

                    if (Physics.Raycast(ray, out hitObject) && !swipeBall.rollable)
                    {
                        lastSelectedObject = hitObject.transform.GetComponent<PlacementObject>();

                        if (lastSelectedObject != null)
                        {
                            PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();

                            foreach (PlacementObject placementObject in allOtherObjects)
                            {
                                MeshRenderer meshRenderer = placementObject.GetComponent<MeshRenderer>();

                                if (placementObject != lastSelectedObject)
                                {
                                    placementObject.Selected = false;
                                    meshRenderer.material.color = inactiveColor;
                                }
                                else
                                {
                                    placementObject.Selected = true;
                                    meshRenderer.material.color = activeColor;
                                }
                            }
                        }
                    }
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    lastSelectedObject.Selected = false;
                }

                if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
 
                    if (spawnable)
                    {
                        if (lastSelectedObject == null && !LimitBall)
                        {
                            //debugLog.text = "spawn ball";
                            LimitBall = true;
                            lastSelectedObject = Instantiate(bowingBall, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                            SoundManager.instance.PlaySfx("Placement");
                            float yDiff = curPlane.transform.localPosition.y - (lastSelectedObject.GetComponent<SphereCollider>().bounds.min.y);
                            Vector3 spawnPosition = new Vector3(lastSelectedObject.transform.position.x, lastSelectedObject.transform.position.y + yDiff, lastSelectedObject.transform.position.z);
                            lastSelectedObject.Size = 1;
                            lastSelectedObject.transform.position = spawnPosition;
                            lastSelectedObject.YPosition = spawnPosition.y;
                        }
                        else
                        {
                            if (lastSelectedObject.Selected)
                            {
                                Vector3 newPosition = new Vector3(hitPose.position.x, lastSelectedObject.YPosition, hitPose.position.z);
                                lastSelectedObject.transform.position = newPosition;
                                lastSelectedObject.transform.rotation = hitPose.rotation;
                            }
                        }

                        //spawnable = false;
                    }
                    else if (!swipeBall.rollable)
                    {
                        if (lastSelectedObject == null)
                        {
                            //debugLog.text = "spawn pins";
                            lastSelectedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                            SoundManager.instance.PlaySfx("Placement");

                            float yDiff = curPlane.transform.localPosition.y - (lastSelectedObject.GetComponent<CapsuleCollider>().bounds.min.y);
                            Vector3 spawnPosition = new Vector3(lastSelectedObject.transform.position.x, lastSelectedObject.transform.position.y + yDiff, lastSelectedObject.transform.position.z);
                            lastSelectedObject.Size = 1;
                            lastSelectedObject.transform.position = spawnPosition;
                            lastSelectedObject.YPosition = spawnPosition.y;
                        }
                        else
                        {
                            if (lastSelectedObject.Selected)
                            {
                                Vector3 newPosition = new Vector3(hitPose.position.x, lastSelectedObject.YPosition, hitPose.position.z);
                                lastSelectedObject.transform.position = newPosition;
                                lastSelectedObject.transform.rotation = hitPose.rotation;
                            }
                        }
                    }

                }
            }
        }
    }
}
