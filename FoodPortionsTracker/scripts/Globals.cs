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
        public static Godot.Collections.Dictionary<string, Portion> PortionsDict = new Godot.Collections.Dictionary<string, Portion>{};
        public static Godot.Collections.Array<string> AllTypes = new Godot.Collections.Array<string> {};

        public static void AddPortion(string type, Portion portion)
        {
            PortionsDict.Add(type, portion);
            AllTypes.Add(type);
        }
        public static void RemovePortion(string type)
        {
            PortionsDict.Remove(type);
            AllTypes.Remove(type);
        }
    }



}