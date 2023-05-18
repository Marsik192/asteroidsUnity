using UnityEngine;

namespace Mmi
{
public class MmiManager : MonoBehaviour
{

    public MmiService mmiService;

    private enum State : int
    {
        Idle = 0,
        Run = 1,
        Error = 2
    }

    public static MmiManager instance;

    // Variables
    private State state = State.Idle;

    // Unity Messages
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
#if UNITY_EDITOR
        state = State.Idle;
#else
        state = State.Idle;
#endif
    }

    private void OnEnable()
    {
        SetEvents();
    }

    private void OnDisable()
    {
        ClearEvents();
    }

    private void Update()
    {
        if (state == State.Idle)
        {
        }
        if (state == State.Run)
        {
        }
    }


    private void SetEvents()
    {
    }

    private void ClearEvents()
    {
    }

    private void OnModalityProviderChanged()
    {
        Debug.Log("Modality Provider changed");
    }

    public void OnStartMmiService()
    {
        mmiService.OnStartService();
    }

    public void OnStopMmiService()
    {
        mmiService.OnStopService();
    }

}

}