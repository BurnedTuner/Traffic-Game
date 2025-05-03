using UnityEngine;

public interface ISaveLoadDependant
{
    void LoadData(StateData stateData);
    void SaveData(ref StateData stateData);
}
