using System;
using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using Unity.Mathematics;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
   public static LevelManager instance;
   [SerializeField] public GameObject[] spawnablePrefabs; //used for server overrides, allows server to spawn what is in this
   [SerializeField] private GameObject[] spawners; //Array of spawn locations
   [SerializeField] private GameObject[] spawnees; // array of spawnable minions

   [SerializeField] public GridLayout gameBoard; //The hexboard
   [SerializeField] public GameObject spire; //minion destenation

    [SerializeField] private int baseCount=12;
    [SerializeField] private float scalingFactor=.75f;
    [SerializeField] private Button startWaveButton;
    private ArrayList spawnLocations = new ArrayList(); 
    private float minionsPerSecond=1f;  
    private float timeBetweenWaves=60f; //update
    private int waveCount=0;
    private float lastSpawnTime=0f;  
    private float lastWaveTime=0;
    private int minionsLeftToSpawn;  
    public bool isSpawning=false; 
    private int minionsPerSource=0;
    private int remainder; 
    private int idex=0;
    private int minionsToKill=0;
    private int minionsKilled=0;

    public static UnityEvent onMinionKilled = new UnityEvent();
    [SerializeField] public Text displayTimer;
    [SerializeField] public Text roundCounter;


   private void Awake() {
        if(instance==null) {
            instance=this;
        } 
        onMinionKilled.AddListener(MinionDestroyed);
        foreach(GameObject gm in spawners) {
            spawnLocations.Add(gm.GetComponent<Transform>());
        }
        roundCounter.text=waveCount.ToString();
    }

    private void Update() { 
        if(isSpawning) {
            TimeConversion(0f);
            lastSpawnTime+=Time.deltaTime;
            if(lastSpawnTime>=1f/minionsPerSecond && minionsPerSource>0) {
                foreach(Transform trans in spawnLocations) {
                    cmdSpawn(trans.position, Quaternion.identity);
                }
                minionsPerSource--;
            }
            if(minionsToKill-minionsKilled<=0) {
                EndWave();
            }
        }

        if (!isSpawning) {
            lastWaveTime+=Time.deltaTime;
            TimeConversion(timeBetweenWaves-lastWaveTime);
            if(lastWaveTime>=timeBetweenWaves && minionsToKill-minionsKilled<=0) {
                StartWave();
            }
        }
    }

    private void MinionDestroyed() {
        minionsKilled++;
    }

    public void StartWave() {
        startWaveButton.interactable=false;
        
        Debug.Log("StartWave");
        waveCount++;
        minionsLeftToSpawn=minionsPerWave();
        isSpawning=true;
        minionsPerSource=minionsLeftToSpawn/spawners.Length;
        remainder=minionsLeftToSpawn%spawners.Length;
        if(remainder>=(.5*spawners.Length)) {
            minionsPerSource++;
        }
        minionsToKill=minionsPerSource*spawners.Length;
        Debug.Log("MinionsToKill: "+minionsToKill);
        roundCounter.text=waveCount.ToString();
        minionsKilled=0;
    }

    private void EndWave() {
        Debug.Log("End Wave");
        Debug.Log("MinionsKilled: "+minionsKilled);
        BuildManager.instance.currency+=Mathf.RoundToInt((float)(100*math.pow(waveCount, scalingFactor)));
        isSpawning=false;
        lastWaveTime=0f;
        startWaveButton.interactable=true;
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
