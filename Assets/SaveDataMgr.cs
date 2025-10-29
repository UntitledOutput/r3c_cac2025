using System.IO;
using System.Text;
using Kaitai;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public static class SaveDataMgr
    {
        public static void Save()
        {
            MemoryStream mem = new MemoryStream();
            BinaryWriter w = new BinaryWriter(mem);
            
            w.Write(Encoding.ASCII.GetBytes("CACS"));
            
            // header
            string paddedName = DataController.saveData.Name.PadRight(16, '\0');
            
            w.Write(Encoding.ASCII.GetBytes(paddedName)); // name
            w.WriteColor(DataController.saveData.SkinColor); // skin color
            w.WriteColor(DataController.saveData.HairColor); // hair color
            
            // stats
            w.Write((ushort)(DataController.saveData.GlassCount)); // glass count
            w.Write((ushort)(DataController.saveData.PlasticCount)); // plastic count
            w.Write((ushort)(DataController.saveData.MetalCount)); // metal count
            
            w.Write((ushort)(DataController.saveData.GlassBitCount)); // cullet count
            w.Write((ushort)(DataController.saveData.PlasticBitCount)); // plastic bit count
            w.Write((ushort)(DataController.saveData.MetalBitCount)); // metal sheet count
            w.Write((ushort)(DataController.saveData.ClothingScrapCount)); // scrap count

            var path = Application.dataPath + "/data.cacs";
            File.WriteAllBytes(path, mem.ToArray());
            
            Debug.Log(path);
        }

        private static void WriteColor(this BinaryWriter w, Color c)
        {
            w.Write(c.r);
            w.Write(c.g);
            w.Write(c.b);
        }
    }
}