using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Inhuman
{
    [XmlRoot("BuildManager"), System.Serializable]
    public class BuildManagerData
    {
        static string DataPath = Application.dataPath + "/BuildManager.xml";
        public int Current = -1;
        public List<Build> Builds = new List<Build>();

        //=====================================================================================================================================//
        public void Save()
        {
            Debug.Log("Saved to File: " + DataPath);

            XmlSerializer serializer = new XmlSerializer(typeof(BuildManagerData));
            using (var stream = new FileStream(DataPath, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        //=====================================================================================================================================//
        public static BuildManagerData Load()
        {
            if (File.Exists(DataPath))
            {
                Debug.Log("Loaded from File: " + DataPath);
                XmlSerializer serializer = new XmlSerializer(typeof(BuildManagerData));

                using (var stream = new FileStream(DataPath, FileMode.Open))
                {
                    return serializer.Deserialize(stream) as BuildManagerData;
                }
            }
            else
            {
                return new BuildManagerData();
            }
        }
    }
}