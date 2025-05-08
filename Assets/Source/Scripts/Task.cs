using UnityEngine;

[System.Serializable]
public class Task
{
    public Cell PickUpNode;
    public Cell DeliveryNode;
    public bool Complete = false;
    public bool PickedUp;
    public Agent AssignedAgent;
}
