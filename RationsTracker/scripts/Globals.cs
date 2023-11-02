using Godot;

namespace Globals
{
    public struct PackedScenes
    {
        public static PackedScene Portion = (PackedScene)ResourceLoader.Load("res://scenes/Portion.tscn");
    }

    public struct Paths
    {
        public static string SaveSetsPlot = "user://sets";
        public static string SaveConfigs = "user://configs";
    }

    public struct SetsData
    {
        public static Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Portion>> PortionsDict = 
            new Godot.Collections.Dictionary<string,Godot.Collections.Dictionary<string, Portion>>{};
        public static Godot.Collections.Dictionary<string, PortionsSetRes> PortionsSetResDict = 
            new Godot.Collections.Dictionary<string, PortionsSetRes>{};

        //TODO NON SERVE -> SONO LE KEYS DI DICT INTERNO DI PORTIONSDICT! 
        public static Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> PortionsTypesDict =
            new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>{};

        public static void AddSet(PortionsSetRes portionsSetRes)
        {
       
            PortionsSetResDict.Add(portionsSetRes.SetName, portionsSetRes);
            PortionsDict[portionsSetRes.SetName] = new Godot.Collections.Dictionary<string, Portion>{};
            PortionsTypesDict[portionsSetRes.SetName] = new Godot.Collections.Array<string>{};

            foreach(PortionRes portionRes in portionsSetRes.PortionsResList)
            {
                Portion portion = PackedScenes.Portion.Instantiate<Portion>();
                portion.Init(portionsSetRes.SetName, portionRes);
                PortionsDict[portionsSetRes.SetName].Add(portion.Info.PortionName, portion);
                PortionsTypesDict[portionsSetRes.SetName].Add(portion.Info.PortionName);
            }
        }
        public static void RemoveSet(string setName)
        {
            PortionsDict.Remove(setName);
            PortionsTypesDict.Remove(setName);
            PortionsSetResDict.Remove(setName);
        }
        public static void AddPortion(string setName, string type, Portion portion)
        {
            PortionsDict[setName].Add(type, portion);
            PortionsTypesDict[setName].Add(type);
            PortionsSetResDict[setName].PortionsResList.Add(portion.Info);
        }
        public static void RemovePortion(string setName, string type, Portion portion)
        {
            PortionsDict[setName].Remove(type);
            PortionsTypesDict[setName].Remove(type);
            PortionsSetResDict[setName].PortionsResList.Remove(portion.Info);

        }
        public static void ChangePortionName(string setName, string newType, string oldType)
        {
            PortionsDict[setName][newType] = PortionsDict[setName][oldType];
            PortionsDict[setName].Remove(oldType); 
            PortionsTypesDict[setName].Remove(oldType);
            PortionsTypesDict[setName].Add(newType);
        }
        public static void ChangeSetName(string newSetName, string oldSetName)
        {
            PortionsDict[newSetName] = PortionsDict[oldSetName];
            PortionsDict.Remove(oldSetName); 
            
            PortionsTypesDict[newSetName] = PortionsTypesDict[oldSetName];
            PortionsTypesDict.Remove(oldSetName); 
        }
        public static bool ContainsPortionType(string setName, string type)
        {
            return PortionsDict[setName].ContainsKey(type);
        }
        public static Godot.Collections.Array<Portion> GetPortions(string setName)
        {
            return (Godot.Collections.Array<Portion>)PortionsDict[setName].Values;
        }
    }



}