using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using External;
using Kaitai;
using ScriptableObj;
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
            
            w.Write(Encoding.UTF8.GetBytes(paddedName)); // name
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

            // predefined 

            List<string> allyRefs = new List<string>();
            foreach (var saveDataAlly in DataController.saveData.allies)
            {
                if (saveDataAlly != null && saveDataAlly.ally && !allyRefs.Contains(saveDataAlly.ally.name))
                    allyRefs.Add(saveDataAlly.ally.name);
            }
            
            foreach (var saveDataAlly in DataController.saveData.availableAllies)
            {
                if (saveDataAlly != null && saveDataAlly.ally && !allyRefs.Contains(saveDataAlly.ally.name))
                    allyRefs.Add(saveDataAlly.ally.name);
            }
            
            w.Write((byte)(allyRefs.Count));
            foreach (var allyRef in allyRefs)
            {
                w.Write(Encoding.UTF8.GetBytes(allyRef.PadRight(16, '\0')));
            }
            
            List<string> abilityRefs = new List<string>();
            foreach (var saveDataAlly in DataController.saveData.abilities)
            {
                if (saveDataAlly != null && !abilityRefs.Contains(saveDataAlly.Type.ToString().Capitalize()+"s/"+saveDataAlly.name))
                    abilityRefs.Add(saveDataAlly.Type.ToString().Capitalize()+"s/"+saveDataAlly.name);
            }
            
            foreach (var saveDataAlly in DataController.saveData.availableAbilities)
            {
                if (saveDataAlly != null && !abilityRefs.Contains(saveDataAlly.Type.ToString().Capitalize()+"s/"+saveDataAlly.name))
                    abilityRefs.Add(saveDataAlly.Type.ToString().Capitalize()+"s/"+saveDataAlly.name);

            }
            
            w.Write((byte)(abilityRefs.Count));
            foreach (var allyRef in abilityRefs)
            {
                w.Write(Encoding.UTF8.GetBytes(allyRef.PadRight(32, '\0')));
            }
            
            List<string> clothRefs = new List<string>();
            foreach (var saveDataAlly in DataController.saveData.availableClothing)
            {
                if (saveDataAlly != null && !clothRefs.Contains(saveDataAlly.name))
                    clothRefs.Add(saveDataAlly.name);
            }
            

            w.Write((byte)(clothRefs.Count));
            foreach (var allyRef in clothRefs)
            {
                w.Write(Encoding.UTF8.GetBytes(allyRef.PadRight(16, '\0')));
            }
            
            List<string> enmRefs = new List<string>();
            foreach (var saveDataAlly in DataController.saveData.enemyInventory)
            {
                if (saveDataAlly != null && !enmRefs.Contains(saveDataAlly.name))
                    enmRefs.Add(saveDataAlly.name);
            }
            

            w.Write((byte)(enmRefs.Count));
            foreach (var allyRef in enmRefs)
            {
                w.Write(Encoding.UTF8.GetBytes(allyRef.PadRight(16, '\0')));
            }

            // data
            foreach (var saveDataAlly in DataController.saveData.allies)
            {
                if (saveDataAlly != null && saveDataAlly.ally)
                    w.Write((sbyte)allyRefs.IndexOf(saveDataAlly.ally.name));
                else
                    w.Write((sbyte)-1);
            }
            
            foreach (var saveDataAlly in DataController.saveData.abilities)
            {
                if (saveDataAlly != null)
                {
                    int abilityIndex = -1;
                    
                    foreach (var abilityRef in abilityRefs)
                    {
                        if (abilityRef.EndsWith(saveDataAlly.name))
                        {
                            abilityIndex = abilityRefs.IndexOf(abilityRef);
                            break;

                            ;
                        }
                    }
                    
                    w.Write((sbyte)abilityIndex);

                    int upgradeIndex = -1;
                    if (DataController.saveData.upgrades.Count > 0)
                    {
                        foreach (var saveDataUpgrade in DataController.saveData.upgrades)
                        {
                            if (saveDataUpgrade != null)
                            {
                                if (saveDataAlly.Upgrades.Contains(saveDataUpgrade))
                                {
                                    upgradeIndex = saveDataAlly.Upgrades.IndexOf(saveDataUpgrade);
                                    break;
                                }
                            }
                        }
                    }

                    w.Write((sbyte)upgradeIndex);
                }
                else
                {
                    w.Write((sbyte)-1);
                    w.Write((sbyte)-1);
                }
            }
            
            if (DataController.saveData.HairObject) w.Write((sbyte)clothRefs.IndexOf(DataController.saveData.HairObject.name)); else w.Write((sbyte)-1);
            if (DataController.saveData.HatObject) w.Write((sbyte)clothRefs.IndexOf(DataController.saveData.HatObject.name)); else w.Write((sbyte)-1);
            if (DataController.saveData.ShirtObject) w.Write((sbyte)clothRefs.IndexOf(DataController.saveData.ShirtObject.name)); else w.Write((sbyte)-1);
            if (DataController.saveData.PantsObject) w.Write((sbyte)clothRefs.IndexOf(DataController.saveData.PantsObject.name)); else w.Write((sbyte)-1);
            if (DataController.saveData.ShoesObject) w.Write((sbyte)clothRefs.IndexOf(DataController.saveData.ShoesObject.name)); else w.Write((sbyte)-1);
            
            w.Write((ushort)DataController.saveData.enemyInventory.Count);
            foreach (var enemyObject in DataController.saveData.enemyInventory)
            {
                w.Write((sbyte)enmRefs.IndexOf(enemyObject.name));
            }
            
            var path = Application.dataPath + "/data.cacs";
            File.WriteAllBytes(path, mem.ToArray());
            
            PlayerPrefs.SetString("cac_sd",Convert.ToBase64String(mem.ToArray()));
            PlayerPrefs.Save();
            
            Debug.Log(path);
        }

        public static void Load(string value)
        {
            var data = Convert.FromBase64String(value);

            var reader = new CacSave(new KaitaiStream(data));

            DataController.saveData.Name = reader.Header.Name;
            DataController.saveData.SkinColor = reader.Header.SkinColor.ReadColor();
            DataController.saveData.HairColor = reader.Header.HairColor.ReadColor();

            DataController.saveData.GlassCount = reader.Stats.GlassCount;
            DataController.saveData.PlasticCount = reader.Stats.PlasticCount;
            DataController.saveData.MetalCount = reader.Stats.MetalCount;
            
            DataController.saveData.GlassBitCount = reader.Stats.GlassCountBit;
            DataController.saveData.PlasticBitCount = reader.Stats.PlasticCountBit;
            DataController.saveData.MetalBitCount = reader.Stats.MetalCountBit;
            DataController.saveData.ClothingScrapCount = reader.Stats.ClothScrapCount;

            var allies = reader.Defs.Allies.Select((s => new AllyObject.AllyInstance(Resources.Load<AllyObject>("Settings/Allies/" + s)))).ToList();
            DataController.saveData.availableAllies = allies;
            
            var abilities = reader.Defs.Abilities.Select((s => Resources.Load<AbilityObject>("Settings/Abilities/" + s))).ToList();
            DataController.saveData.availableAbilities = abilities;

            var clothing = reader.Defs.Clothing.Select((s => Resources.Load<ClothingObject>("Settings/Clothing/" + s))).ToList();
            DataController.saveData.availableClothing = clothing;
            
            var enemies = reader.Defs.Enemies.Select((s => Resources.Load<EnemyObject>("Settings/Enemies/" + s))).ToList();
            
            
            DataController.saveData.allies.Clear(); 
            DataController.saveData.abilities.Clear(); 
            DataController.saveData.upgrades.Clear();
            
            for (var i = 0; i < reader.Data.Allies.Count; i++)
            {
                if (reader.Data.Allies[i] > -1)
                    DataController.saveData.allies.Add(allies[reader.Data.Allies[i]]);
                else
                    DataController.saveData.allies.Add(null);
            }
            
            for (var i = 0; i < reader.Data.Abilities.Count; i++)
            {
                var ability = reader.Data.Abilities[i];
                if (ability.AbilityIndex > -1)
                {
                    DataController.saveData.abilities.Add(abilities[ability.AbilityIndex]); 
                    if (ability.UpgradeIndex > -1)
                        DataController.saveData.upgrades.Add(abilities[ability.AbilityIndex].Upgrades[ability.UpgradeIndex]);
                    else
                    {
                        DataController.saveData.upgrades.Add(null);
                    }
                }
                else
                {
                    DataController.saveData.abilities.Add(null);
                    DataController.saveData.upgrades.Add(null);
                }
            }

            DataController.saveData.HairObject = reader.Data.Hair > -1 ? clothing[reader.Data.Hair] : null;
            DataController.saveData.HatObject = reader.Data.Hat > -1 ? clothing[reader.Data.Hat] : null;
            DataController.saveData.ShirtObject = reader.Data.Shirt > -1 ? clothing[reader.Data.Shirt] : null;
            DataController.saveData.PantsObject = reader.Data.Pants > -1 ? clothing[reader.Data.Pants] : null;
            DataController.saveData.ShoesObject = reader.Data.Shoes > -1 ? clothing[reader.Data.Shoes] : null;

            DataController.saveData.enemyInventory =
                reader.Data.Enemies.Select((arg => arg > -1 ? enemies[arg] : null)).ToList();
            
            

        }
        
        private static void WriteColor(this BinaryWriter w, Color c)
        {
            w.Write(c.r);
            w.Write(c.g);
            w.Write(c.b);
        }

        private static Color ReadColor(this Kaitai.CacSave.Color c)
        {
            return new Color((float)c.R, (float)c.G, (float)c.B, 1);
        }
    }
}