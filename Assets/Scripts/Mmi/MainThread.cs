using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Mmi
{

public class MainThread : MonoBehaviour
{
    public static MainThread Dispatcher;
    private ConcurrentQueue<Action> jobs = new ConcurrentQueue<Action>();

    private void Awake()
    {
        Dispatcher = this;
    }

    private void Update()
    {
        Action job;
        while (jobs.TryDequeue(out job))
            if (job != null)
                job.Invoke();
    }

    public void AddJob(Action newJob)
    {
        jobs.Enqueue(newJob);
    }
}

}