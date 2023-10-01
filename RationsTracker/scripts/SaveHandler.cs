using Godot;

namespace Handlers
{
    public static class SaveLoadHandler
    {
        static public void SaveSet(Godot.Collections.Array<Portion> portions, string name = "")
        {
			PortionsSetRes portionsSetRes = new PortionsSetRes();
			portionsSetRes.SetName = name;

			foreach (Portion portion in portions)
			{
				portionsSetRes.PortionsResList.Add(portion.Info);
			}
			string file_path = System.IO.Path.Combine(Globals.Paths.SaveSetsPlot,$"set_{name}.tres");
			ResourceSaver.Save(portionsSetRes, file_path);
        }
        static public PortionsSetRes LoadSet(string name)
        {
			string file_path = System.IO.Path.Combine(Globals.Paths.SaveSetsPlot,$"set_{name}.tres");
			if (!ResourceLoader.Exists(file_path))
			{
				return null;	
			}

        	PortionsSetRes portionsSetRes = (PortionsSetRes)ResourceLoader.Load(file_path, cacheMode:ResourceLoader.CacheMode.Ignore);
			return portionsSetRes;
		}
        static public ConfigFile LoadMainConfig()
        {
			string configFilePath = System.IO.Path.Combine(Globals.Paths.SaveConfigs, $"main_config.cfg");
			ConfigFile config = new ConfigFile();
			Error err = config.Load(configFilePath);
			if (err != Error.Ok) { return null; }

            return config;
		}
        static public void CreateMainConfig()
        {
			if (DirAccess.Open(Globals.Paths.SaveConfigs) != null)
			{
				return;
			}
			DirAccess dirAccess = DirAccess.Open("user://");
			dirAccess.MakeDir("configs");

			string configFilePath = System.IO.Path.Combine(Globals.Paths.SaveConfigs, $"main_config.cfg");
			ConfigFile config = new ConfigFile();
			
			
			config.Save(configFilePath);
		}
        static public void CreateSaveSetsDir()
        {
			if (DirAccess.Open(Globals.Paths.SaveSetsPlot) != null)
			{
				return;
			}
			DirAccess dirAccess = DirAccess.Open("user://");
			dirAccess.MakeDir("sets");
		}
    }

}