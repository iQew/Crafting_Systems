using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class InteractivityManager : MonoBehaviour {

    [SerializeField]
    private Camera ThirdPersonCamera;

    private List<CollectibleResource> _nearbyResources;
    private CollectibleResource _resourceCache;
    private CollectibleResource _currentlyActiveResource;
    private CollectibleResource _previousPriorityResource;

    private float _timeAtLastProximityCheck;
    private const float PROXIMITY_CHECK_INTERVAL = 0.05f;

    [SerializeField]
    private LayerMask ResourcePriorityLayer;

    [Range(0f, 6f)]
    public float Range = 4.5f;

    private RaycastHit[] _raycastHits;
    private Ray _ray;
    private Collider _closestColliderOnRay;

    private bool _isPriorityCheckRequired = true;

    private void Awake() {
        _nearbyResources = new List<CollectibleResource>();
        _raycastHits = new RaycastHit[5];
    }

    private void Update() {
        if (Time.time - _timeAtLastProximityCheck >= PROXIMITY_CHECK_INTERVAL) {
            if (_currentlyActiveResource) {
                _currentlyActiveResource.Active = false;
            }
            _ray = ThirdPersonCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
            int raycastHitAmount = Physics.RaycastNonAlloc(_ray, _raycastHits, Range, ResourcePriorityLayer);
            if (raycastHitAmount > 0) {
                // checking if we are looking at the same resource as last frame
                // to avoid using getcomponent on every frame
                if (_isPriorityCheckRequired) {
                    _isPriorityCheckRequired = false;
                    if (TryGetResourceByPriority(raycastHitAmount, out _currentlyActiveResource)) {
                        _currentlyActiveResource.Active = true;
                    }
                } else {
                    _previousPriorityResource.Active = true;
                }
            } else {
                if (TryGetResourceByProximity(out _currentlyActiveResource)) {
                    _currentlyActiveResource.Active = true;
                } else {
                    _currentlyActiveResource = null;
                }
                _isPriorityCheckRequired = true;
            }
            _timeAtLastProximityCheck = Time.time;
        }
    }

    /**
     * Player is not looking at a specific resource, try to find nearest resource to grab
     **/
    private bool TryGetResourceByProximity(out CollectibleResource proximityResource) {
        proximityResource = null;
        int count = _nearbyResources.Count;
        if (count > 0) {
            for (int i = 0; i < count; i++) {
                _resourceCache = _nearbyResources[i];
                _resourceCache.Proximity = Vector3.Distance(_resourceCache.transform.position, transform.position);
                _resourceCache.Active = false;
            }
            _nearbyResources = _nearbyResources.OrderBy(o => o.Proximity).ToList<CollectibleResource>();
            proximityResource = _nearbyResources[0];
            return true;
        }
        return false;
    }

    private bool TryGetResourceByPriority(int hits, out CollectibleResource priorityResource) {
        priorityResource = null;
        _closestColliderOnRay = GetClosestColliderOnRay(hits, _raycastHits);
        if (_closestColliderOnRay.TryGetComponent<CollectibleResource>(out priorityResource)) {
            _previousPriorityResource = priorityResource;
            return true;
        }
        return false;
    }

    public void PickUpActiveResource() {        
        if (_currentlyActiveResource != null) {
            if(Player.Instance.PickupResource(_currentlyActiveResource)) {
                _currentlyActiveResource.PickUp();
                _nearbyResources.Remove(_currentlyActiveResource);
            } else {
                //Debug.Log("KB: Pickup failed.");
            }
        } else {
            //Debug.Log("KB: There is nothing to pick up.");
        }
    }

    private Collider GetClosestColliderOnRay(int hits, RaycastHit[] raycastHits) {
        if (hits > 0) {
            _closestColliderOnRay = raycastHits[0].collider;
            for (int i = 0; i < hits; i++) {
                float currentDistance = Vector3.Distance(_closestColliderOnRay.transform.position, transform.position);
                float distance = Vector3.Distance(raycastHits[i].transform.position, transform.position);
                if (distance < currentDistance) {
                    _closestColliderOnRay = raycastHits[i].collider;
                }
            }
        }
        return _closestColliderOnRay;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<CollectibleResource>(out _resourceCache)) {
            if (!_nearbyResources.Contains(_resourceCache)) {
                _nearbyResources.Add(_resourceCache);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent<CollectibleResource>(out _resourceCache)) {
            if (_nearbyResources.Contains(_resourceCache)) {
                _nearbyResources.Remove(_resourceCache);
            }
            _resourceCache.Active = false;
        }
    }

}
