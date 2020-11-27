using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour, IResolutionObserver
{
    [SerializeField]
    Sprite blue_button; // sprite to be rendered after clicking the point

    SpriteRenderer m_SpriteRenderer;
    TextMesh m_TextMesh;

    bool clicked;
    const int MAX_POSITION = 1000; // variable defining x and y positions' max value

    Vector2 pos; // point positions as defined in original json data file

    void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_TextMesh = GetComponentInChildren<TextMesh>();
        m_TextMesh.GetComponent<MeshRenderer>().sortingOrder = 1;
    }

    void Start()
    {
        clicked = false;

        // ataches gameObject to resolution subject, thus making it responsive to resolution changes
        Resolution resolutionSubject = GameObject.FindGameObjectWithTag("ResolutionSubject").GetComponent<Resolution>();
        if (resolutionSubject != null)
            resolutionSubject.Attach(this);
    }

    void OnDestroy()
    {
        // detaches gameObject from resolution subject
        GameObject resolutionSubject = GameObject.FindGameObjectWithTag("ResolutionSubject");
        if(resolutionSubject != null)
            resolutionSubject.GetComponent<Resolution>().Detach(this);
    }

    /** sets button number */
    public void SetNumber(int i)
    {
        m_TextMesh.text = i.ToString();
    }

    /** gets button number */
    public int GetNumber()
    {
        int number = int.Parse(m_TextMesh.text);
        return number;
    }

    /** Checks whether the button has been clicked */
    public bool IsClicked()
    {
        return clicked;
    }

    /** method that is invoked after clicking on a button */
    public void Click()
    {
        clicked = true;
        m_SpriteRenderer.sprite = blue_button;
        StartCoroutine(NumberFadeOut(1f));
    }

    /** fades out button's number */
    IEnumerator NumberFadeOut(float fadeOutTime)
    {
        Color originalColor = m_TextMesh.color;
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime)
        {
            m_TextMesh.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeOutTime));
            yield return null;
        }
    }
    
    public void SetPosition(int x, int y)
    {
        pos = new Vector2(x, y);
        UpdatePosition();
    }
    
    public Vector2 GetPosition()
    {
        return pos;
    }

    /** updates gameObject's position on screen */
    public void UpdatePosition()
    {
        Vector3 p = new Vector3(pos.x / MAX_POSITION, (MAX_POSITION - pos.y) / MAX_POSITION, 1);
        p = Camera.main.ViewportToWorldPoint(p);
        transform.position = p;
    }

    public void UpdateObserver(IResolutionSubject subject)
    {
        UpdatePosition();
    }
}
