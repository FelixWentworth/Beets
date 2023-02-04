using UnityEngine;

public class WorldRaycastHandler : MonoBehaviour
{
    private Camera _mainCam;
    [SerializeField] private LayerMask _potsLayer;
    void Awake()
    {
        _mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (VegPlacer.IsPlacingVeg)
        {
            return;
        }

        CheckForInteractions();


    }
    
    void CheckForInteractions()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var mousePos = Input.mousePosition;
            var ray = _mainCam.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out hit, 999f, _potsLayer);
            if (hit.collider != null)
            {
                var pot = hit.collider.GetComponent<Pot>();
                if (pot != null)
                {
                    var potGridPos = pot.GridPos;
                    GameManager.InteractWithPot(GameManager.GameAction.Harvest, potGridPos, "");
                }
            }
        }
    }
}
