using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    [SerializeField] private GameObject[] buildingPrefabs;
    private int selectedTower=0;
    public GameObject getSelectedTower() {
        return buildingPrefabs[selectedTower];
    }

    public GameObject[] GetPrefabs() {
        return buildingPrefabs;
    }
    private void Awake() {
        if(instance==null) {
            instance=this;
        }
    }

    public void DestroyBldg(GameObject bldg) {

    }

}
