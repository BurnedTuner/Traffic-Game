using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class StateManager : MonoBehaviour
{
    [SerializeField] private string _stateName;
    private List<ISaveLoadDependant> _saveloadObjects;
    private StateData _stateData;
    private FileDataHandler _fileHandler;


    public static StateManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("More than one instance of StateManager!");
        }

        Instance = this;
    }

    private void Start()
    {
        _fileHandler = new FileDataHandler(Application.dataPath, _stateName);
        _saveloadObjects = FindAllSaveLoadObjects();
    }
    public void NewState()
    {
        if (_stateName != "")
            _stateData = new StateData(_stateName);
        else
            _stateData = new StateData();
    }

    [ContextMenu("Load")]
    public void LoadState()
    {
        _stateData = _fileHandler.Load();
        if(_stateData == null)
        {
            Debug.Log("No Data Found on LoadState. Initializing new state");
            NewState();
        }

        foreach(ISaveLoadDependant saveLoadObject in _saveloadObjects)
        {
            saveLoadObject.LoadData(_stateData);
        }
    }

    [ContextMenu("Save")]
    public void SaveState()
    {
        if (_stateData == null)
        {
            NewState();
        }

        foreach (ISaveLoadDependant saveLoadObject in _saveloadObjects)
        {
            saveLoadObject.SaveData(ref _stateData);
        }

        _fileHandler.Save(_stateData);
    }

    private List<ISaveLoadDependant> FindAllSaveLoadObjects()
    {
        IEnumerable<ISaveLoadDependant> saveLoadObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveLoadDependant>();

        return new List<ISaveLoadDependant>(saveLoadObjects);
    }
}
