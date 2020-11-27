using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour, IResolutionObserver
{
    LineRenderer m_LineRenderer;

    Vector3 startPoint;
    Vector3 endPoint;

    Vector3 currentPos; // used for animating line

    bool startedDrawing; // is line being animated
    bool drawn; // is line finished drawing

    const int MAX_POSITION = 1000; // variable defining x and y positions' max value

    void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        drawn = false;
        startedDrawing = false;
    }

    void Start()
    {
        GameObject resolutionSubject = GameObject.FindGameObjectWithTag("ResolutionSubject");
        if (resolutionSubject != null)
            resolutionSubject.GetComponent<Resolution>().Attach(this);
    }

    void OnDestroy()
    {
        // detaches gameObject from resolution subject
        GameObject resolutionSubject = GameObject.FindGameObjectWithTag("ResolutionSubject");
        if(resolutionSubject != null)
            resolutionSubject.GetComponent<Resolution>().Detach(this);
    }

    public void InitLine(Vector3 startPoint, Vector3 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;

        currentPos = startPoint;

        // set line's start(0) and end(1) positions
        m_LineRenderer.positionCount = 2;
        m_LineRenderer.SetPosition(0, UpdateVector(startPoint));
        m_LineRenderer.SetPosition(1, UpdateVector(currentPos));

        // set line width multiplier based on camera's orthographic size
        m_LineRenderer.widthMultiplier = 0.1f * Camera.main.orthographicSize;

        // calculate rope texture tiling
        m_LineRenderer.material.mainTextureScale = new Vector2(0.37f / m_LineRenderer.widthMultiplier, 1);
    }

    public bool IsDrawn()
    {
        return drawn;
    }

    public bool IsStartedDrawing()
    {
        return startedDrawing;
    }

    public void DrawLine()
    {
        startedDrawing = true;
        StartCoroutine(MoveLineToEndPoint());
    }

    IEnumerator MoveLineToEndPoint()
    {
        float maxTime = 5f;
        float dist = Vector3.Distance(startPoint, endPoint);

        // calculate rope animation time based on max time to draw line through whole screen
        float timeToMove = maxTime * dist / MAX_POSITION; 

        while (!drawn)
        {
            for (float t = 0f; t < timeToMove; t += Time.deltaTime)
            {
                currentPos = Vector3.Lerp(currentPos, endPoint, Mathf.Min(1, t/timeToMove));
                m_LineRenderer.SetPosition(1, UpdateVector(currentPos));

                if (Vector3.Distance(currentPos, endPoint) < 0.01f)
                {
                    drawn = true;
                }
                yield return null;
            }
        }
    }

    /**
     * Method used to update GameObject based on resolution subject
     */
    public void UpdateObserver(IResolutionSubject subject)
    {
        if(startPoint != null)
            m_LineRenderer.SetPosition(0, UpdateVector(startPoint));
        if (endPoint != null)
            m_LineRenderer.SetPosition(1, UpdateVector(endPoint));
    }

    Vector3 UpdateVector(Vector3 vector)
    {
        Vector3 p = new Vector3(vector.x / MAX_POSITION, (MAX_POSITION - vector.y) / MAX_POSITION, 1);
        p = Camera.main.ViewportToWorldPoint(p);

        return p;
    }
}
