using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    public Dictionary<int, Vector3> KeyPositions;
    public GameObject[] Levels;
    public TextMeshProUGUI CurrentSelectLevelText;

    private string SuggestedLevelName;
    private void Awake()
    {
        KeyPositions = new Dictionary<int, Vector3>();
        for (int i = 0; i < Levels.Length; i++)
        {
            KeyPositions.Add(i + 1, Levels[i].transform.position);
        }
    }

    public void SuggestLevelByPosition(Vector3 pos)
    {
        var levelToLoad = KeyPositions.First(x => x.Value.Equals(pos)).Key;

        SuggestedLevelName = "Level_" + levelToLoad;
        CurrentSelectLevelText.text = "Level :" + levelToLoad;
    }

    public void GoToSuggestedLevel()
    {
        if (SuggestedLevelName != string.Empty)
            SceneManager.LoadScene(SuggestedLevelName);
    }
}
