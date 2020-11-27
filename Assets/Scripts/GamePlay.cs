using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    [SerializeField]
    GameObject button;  // button gameObject

    Point first, current, previous; // references to first, current and previously clicked points

    int pointCount;     // total point count of the level

    [SerializeField]
    GameObject line;    // line gameObject

    List<Line> lines;   // list of line gameObjects

    [SerializeField]
    GameObject menu;    // menu gameObject to be shown after level is completed

    bool playing;

    void Awake()
    {
        playing = false;
    }

    /** method called before starting the level */
    public void StartGame(List<string> levelData)
    {
        // delete any game objects from previous levels
        DeleteGameObjects("Button");
        DeleteGameObjects("Line");

        List<Vector2> points = InitPoints(levelData);
        pointCount = points.Count;
        DrawPoints(points);
        
        lines = new List<Line>();
        playing = true;
    }

    /** deletes gameObjects with a specific tag */
    void DeleteGameObjects(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach(GameObject obj in objects)
        {
            Destroy(obj);
        }
    }

    /**
     * returns a list of points created from string list;
     * the created points' list is later used for instantiating game objects
     */
    List<Vector2> InitPoints(List<string> levelData)
    {
        List<Vector2> pointList = new List<Vector2>();

        for(int i = 0; i< levelData.Count; i += 2)
        {
            pointList.Add(new Vector2(int.Parse(levelData[i]), int.Parse(levelData[i + 1])));
        }
        
        return pointList;
    }

    /** instantiates Button gameobjects from list of points */
    void DrawPoints(List<Vector2> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            GameObject btn = Instantiate(button);
            btn.GetComponent<Point>().SetPosition((int)points[i].x, (int)points[i].y);
            int number = i + 1;
            btn.GetComponent<Point>().SetNumber(number);
        }
    }

    void Update()
    {
        if (playing) // execute Update only when the game is being played
        {
            // checking user input
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null)
                {
                    if(hit.collider.gameObject.tag == "Button")
                    {
                        Point point = hit.collider.gameObject.GetComponent<Point>();
                        if (!point.IsClicked()) // if point hasn't been clicked before
                        {
                            if(point.GetNumber() == 1) // if it's the first point
                            {
                                point.Click();
                                current = point;
                                first = point;
                            }
                            else if(current != null && point.GetNumber() - current.GetNumber() == 1) // if the previous point has been selected and the number of the point is correct
                            {
                                point.Click();
                                previous = current;
                                current = point;

                                // create line gameObject
                                GameObject ln = Instantiate(line);
                                ln.GetComponent<Line>().InitLine(previous.GetPosition(), current.GetPosition());
                                lines.Add(ln.GetComponent<Line>());

                                if(point.GetNumber() == pointCount) // if the last point has been clicked
                                {
                                    ln = Instantiate(line);
                                    ln.GetComponent<Line>().InitLine(current.GetPosition(), first.GetPosition());
                                    lines.Add(ln.GetComponent<Line>());
                                }
                            }
                        }
                    }
                }
            }

            if(lines != null)
            {
                // checking when to draw another line
                for (int i = 0; i < lines.Count; i++)
                {
                    bool first = i == 0 && !lines[i].IsStartedDrawing(); // if the first line hasn't started to be drawn
                    bool other = i > 0 && !lines[i].IsStartedDrawing() && lines[i - 1].IsDrawn(); // if the previous line is drawn the following can be started as well

                    if (first || other)    
                    {
                        lines[i].DrawLine();
                    }
                }

                // checking if level is completed
                if (CheckIfLevelCompleted())
                {
                    Debug.Log("Level completed!");
                    playing = false;
                    menu.SetActive(true);
                    this.enabled = false;
                }
            }

        }
    }

    bool CheckIfLevelCompleted()
    {
        // check if all lines have been drawn
        int drawnLinesCount = 0;
        foreach (Line ln in lines)
        {
            if (ln.IsDrawn())
                drawnLinesCount++;
        }

        // if drawn line count is equal to point amount, all lines have been drawn and level is completed
        if (drawnLinesCount == pointCount)
            return true;

        return false;
    }
}
