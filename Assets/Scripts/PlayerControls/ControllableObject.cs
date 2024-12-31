using UnityEngine;

public class ControllableObject : MonoBehaviour
{
    public string Id;
    public Camera childCamera;

    public Transform bodyTransform;

    //MAKE INTO STATIC UTIL FUNCTION

    private Outline AddOutlineToObjectOrGetOutline(GameObject objectToOutline)
    {
        if(objectToOutline.TryGetComponent<Outline>(out Outline outline)){
            return outline;
        }
        else{
            var line = objectToOutline.AddComponent<Outline>();
            return line;
        }
        
    }

    void OnMouseEnter()
    {
        // bool hasOutline = gameObject.TryGetComponent<Outline>(out Outline myOutline);
        // if(!hasOutline){
        //     myOutline = gameObject.AddComponent<Outline>();
        //     myOutline.OutlineMode = Outline.Mode.OutlineAll;
        //     myOutline.OutlineColor = Color.white;
        //     myOutline.OutlineWidth = 8f;
        // }
        var myOutline = AddOutlineToObjectOrGetOutline(gameObject);
        myOutline.OutlineMode = Outline.Mode.OutlineAll;
        myOutline.OutlineColor = Color.white;
        myOutline.OutlineWidth = 8f;
    }

    void OnMouseExit()
    {
        bool hasOutline = gameObject.TryGetComponent<Outline>(out Outline myOutline);
        if(hasOutline){
            Destroy(myOutline);
        }
    }

}
