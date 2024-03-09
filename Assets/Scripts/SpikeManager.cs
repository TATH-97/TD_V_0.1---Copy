using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpikeManager : NetworkBehaviour
{
    public List<Spike> spikeList;
    public int level=1;
    public int maxLevel=3;
    [SerializeField] GameObject spikePrefab;

    public void Inst() {
        spikeList= new List<Spike>();
    }

//******************SPIKE_SPAWN******************\\
    public void AddSpike(Transform pos) {
        // if(isClientOnly) {
        //     AddSpikeCL(pos);
        // } else {
        //     SpikeGen(pos);
        //     CmdPlaceSpike(pos);
        // }
        AddSpikeCL(pos);
    }

    private void SpikeGen(Transform pos) {
        if(GetCount()>=level) {
            Debug.Log("Reached Max Num of spikes");
            return;
        }
        Debug.Log("BUILDING");
        GameObject temp=Instantiate(spikePrefab);
        temp.transform.position=pos.position;
        Spike spike=temp.GetComponent<Spike>();
        spikeList.Add(spike);
        spike.roundSet=LevelManager.instance.waveCount;
        spike.roundsWillLast=level;
    }

    [Client] private void AddSpikeCL(Transform pos) {
        if(!isLocalPlayer) return;
        Debug.Log("SPIKECL");
        SpikeGen(pos);
        CmdPlaceSpike(pos);
    }   

    [Command(requiresAuthority =false)] private void CmdPlaceSpike(Transform pos) {
        // if(GetCount()>=level) {
        //     Debug.Log("Reached Max Num of spikes");
        //     return;
        // }
        // Debug.Log("CMD BUILDING");
        // GameObject temp=Instantiate(spikePrefab);
        // temp.transform.position=pos.position;
        // Spike spike=temp.GetComponent<Spike>();
        // spikeList.Add(spike);
        // spike.roundSet=LevelManager.instance.waveCount;
        // spike.roundsWillLast=level;
        // NetworkServer.Spawn(temp);
        SpikeGen(pos);
        RPCSpike(pos);
    }

    [ClientRpc] private void RPCSpike(Transform pos) {
        if(isLocalPlayer) return;
        SpikeGen(pos);
    }
    //******************SPIKE_SPAWN******************\\

    public void CheckSpikes() {
        foreach(Spike spike in spikeList) {
            CheckExpiration(spike);
        }
    }

    private void CheckExpiration(Spike spike) {
        if(LevelManager.instance.waveCount>=(spike.roundSet+spike.roundsWillLast)) {
            spikeList.Remove(spike);
            ClientDestroy(spike.gameObject);
        }
    }

    [Command(requiresAuthority =false)] private void ClientDestroy(GameObject spikePrefab) {
        RPCDestroy(spikePrefab);
        NetworkServer.Destroy(spikePrefab);
    } 

    [ClientRpc] private void RPCDestroy(GameObject spikePrefab) {
        Destroy(spikePrefab);
    }

    public int GetCount() {
        Debug.Log(spikeList.Count.ToString());
        return spikeList.Count;
    }

    public void LevelUp() {
        if(level<maxLevel) {
            level++;
        }
    }

}
