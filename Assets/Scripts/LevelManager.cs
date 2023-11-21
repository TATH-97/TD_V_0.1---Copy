using System;
using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using Unity.Mathematics;
using UnityEngine.UI;

public class LevelManager : NetworkBehaviour
{
   public static LevelManager instance;
   [SerializeField] public GameObject[] spawnablePrefabs; //used for server overrides, allows server to spawn what is in this
   [SerializeField] public GameObject[] spawners; //Array of spawn locations
   [SerializeField] private GameObject[] spawnees; // array of spawnable minions
   [SerializeField] public GridLayout gameBoard; //The hexboard
   [SyncVar] [SerializeField] public GameObject spire; //minion destenation

    [SerializeField] private int baseCount=12;
    [SerializeField] private float scalingFactor=.75f;
    [SerializeField] private Button startWaveButton;
    // [SerializeField] public GameObject fogOfWar;
    private ArrayList spawnLocations = new ArrayList(); 
    private float minionsPerSecond=.5f;  
    public float timeBetweenWaves=10f; //update
    [SyncVar] private int waveCount=0;
    private float lastSpawnTime=0f; 
    [SyncVar] public float lastWaveTime=0;
    private int minionsLeftToSpawn; 
    [SyncVar] public bool isSpawning=false; 
    private int minionsPerSource=0;
    private int remainder; 
    private int idex=0;
    private int minionsToKill=0;
    private int minionsKilled=0;
    public float roundTime=0f;

    public static UnityEvent onMinionKilled = new UnityEvent();
    [SerializeField] public Text displayTimerD;
    [SerializeField] public Text roundCounterD;
    [SerializeField] public Text displayTimerA;
    [SerializeField] public Text roundCounterA;
    public GameObject fogOfWarInstance; 


   private void Awake() {
        if(instance==null) {
            instance=this;
        } 
        onMinionKilled.AddListener(MinionDestroyed);
        foreach(GameObject gm in spawners) {
            spawnLocations.Add(gm.GetComponent<Transform>());
        }
        SetCounters();
    }

    public void SetFOW() {
        fogOfWarInstance=GameObject.FindWithTag("FogOfWarTag");
        fogOfWarInstance.SetActive(false);
    }

    private void Update() { 
        if(isSpawning) {
            TimeConversion(roundTime);
            roundTime+=Time.deltaTime;
            lastSpawnTime+=Time.deltaTime;  
            if(lastSpawnTime>=1f/minionsPerSecond &&minionsPerSource>0) {
                foreach(Transform trans in spawnLocations) {
                    cmdSpawn(trans.position, Quaternion.identity);
                }
                minionsPerSource--;
            }
            if(minionsToKill-minionsKilled<=0) {
                EndWaveCL();
            }
        }

        if (!isSpawning) {
            lastWaveTime+=Time.deltaTime;
            TimeConversion(timeBetweenWaves-lastWaveTime);
            if(lastWaveTime>=timeBetweenWaves && minionsToKill-minionsKilled<=0) {
                StartWaveCL();
            }
        }
    }

    // [ClientRpc]
    private void SetCounters() {
        roundCounterD.text=waveCount.ToString();
        roundCounterA.text=waveCount.ToString();
    }

    private void MinionDestroyed() {
        minionsKilled++;
    }

//***************START WAVE NETCODE***************\\
    public void StartWave() {
        startWaveButton.interactable=false;
        roundTime+=Time.deltaTime;
        Debug.Log("StartWave");
        waveCount++;
        minionsLeftToSpawn=minionsPerWave();
        isSpawning=true;
        fogOfWarInstance.SetActive(true);
        lastWaveTime=timeBetweenWaves-.5f;
        
        minionsPerSource=minionsLeftToSpawn/spawners.Length;
        remainder=minionsLeftToSpawn%spawners.Length;
        if(remainder>=(.5*spawners.Length)) {
            minionsPerSource++;
        }
        minionsToKill=minionsPerSource*spawners.Length;
        Debug.Log("MinionsToKill: "+minionsToKill);
        roundCounterD.text=waveCount.ToString();
        roundCounterA.text=waveCount.ToString();
        minionsKilled=0;
    }

    [Client] public void StartWaveCL() {
        Debug.Log("Start");
        if(!isClientOnly) {
            Debug.Log("Local Call, start, !isClientOnly");
            CMDStartWave();
            return;
        } else {
            Debug.Log("Local Call, start");
            StartWave();
            CMDStartWave();
        }
    }

    [Command(requiresAuthority =false)] public void CMDStartWave() {
        Debug.Log("Command call, start");
        StartWave();
        RPCStartWave();
    }

    [ClientRpc] private void RPCStartWave() {
        if(!isClientOnly) {
            Debug.Log("Client call, escape");
            return;
        }
        Debug.Log("Client call, start");
        StartWave();
    }
//***************START WAVE NETCODE***************\\



//***************END WAVE NETCODE***************\\
    private void EndWave() {
        Debug.Log("End Wave");
        Debug.Log("MinionsKilled: "+minionsKilled);
        roundTime=0f;
        BuildManager.instance.currency+=Mathf.RoundToInt((float)(100*math.pow(waveCount, scalingFactor)));
        isSpawning=false;
        fogOfWarInstance.SetActive(false);
        lastWaveTime=0f;
        startWaveButton.interactable=true;
    }

    [Client] public void EndWaveCL() {
        if(!isClientOnly) {
            CMDEndWave();
            return;
        } else {
            Debug.Log("Local Call, end");
            EndWave();
            CMDEndWave();
            return;
        }
    }

    [Command(requiresAuthority =false)] public void CMDEndWave() {
        Debug.Log("Server Call, end");
        EndWave();
        RPCEndWave();
    }

    [ClientRpc] private void RPCEndWave() {
        if(!isClientOnly) {
            return;
        }
        Debug.Log("Client Call, end");
        EndWave();
    }

//***************END WAVE NETCODE***************\\

    private int minionsPerWave() {
        return Mathf.RoundToInt((float)(baseCount *Math.Pow(waveCount, scalingFactor)));
    }

    void cmdSpawn(Vector3 pos, Quaternion rot) {
        if(!isServer) {//minions only on server
            return;
        }
        GameObject newBorn=Instantiate(spawnees[idex]);
        BasicMinionMovement move =newBorn.GetComponent<BasicMinionMovement>();
        move.setTarget(spire.transform);
        move.targetGameobject=spire;
        newBorn.transform.SetPositionAndRotation(pos, rot);
        NetworkServer.Spawn(newBorn);
    }

    public GameObject[] GetPrefabs() {
        return spawnablePrefabs;
    }

    private void TimeConversion(float inputTime) {
        float minutes = Mathf.FloorToInt(inputTime / 60);  
        float seconds = Mathf.FloorToInt(inputTime % 60);
        displayTimerD.text=string.Format("{0:00}:{1:00}", minutes, seconds);
        displayTimerA.text=string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
