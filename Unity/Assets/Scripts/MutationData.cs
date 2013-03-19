using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

//=====================================================================================================================================//
//=====================================================================================================================================//
//=====================================================================================================================================//
[System.Serializable]
public class MutationLevel
{
    public string Name = "";
	public int TotalItems;
	public int CollectedItems;
	public bool Completed;
}

//=====================================================================================================================================//
//=====================================================================================================================================//
//=====================================================================================================================================//
[System.Serializable]
public class MutationStats
{
    public float StartTime;
    public float TotalPlayTime;
    public float SessionTime;
    public float AverageSessionTime; 
    public int PlayCount = 0;

    //=====================================================================================================================================//
    void Awake()
    {
        StartTime = Time.time;
    }

    //=====================================================================================================================================//
    void Update()
    {
        SessionTime = Time.time - StartTime;
        AverageSessionTime = (AverageSessionTime * PlayCount + SessionTime) / PlayCount + 1;
        PlayCount++;
    }
}

//=====================================================================================================================================//
//=====================================================================================================================================//
//=====================================================================================================================================//
[System.Serializable]
public class MutationSettings
{
    public float MusicVolume = 1;
    public bool MusicMute = false;
    public float SoundVolume = 1;
    public bool SoundMute = false;
    public int CurrentTrack = 0;
}

//=====================================================================================================================================//
//=====================================================================================================================================//
//=====================================================================================================================================//
[XmlRoot("Mutation"), System.Serializable]
public class MutationData
{
	static public string DataPath = Application.persistentDataPath + "/Mutation.xml";
	
    public MutationSettings Settings = new MutationSettings();	
	public string CurrentLevel = "";
	public string CurrentCheckPoint = "";
	public int Score;
	
    [XmlArray("Levels"), XmlArrayItem("Level")]
    public List<MutationLevel> Levels = new List<MutationLevel>();

    //=====================================================================================================================================//
    public void Save()
    {
        Debug.Log("Saved to File: " + DataPath);
        
        XmlSerializer serializer = new XmlSerializer(typeof(MutationData));
        using (var stream = new FileStream(DataPath, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    //=====================================================================================================================================//
    public static MutationData Load()
    {
        DataPath = Application.persistentDataPath + "/Mutation.xml";

        if (File.Exists(DataPath))
        {
			Debug.Log("Loaded from File: " + DataPath);
        	//InhumanIOS.Popup("Loaded Data", "From File: " + path, "OK");
			
			XmlSerializer serializer = new XmlSerializer(typeof(MutationData));

	        using (var stream = new FileStream(DataPath, FileMode.Open))
	        {
	            return serializer.Deserialize(stream) as MutationData;
	        }

            // Unreachable code.
			//Debug.LogError("Couldn't Load Data");
            //return new MutationData();
        }
        else
        {
			Debug.Log("Loaded New");
            return new MutationData();
        }       
    }
}
