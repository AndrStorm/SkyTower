using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievmentsWindow : MonoBehaviour
{
    public GameObject achievmentPrefab;
    public RectTransform gridWindow;


    private List<AchievmentScriptable> achievments;


    void Awake()
    {
        achievments = AchievementManager.Instance.achievments;

        for (int i = 0; i < achievments.Count; i++)
        {
            var icon = Instantiate(achievmentPrefab, gridWindow) as GameObject;
            icon.name = $"Achievment {i} ";


            for (int j = 0; j < icon.transform.childCount; j++)
            {
                var child = icon.transform.GetChild(j);

                if (child.name =="Sprite")
                {
                    child.GetComponent<Image>().sprite = achievments[i].image;
                }
                else if (child.name == "Title")
                {
                    child.GetComponent<Text>().text = $"<b>{achievments[i].title}</b>";
                }
                else if (child.name == "Description")
                {
                    child.GetComponent<Text>().text = achievments[i].description;
                }
            }
                      
        }
    }
    private void OnEnable()
    {

        for (int i = 0; i < achievments.Count; i++)
        {
            var icon = gridWindow.GetChild(i);

            for (int j = 0; j < icon.childCount; j++)
            {
                var child = icon.GetChild(j);
                
                if (child.name == "Mask")
                {
                    if (achievments[i].GetAchived())
                        child.gameObject.SetActive(false);
                    else
                        child.gameObject.SetActive(true);
                }
            }

        }
    }

}
