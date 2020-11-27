using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** The following interfaces and classes are used to implement observer pattern
 *  which is used to notify observers based on resolution change (later changed to camera aspect's change).
 *  
 *  (I know that this wasn't necesarry but I decided to implement this pattern to 
 *  make button and line gameObjects more responsive to the screen aspect changes. 
 *  If the screen aspect ratio changes, the gameObjects change their positions
 *  accordingly, still maintaining their original positions based on viewport)
 */

public interface IResolutionObserver
{
    void UpdateObserver(IResolutionSubject subject);
}

public interface IResolutionSubject
{
    void Attach(IResolutionObserver observer);
    
    void Detach(IResolutionObserver observer);
    
    void Notify();
}

public class Resolution : MonoBehaviour, IResolutionSubject
{
    //private Vector2 resolution;
    private float aspect;

    private void Awake()
    {
        //resolution = new Vector2(Screen.width, Screen.height);
        aspect = Camera.main.aspect;
    }

    private void Update()
    {
        //if (resolution.x != Screen.width || resolution.y != Screen.height)
        //{
        //    // notify resolution observers
        //    Notify();

        //    resolution.x = Screen.width;
        //    resolution.y = Screen.height;
        //}

        if (aspect != Camera.main.aspect)   // notify resolution observers based on camera aspect
        {
            Notify();

            aspect = Camera.main.aspect;
        }
    }
    
    public int State { get; set; } = -0;
    
    private List<IResolutionObserver> resolutionObservers = new List<IResolutionObserver>();
    
    public void Attach(IResolutionObserver observer)
    {
        resolutionObservers.Add(observer);
    }

    public void Detach(IResolutionObserver observer)
    {
        resolutionObservers.Remove(observer);
    }
    
    public void Notify()
    {
        foreach (var observer in resolutionObservers)
        {
            observer.UpdateObserver(this);
        }
    }
}
