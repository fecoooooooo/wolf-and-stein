using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffect : MonoBehaviourSingleton<ScreenEffect>
{
    public float AnimationHalfTime = .05f;
    public float GoalOpacity = .75f;

    Image powerUpEffectImg;
    Image damageEffectImg;

	void Start()
    {
        powerUpEffectImg = transform.Find("PowerUpEffectImg").GetComponent<Image>();
        damageEffectImg = transform.Find("DamageEffectImg").GetComponent<Image>();
    }

    public void PlayPowerUp()
	{
        StartCoroutine(PlayEffectOnImage(powerUpEffectImg));
	}

    public void PlayDamage()
    {
        StartCoroutine(PlayEffectOnImage(damageEffectImg));
    }

    IEnumerator PlayEffectOnImage(Image image)
	{
        float timePassed = 0;
        Color currentColor = image.color;

        while(timePassed < AnimationHalfTime)
		{
            float t = timePassed / AnimationHalfTime;
            currentColor.a = Mathf.Lerp(0, GoalOpacity, t);
            image.color = currentColor;
            
            timePassed += Time.deltaTime;

            yield return null;
		}

        timePassed = 0;

        while (timePassed < AnimationHalfTime)
        {
            float t = timePassed / AnimationHalfTime;
            currentColor.a = Mathf.Lerp(GoalOpacity, 0, t);
            image.color = currentColor;

            timePassed += Time.deltaTime;

            yield return null;
        }

        currentColor.a = 0;
        image.color = currentColor;
    }
}
