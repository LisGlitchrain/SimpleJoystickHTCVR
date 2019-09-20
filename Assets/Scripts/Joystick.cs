using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Utility;

public class Joystick : MonoBehaviour
{
    GameObject handToTrack;
    public GameObject baseObject;
    public Collider leftHandCollider;
    public Collider rightHandCollider;
    List<Collider> colliders = new List<Collider>();
    Quaternion neutralRotation;
    Vector3 neutralPosition;
    [Range(-179, 0)]
    [SerializeField] float minX;
    [Range(0, 180)]
    [SerializeField] float maxX;
    [Range(-179, 0)]
    [SerializeField] float minY;
    [Range(0, 180)]
    [SerializeField] float maxY;
    Vector3 stickVector;

    public float Xpos { get; private set; }
    public float Ypos { get; private set; }

    float prevXPos;
    float prevYPos;

    // Start is called before the first frame update
    void Start()
    {
        neutralRotation = transform.rotation;
        neutralPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(handToTrack)
        {
            stickVector = handToTrack.transform.position - baseObject.transform.position;
            Ypos = Vector3.SignedAngle(Vector3.ProjectOnPlane(stickVector, baseObject.transform.forward), baseObject.transform.up, baseObject.transform.forward);
            Xpos = -Vector3.SignedAngle(Vector3.ProjectOnPlane(stickVector, baseObject.transform.right), baseObject.transform.up, baseObject.transform.right);            
            if (Xpos > 180) Xpos = -360 + Xpos;
            if (Ypos > 180) Ypos = -360 + Ypos;
            Xpos = Mathf.Clamp(Xpos, minX, maxX);
            Ypos = Mathf.Clamp(Ypos, minY, maxY);      
            print($"{handToTrack.gameObject.name} <color=red> around X: {Xpos}</color> <color=green> around Z: {Ypos}</color>");
            if (Xpos <= maxX && Xpos >= minX)
            {
                transform.RotateAround(baseObject.transform.position, baseObject.transform.right, Xpos - prevXPos);
                prevXPos = Xpos;
            }
            if (Ypos <= maxY && Ypos >= minY)
            {
                transform.RotateAround(baseObject.transform.position, -baseObject.transform.forward, Ypos - prevYPos);
                prevYPos = Ypos;
            }
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, neutralRotation, 0.1f);
            transform.position = Vector3.Lerp(transform.position, neutralPosition, 0.1f);
            stickVector = transform.position - baseObject.transform.position;
            prevYPos = Vector3.SignedAngle(Vector3.ProjectOnPlane(stickVector, baseObject.transform.forward), baseObject.transform.up, baseObject.transform.forward);
            prevXPos = -Vector3.SignedAngle(Vector3.ProjectOnPlane(stickVector, baseObject.transform.right), baseObject.transform.up, baseObject.transform.right);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print($"{other.gameObject.name} entered");
        colliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        print($"{other.gameObject.name} exited");
        if (handToTrack && other == handToTrack.GetComponent<Collider>())
            handToTrack = null;
        colliders.Remove(other);
    }

    public void RightTriggerPressed()
    {
        print($"RIGHT PRESSED");
        if (colliders.Contains(rightHandCollider) && !handToTrack)
            handToTrack = rightHandCollider.gameObject;
    }

    public void RightTriggerReleased()
    {
        if (handToTrack = rightHandCollider.gameObject)
            handToTrack = null;
    }

    public void LeftTriggerReleased()
    {
        print($"LEFT PRESSED");
        if (handToTrack = leftHandCollider.gameObject)
            handToTrack = null;
    }

    public void LeftTriggerPressed()
    {
        if (colliders.Contains(leftHandCollider) && !handToTrack)
            handToTrack = leftHandCollider.gameObject;       
    }    
}
