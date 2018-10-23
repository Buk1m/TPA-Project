﻿using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization;
using MEFDefinitions;
using Model.Reflection.MetadataModels;

namespace Model
{
    [Export(typeof(ISerializer))]
    public class Serializer : ISerializer
    {
        private DataContractSerializer _serializer;

        public void Serialize<T>(T metadata)
        {
            //TODO: use DI to inject implementation through method based on config file??
            _serializer = new DataContractSerializer(metadata.GetType());
            using (FileStream stream = File.Create(@"Test.Xml"))
            {
                //TODO: error proof
                _serializer.WriteObject(stream, metadata);
            }
        }

        public T Deserialize<T>(string filename)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(AssemblyMetadata));
            T data;
            using (FileStream stream = File.OpenRead(filename))
            {
                data = (T)serializer.ReadObject(stream);
            }

            return data;
        }

    }
}