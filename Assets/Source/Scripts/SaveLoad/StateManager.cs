using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEditor;

public class StateManager : MonoBehaviour
{
    [SerializeField] private string _stateName;
    [SerializeField] private StateData _stateData;
    [SerializeField] private List<GameObject> _selectiveSaveLoad = new List<GameObject>();
    private List<ISaveLoadDependant> _saveLoadObjects = new List<ISaveLoadDependant>();
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
        if (_selectiveSaveLoad.Count == 0)
            _saveLoadObjects = FindAllSaveLoadObjects();
        else
        {
            foreach (GameObject gameObject in _selectiveSaveLoad)
                _saveLoadObjects.AddRange(gameObject.GetComponents<MonoBehaviour>().OfType<ISaveLoadDependant>());
        }
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
        _stateData = _fileHandler.Load(_stateName);
        if(_stateData == null)
        {
            Debug.Log("No Data Found on LoadState. Initializing new state");
            NewState();
        }

        foreach(ISaveLoadDependant saveLoadObject in _saveLoadObjects)
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

        foreach (ISaveLoadDependant saveLoadObject in _saveLoadObjects)
        {
            saveLoadObject.SaveData(ref _stateData);
        }

        _fileHandler.Save(_stateName, _stateData);
    }

    private List<ISaveLoadDependant> FindAllSaveLoadObjects()
    {
        IEnumerable<ISaveLoadDependant> saveLoadObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveLoadDependant>();

        return new List<ISaveLoadDependant>(saveLoadObjects);
    }
}
