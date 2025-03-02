using Unity.Netcode;
using UnityEngine;

public class Grappling : NetworkBehaviour
{
    [SerializeField] private Agent PlayerMovement;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappable;
    public LineRenderer lr;

    public float maxGrappleDistance;

    public float grappleDelayTime;
    private Vector3 grapplePoint;

    public float grapplingCd;

    private float grapplingCdTimer;

    public KeyCode grappleKey = KeyCode.Mouse0;
    private bool grappling;
    public float overshootYAxis;


    void Update()
    {
        if(!IsOwner) return;
        if(Input.GetKeyDown(grappleKey) && PlayerMovement.canGrapple) StartGrapple();
        if(grapplingCdTimer > 0) grapplingCdTimer -= Time.deltaTime;
    }

    void LateUpdate()
    {
        if(!IsOwner) return;
        if(grappling) lr.SetPosition(0, gunTip.position);
    }

    void StartGrapple()
    {
         if(grapplingCdTimer > 0) return;
        
        grappling = true;
        PlayerMovement.freeze = true;

        RaycastHit hit;
        if(Physics.Raycast(PlayerMovement.gameObject.transform.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappable)){
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint =  PlayerMovement.gameObject.transform.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);

        }
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);

        EnableLrServerRpc();
    }
    [ServerRpc]
    void EnableLrServerRpc(){
        EnableLrClientRpc();
    }
    [ClientRpc]
    void EnableLrClientRpc(){
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }
    [ServerRpc]
    void DisableLrServerRpc(){
        DisableLrClientRpc();
    }
    [ClientRpc]
    void DisableLrClientRpc(){
        lr.enabled = false;
    }
    void ExecuteGrapple(){
        PlayerMovement.freeze = false;
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        PlayerMovement.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);


    }
    public void StopGrapple(){
        PlayerMovement.freeze = false;
        grappling = false;
        grapplingCdTimer = grapplingCd;

        DisableLrServerRpc();
        lr.enabled = false;

    }

}
