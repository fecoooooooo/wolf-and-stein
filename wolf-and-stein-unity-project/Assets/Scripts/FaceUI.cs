using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceUI : MonoBehaviour
{
    Sprite leftImage;
    Sprite rightImage;
    Sprite idleImage;

    Image currentImage;

    Vector2 sideWatchTime = new Vector2(.2f, 2f);
    float sideWatchTimeLeft;
    float sideWatchChance = 0.01f;


    void Start()
    {
        currentImage = GetComponent<Image>();
        LoadImagesRegardingHP();
    }

    void Update()
    {
        if(Random.Range(0f, 1f) < sideWatchChance)
		{
            currentImage.sprite = Random.Range(0f, 1f) < .5f ? leftImage : rightImage;
            sideWatchTimeLeft = Random.Range(sideWatchTime.x, sideWatchTime.y);
        }

        if(0 < sideWatchTimeLeft)
		{
            sideWatchTimeLeft -= Time.deltaTime;
		}
		else
		{
            currentImage.sprite = idleImage;
		}
    }

    void LoadImagesRegardingHP()
	{
        //TODO: load regarding HP
        leftImage  = Resources.Load<Sprite>("wolf_left_healthy");
        rightImage = Resources.Load<Sprite>("wolf_right_healthy");
        idleImage = Resources.Load<Sprite>("wolf_idle_healthy");
	}
}
