using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Game
{

public class GameManager : MonoBehaviour
{
    private enum State : int
    {
        Idle = 0,
        FindingGround = 1,
        PickMash = 2
    }

    public static GameManager instance;

    // Editor Fields
    public ARSessionOrigin sessionOrigin;
    public ARPointCloudManager pointCloudManager;
    public ARPlaneManager planeManager;
    public GameObject asset;
    public GameObject rocket;
    public PartUI partUI;


    // Variables
    private NativeArray<XRRaycastHit> raycastHits = new NativeArray<XRRaycastHit>();
    private Ray inputRay;
    private State state = State.Idle;
    private LayerMask layerMask;
    private ARPart selectedPart;

    // Unity Messages
    private void Awake()
    {
        instance = this;
        layerMask = LayerMask.GetMask(new string[] { "3D Model" });
        rocket = Instantiate(asset, new Vector3(0, 0, 0), Quaternion.identity);
        partUI.Hide();
            rocket.SetActive(false);
        }

    private void Start()
    {
#if UNITY_EDITOR
        state = State.PickMash;
        sessionOrigin.camera.transform.position = new Vector3(0, 0.5f, -1.5f);
#else
        rocket.SetActive(false);
        state = State.FindingGround;
#endif
    }

    private void OnEnable()
    {
        SetEvents();
    }

    private void OnDisable()
    {
        rocket = null;
        ClearEvents();
    }

    private void Update()
    {
        if (state == State.FindingGround)
        {
            RaycastDetectAndPlace();
        }
        if (state == State.PickMash)
        {
            RaycastPickMesh();
        }
    }

    private void RaycastDetectAndPlace()
    {
        if (Input.GetMouseButtonDown(0) && !Mmi.UI.MenuManager.instance.IsShowing())
        {
            // Get Ray
            inputRay = sessionOrigin.camera.ScreenPointToRay(Input.mousePosition);
            // Cast ray and get the collisions
            raycastHits = planeManager.Raycast(inputRay, TrackableType.AllTypes, Allocator.Temp);

            if (raycastHits.Length > 0)
            {
                rocket.transform.position = raycastHits[0].pose.position;
                rocket.SetActive(true);
                state = State.PickMash;
            }
        }
    }

    private void RaycastPickMesh()
    {
        if (Input.GetMouseButtonDown(0) && selectedPart == null && !Mmi.UI.MenuManager.instance.IsShowing())
        {
            inputRay = sessionOrigin.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            bool collided = Physics.Raycast(inputRay, out raycastHit, 10, layerMask);
            if (collided)
            {
                ARPart arPart = raycastHit.transform.GetComponent<ARPart>();
                if (arPart != null)
                {
                    selectedPart = arPart;
                    partUI.Set(selectedPart, raycastHit.point);
                }
                Debug.Log($"Mesh collided { raycastHit.collider.name }");
            }
        }
    }

    public void DeselectPart() {
        selectedPart = null;
    }

    public void HideSelectedPart()
    {
        if (selectedPart != null)
        {
            selectedPart.Hide();
            DeselectPart();
        }
    }

    private void SetEvents()
    {
        pointCloudManager.pointCloudsChanged += OnPoinCloudsChanged;
        planeManager.planesChanged += OnPlanesChanged;
    }

    private void ClearEvents()
    {
        pointCloudManager.pointCloudsChanged -= OnPoinCloudsChanged;
        planeManager.planesChanged -= OnPlanesChanged;
    }
    private void OnPoinCloudsChanged(ARPointCloudChangedEventArgs pEventArgs)
    {
        //Debug.Log("Point clouds changed");
    }
    private void OnPlanesChanged(ARPlanesChangedEventArgs pEventArgs)
    {
        // Debug.Log($"AR planes changed. Added {pEventArgs.added.Count}, Updated {pEventArgs.updated.Count}, Removed {pEventArgs.removed.Count}");
    }

    public void OnShowAllParts()
    {
        ARPartManager.ShowAll();
    }

    public void OnHideAllParts()
    {
        ARPartManager.HideAll();
    }

}

}