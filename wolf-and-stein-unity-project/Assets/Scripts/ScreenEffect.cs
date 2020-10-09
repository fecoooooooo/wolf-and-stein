using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffect : MonoBehaviourSingleton<ScreenEffect>
{
    public float BlinkAnimationHalfTime = .05f;
    public float BlinkGoalOpacity = .75f;

    public float DeathAnimTime = 1f;
    public int DeathAnimIterationsPerFrame = 30;

    Image powerUpEffectImg;
    Image damageEffectImg;
    RawImage deathEffectImg;

	void Start()
    {
        powerUpEffectImg = transform.Find("PowerUpEffectImg").GetComponent<Image>();
        damageEffectImg = transform.Find("DamageEffectImg").GetComponent<Image>();
        deathEffectImg = transform.Find("DeathEffectImg").GetComponent<RawImage>();
    }

    public void PlayPowerUp()
	{
        StartCoroutine(PlayBlinkEffectOnImage(powerUpEffectImg));
	}

    public void PlayDamage()
    {
        StartCoroutine(PlayBlinkEffectOnImage(damageEffectImg));
    }

    public void PlayDeath()
	{
        StartCoroutine(PlayPixelizeOnTexture(deathEffectImg));
	}


    IEnumerator PlayBlinkEffectOnImage(Image image)
	{
        float timePassed = 0;
        Color currentColor = image.color;

        while(timePassed < BlinkAnimationHalfTime)
		{
            float t = timePassed / BlinkAnimationHalfTime;
            currentColor.a = Mathf.Lerp(0, BlinkGoalOpacity, t);
            image.color = currentColor;
            
            timePassed += Time.deltaTime;

            yield return null;
		}

        timePassed = 0;

        while (timePassed < BlinkAnimationHalfTime)
        {
            float t = timePassed / BlinkAnimationHalfTime;
            currentColor.a = Mathf.Lerp(BlinkGoalOpacity, 0, t);
            image.color = currentColor;

            timePassed += Time.deltaTime;

            yield return null;
        }

        currentColor.a = 0;
        image.color = currentColor;
    }

    IEnumerator PlayPixelizeOnTexture(RawImage image)
	{
        int width = (int)image.rectTransform.rect.width;
        int height = (int)image.rectTransform.rect.height;

        Texture2D texture = new Texture2D(width / 5, height / 5);
        image.texture = texture;
        
        float timePassed = 0;
        while(timePassed < DeathAnimTime)
		{
            for(int i = 0; i < DeathAnimIterationsPerFrame; ++i)
			{
                int u = Random.Range(0, texture.width);
                int v = Random.Range(0, texture.height);
                texture.SetPixel(u, v, Color.red);
			}

            texture.Apply();

            timePassed += Time.deltaTime;
            yield return null;
		}
	}

}
