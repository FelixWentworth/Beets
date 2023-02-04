using System;
using UnityEngine;

public class VegPlacer : MonoBehaviour
{
    public static bool IsPlacingVeg { get; private set; }
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _plantableLayer;
    [SerializeField] private float _fingerYOffset;
    [SerializeField] private float _offsetFromGround;
    private Camera _mainCam;
    private GameObject _currentSpawnedVeg;
    private Pot previousPot;
    private SO_Veg _currentVeg;
    
    private void Awake()
    {
        _mainCam = Camera.main;
        InventoryBtn.OnSelected += OnVegSelected;
        InventoryBtn.OnDragEnd += OnDragEnd;
    }

    private void OnDestroy()
    {
        InventoryBtn.OnSelected -= OnVegSelected;
        InventoryBtn.OnDragEnd -= OnDragEnd;
    }

    private void Update()
    {
        if (IsPlacingVeg)
        {
            _currentSpawnedVeg.transform.position = GetNewDragPos();
            HighlightingAssist();
        }
    }
    
    void OnVegSelected(SO_Veg veg)
    {
        _currentVeg = veg;
        _currentSpawnedVeg = Instantiate(veg.Prefab, GetNewDragPos(), Quaternion.identity);
        IsPlacingVeg = true;
    }
    
    Vector3 GetNewDragPos()
    {
        RaycastHit hit;
        var mousePos = Input.mousePosition;
        
        mousePos.y += _fingerYOffset;
        var ray = _mainCam.ScreenPointToRay(mousePos);
        Physics.Raycast(ray, out hit, 999f, _groundLayer);
        if (hit.collider != null)
        {
            var point = hit.point;
            point.y += _offsetFromGround;
            return point;
        }
        return Vector3.zero;
    }
    
    void HighlightingAssist()
    {
        // ray from the veg down and highlight a valid spot
        var rayStart = _currentSpawnedVeg.transform.position;
        Ray ray = new Ray(rayStart, Vector3.down);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 100f, _plantableLayer);
        if (hit.collider != null)
        {
            var thisPot = hit.collider.GetComponent<Pot>();
            if (thisPot != null)
            {
                // highlight
                thisPot.SetHighlight(thisPot.HasVeg == false);
                if (previousPot != null && previousPot != thisPot)
                {
                    previousPot.SetHighlight(false);
                }

                previousPot = thisPot;
            }
        }
    }
    
    private void OnDragEnd()
    {
        
        previousPot.SetHighlight(false);
        Destroy(_currentSpawnedVeg);
        _currentSpawnedVeg = null;
        IsPlacingVeg = false;
        
        if (previousPot.Active)
        {
            GameManager.InteractWithPot(GameManager.GameAction.Plant, previousPot.GridPos, _currentVeg.Name);
        }
    }



}
