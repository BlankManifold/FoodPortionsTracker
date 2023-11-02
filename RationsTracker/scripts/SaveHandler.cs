using System.Linq;
using Godot;

namespace Handlers
{
	public static class SaveLoadHandler
	{
		static public Godot.Collections.Array<string> CreateSetsNameList()
		{
			string[] setsList = new string[] { };

			if (DirAccess.Open(Globals.Paths.SaveSetsPlot) != null)
			{
				DirAccess dirAccess = DirAccess.Open(Globals.Paths.SaveSetsPlot);
				setsList = dirAccess.GetFiles();
			}

			return new Godot.Collections.Array<string>(
				setsList.Select(
				fileName =>
					{
						int startIndex = fileName.IndexOf('_') + 1;
						int endIndex = fileName.LastIndexOf('.');
						string setName = fileName.Substring(startIndex, endIndex - startIndex);
						return setName;
					}
				)
			);
		}
		static public void SaveSet(PortionsSetRes portionsSetRes)
		{
			string file_path = System.IO.Path.Combine(Globals.Paths.SaveSetsPlot, $"set_{portionsSetRes.SetName}.tres");
			ResourceSaver.Save(portionsSetRes, file_path);
		}
		static public PortionsSetRes LoadSet(string name)
		{
			string file_path = System.IO.Path.Combine(Globals.Paths.SaveSetsPlot, $"set_{name}.tres");
			if (!ResourceLoader.Exists(file_path))
			{
				return null;
			}

			PortionsSetRes portionsSetRes = (PortionsSetRes)ResourceLoader.Load(file_path, cacheMode: ResourceLoader.CacheMode.Ignore);
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