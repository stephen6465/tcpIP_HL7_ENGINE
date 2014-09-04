using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Collections;
using System.Net;
using System.IO;



namespace AsyncSocketServer
{
    [Serializable()]
    [XmlRoot("Settings")]
   public class Settings
    {
        public Settings() { }

         [XmlElement("LocalIPAddress")]
         public String LocalIPAddress { get; set; }

         [XmlElement("LocalPort")]
         public String LocalPort { get; set; }

         [XmlElement("RemoteIPAddress")]
         public String RemoteIPAddress { get; set; }

         [XmlElement("RemotePort")]
         public String RemotePort { get; set; }
        
         [XmlElement("NumConnect")]
         public String NumConnect { get; set; }

         [XmlElement("OutFolderPath")]
         public String OutFolderPath { get; set; }

         [XmlElement("Type")]
         public String Type { get; set; }

         [XmlElement("InFolderPath")]
         public String InFolderPath { get; set; }

         [XmlElement("BufferSize")]
         public String BufferSize { get; set; }

         public void Serialize(String file, Settings c)
         {
             if (String.IsNullOrEmpty(c.NumConnect)){
                 //Put your defualt values here so we always instantiate with defualt values
                 c.BufferSize = "1500";
                 c.InFolderPath = "";
                 c.LocalIPAddress = Dns.GetHostName();
                 c.LocalPort = "3915";
                 c.OutFolderPath = "";
                 c.RemoteIPAddress = "0.0.0.0";
                 c.RemotePort = "3920";
                 c.Type = "SERVER";
                 c.NumConnect = "100";
             }

             System.Xml.Serialization.XmlSerializer xs
                = new System.Xml.Serialization.XmlSerializer(c.GetType());
             StreamWriter writer = File.CreateText(file);
             xs.Serialize(writer, c);
             writer.Flush();
             writer.Close();
         }
         public Settings Deserialize(string file)
         {
             System.Xml.Serialization.XmlSerializer xs
                = new System.Xml.Serialization.XmlSerializer(
                   typeof(Settings));
             StreamReader reader = File.OpenText(file);
             Settings c = (Settings)xs.Deserialize(reader);
             reader.Close();
             return c;
         }

    }
}
