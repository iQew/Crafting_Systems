using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour {
    [SerializeField]
    private Camera ThirdPersonCamera;

    private List<Resource> _nearbyResources;
    private Resource _resourceCache;
    private Resource _priorityResource;
    private Resource _proximityResource;

    private float _timeAtLastProximityCheck;
    private const float PROXIMITY_CHECK_INTERVAL = 0.05f;

    // - - - - - -

    [SerializeField]
    private LayerMask ResourcePriorityLayer;

    [Range(0f, 15f)]
    public float Range = 4f;

    private RaycastHit[] _raycastHits;
    private Ray _ray;
    private Collider _previousClosestCollider;
    private Collider _closestCollider;

    private bool _rayEnteredCollider;

    private void Awake() {
        _nearbyResources = new List<Resource>();
        _raycastHits = new RaycastHit[5];
    }

    private void Update() {
        if (Time.time - _timeAtLastProximityCheck >= PROXIMITY_CHECK_INTERVAL) {
            // Shoots a ray from the center of the screen towards the scene. Resources being hit are getting prioritized over
            // the resources that are just in proximity of the player. Prioritizted resources are picked up first.
            _ray = ThirdPersonCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
            Debug.DrawRay(_ray.origin, _ray.direction * Range, Color.green, PROXIMITY_CHECK_INTERVAL, false);
            int hits = Physics.RaycastNonAlloc(_ray, _raycastHits, Range, ResourcePriorityLayer);
            if (hits > 0) {                
                if(_proximityResource) { // overwriting current nearest resource since raycast target has priority
                    _proximityResource.Active = false;
                    _proximityResource = null;
                }
                _closestCollider = GetClosestColliderOnRay(hits, _raycastHits);
                if (!_rayEnteredCollider) { // player is looking at a new collider compared to the last frame(s)
                    _rayEnteredCollider = true;
                    _resourceCache = _closestCollider.GetComponent<Resource>();
                    if (_resourceCache) {                        
                        _priorityResource = _resourceCache;
                        _priorityResource.Active = true;
                    }
                }               
            } else {
                // player is not looking at a specific resource, try to find nearest resource to grab
                _rayEnteredCollider = false;
                if(_priorityResource) { 
                    _priorityResource.Active = false; // player is no longer looking at prioritzed resource, so deactivating it
                    _priorityResource = null;
                }
                int count = _nearbyResources.Count;                
                if (count > 0) {
                    for (int i = 0; i < count; i++) {
                        _resourceCache = _nearbyResources[i];
                        if (_nearbyResources[i] == null) { // resource has been picked up and got deleted from world
                            _nearbyResources.RemoveAt(i);
                        } else {                            
                            _nearbyResources[i].Proximity = Vector3.Distance(_nearbyResources[i].transform.position, transform.position);
                            _nearbyResources[i].Active = false;
                        }
                    }
                    _nearbyResources = _nearbyResources.OrderBy(o => o.Proximity).ToList<Resource>();
                    if (_nearbyResources.Any()) {
                        _proximityResource = _nearbyResources[0];
                        _proximityResource.Active = true;
                    }
                }
            }
            _timeAtLastProximityCheck = Time.time;
        }
    }

    private Collider GetClosestColliderOnRay(int hits, RaycastHit[] raycastHits) {
        if (hits > 0) {
            _closestCollider = raycastHits[0].collider;
            for (int i = 0; i < hits; i++) {
                float currentDistance = Vector3.Distance(_closestCollider.transform.position, transform.position);
                float distance = Vector3.Distance(raycastHits[i].transform.position, transform.position);
                if (distance < currentDistance) {
                    _closestCollider = raycastHits[i].collider;
                }
            }
        }
        return _closestCollider;
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("KB: other: " + other.name + " || me: " + tag);
        if (other.CompareTag(TagHelper.RESOURCE_NODE)) {
            _resourceCache = other.GetComponent<Resource>();
            if (_resourceCache != null && !_nearbyResources.Contains(_resourceCache)) {
                _nearbyResources.Add(_resourceCache);
            }
            Debug.Log("KB: added resource node to list.");
        }
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log("KB: exited trigger: " + other.name);
        if (other.CompareTag(TagHelper.RESOURCE_NODE)) {
            _resourceCache = other.GetComponent<Resource>();
            if (_resourceCache != null && _nearbyResources.Contains(_resourceCache)) {
                _nearbyResources.Remove(_resourceCache);
            }
            _resourceCache.Active = false;
            Debug.Log("KB: removed resource node to list.");
        }
    }

    public void OnResourceDestruction(ItemDataContainer droppedItem) {
        Debug.Log("KB: Adding " + droppedItem.Quantity + " " + droppedItem.Name + " to PlayerData");
        //PlayerData.AddItem(droppedItem);
    }
}
