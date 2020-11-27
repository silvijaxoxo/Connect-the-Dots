using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    const string fileName = "level_data2.json";
    const string folder = "Assets/";

    Dropdown m_Dropdown;
    Data data;

    void Start()
    {
        LoadLevelData();
        InitDropdown();
    }

    void LoadLevelData()
    {
        string path = folder + fileName;
        string contents = File.ReadAllText(path);

        // reading data from json file
        data = JsonConvert.DeserializeObject<Data>(contents);
    }

    void InitDropdown()
    {
        m_Dropdown = GetComponent<Dropdown>();
        m_Dropdown.options.Clear();

        // preparing dropdowm options
        for (int i = 1; i <= data.levels.Count; i++)
        {
            m_Dropdown.options.Add(new Dropdown.OptionData("Level " + i));
        }

        m_Dropdown.RefreshShownValue();
    }

    public List<string> GetSelectedLevelData()
    {
        return data.levels[m_Dropdown.value].level_data;
    }

    public class Level
    {
        public List<string> level_data { get; set; }
    }

    public class Data
    {
        public List<Level> levels { get; set; }
    }
}
