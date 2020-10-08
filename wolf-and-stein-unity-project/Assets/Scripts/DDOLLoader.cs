using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOLLoader : MonoBehaviour
{
    const string DDOL_PREFAB_NAME_PATH = "DontDestroyOnLoad";
    static bool added;

    void Start()
    {
        if (added)
            Destroy(gameObject);
		else
		{
            added = true;
            GameObject DDOLPrefab = Resources.Load(DDOL_PREFAB_NAME_PATH) as GameObject;
            Instantiate(DDOLPrefab, transform);
            DontDestroyOnLoad(gameObject);
		}
    }
}
