using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    const float TIME_BETWEEN_ANIMATIONS = .2f;

    void Start()
    {
        SaveGameHandler.instance.LoadData();
    }

    void RestartWithDeath()
	{
        StartCoroutine(RestartWithDeathRoutine());
	}

	IEnumerator RestartWithDeathRoutine()
	{
        StopAllAnimations();

        ScreenEffect.instance.PlayDeath();
        do
        {
            yield return null;
        } while (ScreenEffect.instance.Animating);

        yield return new WaitForSeconds(TIME_BETWEEN_ANIMATIONS);

        Map.instance.RegenerateCurrent();

        do
        {
            yield return null;
        } while (Map.instance.Generating);


        yield return new WaitForSeconds(TIME_BETWEEN_ANIMATIONS);
        ScreenEffect.instance.PlayRespawn();
    }

    private void Update()
	{
    }

    void StopAllAnimations()
	{
        Animation[] animatorsInTheScene = FindObjectsOfType(typeof(Animation)) as Animation[];
        foreach (Animation animatorItem in animatorsInTheScene)
            animatorItem.Stop();
    }
}

public enum GameSate
{
    LOADING,
    IN_GAME
}