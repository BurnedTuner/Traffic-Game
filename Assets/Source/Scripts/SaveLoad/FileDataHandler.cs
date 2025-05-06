using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string _dataDirPath = "";

    private string _dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        _dataDirPath = dataDirPath;
        _dataFileName = dataFileName;
    }

    public StateData Load()
    {
        string fullpath = Path.Combine(_dataDirPath, _dataFileName);
        StateData loadedData = null;
        if(File.Exists(fullpath))
        {
            try 
            {
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullpath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<StateData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("Error Occured when trying to load data from file: " + fullpath + "\n" + e);
            }
        }

        Debug.Log("Loaded from: " + fullpath);
        return loadedData;
    }

    public StateData Load(string fileName)
    {
        _dataFileName = fileName;
        return Load();
    }

    public void Save(string fileName, StateData data)
    {
        _dataFileName = fileName;
        Save(data);
    }

    public void Save(StateData data)
    {
        string fullpath = Path.Combine(_dataDirPath, _dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullpath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(fullpath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
            Debug.Log("Saved to: " + fullpath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error while creating save file: " + fullpath + "\n" + e);
        }
    }
}
