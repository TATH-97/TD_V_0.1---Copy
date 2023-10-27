using System;
using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using Unity.Mathematics;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
   public static LevelManager instance;
   [SerializeField] public GameObject[] spawnablePrefabs; //used for server overrides, allows server to spawn what is in this
   [SerializeField] private GameObject[] spawners; //Maybe find better way to do this
   [SerializeField] private GameObject[] spawnees; // array of spawnable minions

   [SerializeField] public GridLayout gameBoard; //The hexboard
   [SerializeField] public GameObject spire;

    [SerializeField] private int baseCount=12;
    [SerializeField] private float scalingFactor=.75f;
    private ArrayList spawnLocations = new ArrayList(); 
    // private ArrayList minions = new ArrayList();
    private float minionsPerSecond=.5f;  
    private float timeBetweenWaves=5f; //update
    private int waveCount=1;
    private float lastSpawnTime=0f;  
    private float lastWaveTime;
    private float time1;
    private int minionsAlive=0;
    private int minionsLeftToSpawn;  
    private bool isSpawning=false; 
    private int minionsPerSource;
    private int remainder; 
    private int idex=0;

    public static UnityEvent onMinionKilled = new UnityEvent();
    [SerializeField] public Text displayTimer;


   private void Awake() {
        if(instance==null) {
            instance=this;
        } 
        onMinionKilled.AddListener(MinionDestroyed);
        Debug.Log("Past");
        foreach(GameObject gm in spawners) {
            spawnLocations.Add(gm.GetComponent<Transform>());
        }
    }

    private void Update() { 
        if(isSpawning) {
            TimeConversion(0f);
            // Debug.Log("isSpawning");
            lastSpawnTime+=Time.deltaTime;
            if(lastSpawnTime>=1f/minionsPerSecond && minionsPerSource>0) {
                // Debug.Log("Spawn");
                foreach(Transform trans in spawnLocations) {
                    cmdSpawn(trans.position, Quaternion.identity);
                }
                minionsPerSource--;
            }
            if(minionsAlive==0 && minionsPerSource<=0) {
                EndWave();
            }
        }

        if (!isSpawning) {
            lastWaveTime+=Time.deltaTime;
            time1=timeBetweenWaves-lastWaveTime;
            TimeConversion(time1);
            if(lastWaveTime>=timeBetweenWaves) {
                isSpawning=true;
                StartWave();
            }
        }
    }

    private void MinionDestroyed() {
        Debug.Log("MinionDestroyed");
        minionsAlive--;
    }

    private void StartWave() {
        Debug.Log("StartWave");
        minionsLeftToSpawn=minionsPerWave();
        minionsPerSource=minionsLeftToSpawn/spawners.Length;
        remainder=minionsLeftToSpawn%spawners.Length;
        if(remainder>=(.5*spawners.Length)) {
            minionsPerSource++;
        }
        waveCount++;
    }

    private void EndWave() {
        Debug.Log("End Wave");
        BuildManager.instance.currency+=Mathf.RoundToInt((float)(100*math.pow(waveCount, scalingFactor)));
        isSpawning=false;
        lastWaveTime=0f;
    }

    private int minionsPerWave() {
        return Mathf.RoundToInt((float)(baseCount *Math.Pow(waveCount, scalingFactor)));
    }

    void cmdSpawn(Vector3 pos, Quaternion rot) {
        GameObject newBorn=Instantiate(spawnees[idex]);
        BasicMinionMovement move =newBorn.GetComponent<BasicMinionMovement>();
        move.setTarget(spire.transform);
        newBorn.transform.SetPositionAndRotation(pos, rot);
        NetworkServer.Spawn(newBorn);
        minionsAlive++;
    }

    public GameObject[] GetPrefabs() {
        return spawnablePrefabs;
    }

    private void TimeConversion(float inputTime) {
        float minutes = Mathf.FloorToInt(inputTime / 60);  
        float seconds = Mathf.FloorToInt(inputTime % 60);
        displayTimer.text=string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
