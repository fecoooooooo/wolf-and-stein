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

    HPState previousHPState;


    void Start()
    {
        currentImage = GetComponent<Image>();
        LoadImagesRegardingHP();

        Character.instance.HPChanged += OnRecievedDamage;
    }

    void Update()
    {
        if(Random.Range(0f, 1f) < sideWatchChance)
		{
            currentImage.sprite = Random.Range(0f, 1f) < .5f ? leftImage : rightImage;
            sideWatchTimeLeft = Random.Range(sideWatchTime.x, sideWatchTime.y);
        }

        if(0 < sideWatchTimeLeft)
            sideWatchTimeLeft -= Time.deltaTime;
		else
            currentImage.sprite = idleImage;
    }

    void OnRecievedDamage(object sender, System.EventArgs e)
	{
        if(previousHPState != Character.instance.HPState)
            LoadImagesRegardingHP();

        previousHPState = Character.instance.HPState;
	}

    void LoadImagesRegardingHP()
	{
		switch (Character.instance.HPState)
		{
			case HPState.Healty:
				leftImage = Resources.Load<Sprite>("wolf_left_healthy");
				rightImage = Resources.Load<Sprite>("wolf_right_healthy");
				idleImage = Resources.Load<Sprite>("wolf_idle_healthy");
				break;
			case HPState.Hurt1:
				leftImage = Resources.Load<Sprite>("wolf_left_hurt1");
				rightImage = Resources.Load<Sprite>("wolf_right_hurt1");
				idleImage = Resources.Load<Sprite>("wolf_idle_hurt1");
				break;
			case HPState.Hurt2:
				leftImage = Resources.Load<Sprite>("wolf_left_hurt2");
				rightImage = Resources.Load<Sprite>("wolf_right_hurt2");
				idleImage = Resources.Load<Sprite>("wolf_idle_hurt2");
				break;
			case HPState.Hurt3:
				leftImage = Resources.Load<Sprite>("wolf_left_hurt3");
				rightImage = Resources.Load<Sprite>("wolf_right_hurt3");
				idleImage = Resources.Load<Sprite>("wolf_idle_hurt3");
				break;
			case HPState.Hurt4:
				leftImage = Resources.Load<Sprite>("wolf_left_hurt4");
				rightImage = Resources.Load<Sprite>("wolf_right_hurt4");
				idleImage = Resources.Load<Sprite>("wolf_idle_hurt4");
				break;
			case HPState.Hurt5:
				leftImage = Resources.Load<Sprite>("wolf_left_hurt5");
				rightImage = Resources.Load<Sprite>("wolf_right_hurt5");
				idleImage = Resources.Load<Sprite>("wolf_idle_hurt5");
				break;
			case HPState.Hurt6:
				leftImage = Resources.Load<Sprite>("wolf_left_hurt6");
				rightImage = Resources.Load<Sprite>("wolf_right_hurt6");
				idleImage = Resources.Load<Sprite>("wolf_idle_hurt6");
				break;
			case HPState.Dead:
				leftImage = Resources.Load<Sprite>("wolf_idle_death");
				rightImage = Resources.Load<Sprite>("wolf_idle_death");
				idleImage = Resources.Load<Sprite>("wolf_idle_death");
				break;
			default:
				throw new System.Exception("No such HPState exists");
		}
	}
}
