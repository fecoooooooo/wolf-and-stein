﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffect : MonoBehaviourSingleton<ScreenEffect>
{
    public readonly Color DEATH_COLOR = Color.red;
    public readonly Color TRANSPARENT_COLOR = new Color(0, 0, 0, 0);

    public float BlinkAnimationHalfTime = .05f;
    public float BlinkGoalOpacity = .75f;

    public int DeathAnimIterationsPerFrame = 30;
    public int DeathAnimPixelSize = 20;

    public float EndSceneTime = 1f;

    Image powerUpEffectImg;
    Image damageEffectImg;
    RawImage deathEffectImg;
    Image endSceneImg;

    Transform originalparent;

    public bool Animating { get; private set; }

	void Start()
    {
        originalparent = transform.parent;
        powerUpEffectImg = transform.Find("PowerUpEffectImg").GetComponent<Image>();
        damageEffectImg = transform.Find("DamageEffectImg").GetComponent<Image>();
        deathEffectImg = transform.Find("DeathEffectImg").GetComponent<RawImage>();
        endSceneImg = transform.Find("EndSceneImg").GetComponent<Image>();
    }

    public void PlayPowerUp()
	{
        PlaceEffectsUnderHUD();
        StartCoroutine(PlayBlinkEffectOnImage(powerUpEffectImg));
	}

    public void PlayDamage()
    {
        PlaceEffectsUnderHUD();
        StartCoroutine(PlayBlinkEffectOnImage(damageEffectImg));
    }
    
    public void PlayDeath()
	{
        PlayEffectsOverMiniMap();
        deathEffectImg.gameObject.SetActive(true);
        StartCoroutine(PlayPixelizeOnTexture(deathEffectImg, DEATH_COLOR, TRANSPARENT_COLOR));
	}

    public void PlayRespawn()
    {
        PlayEffectsOverMiniMap();
        deathEffectImg.gameObject.SetActive(true);
        StartCoroutine(PlayPixelizeOnTexture(deathEffectImg, TRANSPARENT_COLOR, DEATH_COLOR));
    }

    public void PlayEndScene()
    {
        PlaceEffectsOverHUD();
        endSceneImg.gameObject.SetActive(true);
        StartCoroutine(PlayFadeInEffect(endSceneImg));
    }

    public void PlayStartScene()
    {
        PlaceEffectsOverHUD();
        endSceneImg.gameObject.SetActive(true);
        StartCoroutine(PlayFadeOutEffect(endSceneImg));
    }

    IEnumerator PlayFadeInEffect(Image image)
    {
        Animating = true;

        Color currentColor = image.color;

        float timePassed = 0;
        while (timePassed < EndSceneTime)
        {
            float t = timePassed / EndSceneTime;
            currentColor.a = Mathf.Lerp(0, 1, t);
            image.color = currentColor;

            timePassed += Time.deltaTime;

            yield return null;
        }

        currentColor.a = 1;
        image.color = currentColor;

        Animating = false;
    }

    IEnumerator PlayFadeOutEffect(Image image)
    {
        Animating = true;

        Color currentColor = image.color;

        float timePassed = 0;
        while (timePassed < EndSceneTime)
        {
            float t = timePassed / EndSceneTime;
            currentColor.a = Mathf.Lerp(1, 0, t);
            image.color = currentColor;

            timePassed += Time.deltaTime;

            yield return null;
        }

        currentColor.a = 0;
        image.color = currentColor;

        Animating = false;
    }

    IEnumerator PlayBlinkEffectOnImage(Image image)
	{
        Animating = true;

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

        Animating = false;
    }

    IEnumerator PlayPixelizeOnTexture(RawImage image, Color pixelColor, Color startTintColor)
	{
        Animating = true;

        int width = (int)image.rectTransform.rect.width;
        int height = (int)image.rectTransform.rect.height;

        Texture2D texture = GenerateAndSetupTextureForDeathAnim(image, width, height, startTintColor);
        Color[] colors = GetColorsArrForDeathAnim(DeathAnimPixelSize, pixelColor);
        List<Vector2Int> startingUVs = GenerateDeathAnimStartingUVs(width, height);

        while(startingUVs.Count > 0)
		{
            for (int i = 0; i < DeathAnimIterationsPerFrame; ++i)
			{
                if (0 >= startingUVs.Count)
                    break;

                int index = Random.Range(0, startingUVs.Count);
                int u = startingUVs[index].x;
                int v = startingUVs[index].y;

                int xBlockSize = Mathf.Min(DeathAnimPixelSize, width - u);
                int yBlockSize = Mathf.Min(DeathAnimPixelSize, height - v);

                startingUVs.RemoveAt(index);

                texture.SetPixels(u, v, xBlockSize, yBlockSize, colors);
			}

            texture.Apply();

            yield return null;
         
        }

        Animating = false;
    }

    private Texture2D GenerateAndSetupTextureForDeathAnim(RawImage image, int width, int height, Color startTintColor)
	{
        Texture2D texture = new Texture2D(width, height);

        Color[] colors = new Color[width * height];
        for (int i = 0; i < width * height; ++i)
            colors[i] = startTintColor;

        texture.SetPixels(colors);
        texture.Apply();

        image.texture = texture;

        return texture;
	}

	private Color[] GetColorsArrForDeathAnim(int deathAnimPixelSize, Color pixelColor)
	{
        Color[] colors = new Color[DeathAnimPixelSize * DeathAnimPixelSize];
        for (int i = 0; i < DeathAnimPixelSize * DeathAnimPixelSize; ++i)
            colors[i] = pixelColor;

        return colors;
    }


	private List<Vector2Int> GenerateDeathAnimStartingUVs(int width, int height)
	{
        List<Vector2Int> startingUVs = new List<Vector2Int>(width * height);

        int uCoordCount = Mathf.CeilToInt(width / (float)DeathAnimPixelSize);
        int vCoordCount = Mathf.CeilToInt(height / (float)DeathAnimPixelSize);

        for (int i = 0; i < uCoordCount; ++i)
        {
            for (int j = 0; j < vCoordCount; ++j)
                startingUVs.Add(new Vector2Int(i * DeathAnimPixelSize, j * DeathAnimPixelSize));
        }

        return startingUVs;
    }

    void PlaceEffectsOverHUD()
	{
        transform.SetParent(originalparent);
        transform.SetAsLastSibling();

    }
    void PlaceEffectsUnderHUD()
	{
        transform.SetParent(originalparent);
        HUD.instance.transform.SetAsLastSibling();
	}

    void PlayEffectsOverMiniMap()
	{
        transform.SetParent(HUD.instance.transform);
        transform.SetAsFirstSibling();
        MiniMap.instance.transform.SetAsFirstSibling();
    }
}
