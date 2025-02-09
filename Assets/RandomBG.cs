using UnityEngine;
using Cinemachine;

public class RandomBackground : MonoBehaviour
{
    public GameObject[] dayBackgrounds;
    public GameObject[] nightBackgrounds; 
    public CinemachineConfiner2D confiner; 

    public enum BackgroundType { Day, Night, Random }
    public BackgroundType chosenBackgroundType;

    private int lastIndex = -1;

    void Start()
    {
        int savedType = PlayerPrefs.GetInt("ChosenBackgroundType", (int)BackgroundType.Random);
        chosenBackgroundType = (BackgroundType)savedType;
        SpawnBackground();
    }

    public void SetBackgroundType(int type)
    {
        chosenBackgroundType = (BackgroundType)type;

        PlayerPrefs.SetInt("ChosenBackgroundType", type);

        SpawnBackground();
    }

    void SpawnBackground()
    {

        GameObject oldBackground = GameObject.FindWithTag("Background");
        if (oldBackground != null)
        {
            Destroy(oldBackground);
        }

        GameObject[] chosenArray;
        switch (chosenBackgroundType)
        {
            case BackgroundType.Day:
                chosenArray = dayBackgrounds;
                break;
            case BackgroundType.Night:
                chosenArray = nightBackgrounds;
                break;
            case BackgroundType.Random:
                chosenArray = Random.value < 0.5f ? dayBackgrounds : nightBackgrounds;
                break;
            default:
                chosenArray = dayBackgrounds;
                break;
        }

        int index;
        do
        {
            index = Random.Range(0, chosenArray.Length);
        } while (index == lastIndex && chosenArray.Length > 1);

        lastIndex = index;

        Vector3 spawnPosition = new Vector3(0, +8, 0);
        GameObject chosenBackground = Instantiate(chosenArray[index], spawnPosition, Quaternion.identity);
        chosenBackground.tag = "Background";

        PolygonCollider2D backgroundCollider = chosenBackground.GetComponent<PolygonCollider2D>();
        if (backgroundCollider != null)
        {
            confiner.m_BoundingShape2D = backgroundCollider;
        }
        else
        {
            Debug.LogError("nu ai PolygonCollider2D component bro");
        }
    }
}
