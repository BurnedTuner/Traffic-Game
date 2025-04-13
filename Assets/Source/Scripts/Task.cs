using UnityEngine;

[System.Serializable]
public class Task
{
    public Cell PickUpNode;
    public Cell DeliveryNode;
    public bool Complete;
    public bool PickedUp;
    public Agent AssignedAgent;
}
