using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour {

    [SerializeField]
    private SphereCollider _sphereCollider;

    private List<Resource> _nearbyNodes;
    private Resource _resourceNodeCache;

    private float _timeAtLastProximityCheck;
    private const float PROXIMITY_CHECK_INTERVAL = 0.05f;

    private void Awake() {
        _nearbyNodes = new List<Resource>();
    }

    private void Update() {
        if(_nearbyNodes.Count > 0) {
            if(Time.time - _timeAtLastProximityCheck >= PROXIMITY_CHECK_INTERVAL) {
                _timeAtLastProximityCheck = Time.time;
                int count = _nearbyNodes.Count;
                for (int i = 0; i < count; i++) {
                    _resourceNodeCache = _nearbyNodes[i];
                    if(_nearbyNodes[i] == null) { // resource has been picked up and got deleted from world
                        _nearbyNodes.RemoveAt(i);
                    } else {
                        float proximity = (_nearbyNodes[i].transform.position - transform.position).magnitude;
                        _nearbyNodes[i].Proximity = proximity;
                        _nearbyNodes[i].Active = false;
                    }                    
                }
                _nearbyNodes = _nearbyNodes.OrderBy(o => o.Proximity).ToList<Resource>();
            }
            if(_nearbyNodes.Any()) {
                _nearbyNodes[0].Active = true;
            }            
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("KB: other: " + other.name + " || me: " + tag);
        if(other.CompareTag(TagHelper.RESOURCE_NODE)) {
            _resourceNodeCache = other.GetComponent<Resource>();
            if(_resourceNodeCache != null && !_nearbyNodes.Contains(_resourceNodeCache)) {
                _nearbyNodes.Add(_resourceNodeCache);
            }            
            Debug.Log("KB: added resource node to list.");
        }
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log("KB: exited trigger: " + other.name);
        if(other.CompareTag(TagHelper.RESOURCE_NODE)) {
            _resourceNodeCache = other.GetComponent<Resource>();
            if(_resourceNodeCache != null && _nearbyNodes.Contains(_resourceNodeCache)) {
                _nearbyNodes.Remove(_resourceNodeCache);
            }
            _resourceNodeCache.Active = false;
            Debug.Log("KB: removed resource node to list.");
        }
    }
}
