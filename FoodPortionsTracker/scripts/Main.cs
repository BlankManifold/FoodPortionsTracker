using Godot;
using System;

public partial class Main : Node
{
    private PortionsList _portionsList;
    public override void _Ready()
    {
		Handlers.SaveLoadHandler.CreateSaveSetsDir();

        _portionsList = GetNode<PortionsList>("%PortionsList");
        PortionsSetRes portionsSetRes = Handlers.SaveLoadHandler.LoadSet("prova");
        _portionsList.LoadPortions(portionsSetRes);
    }

    public void _on_tree_exiting()
    {
        Handlers.SaveLoadHandler.SaveSet(
            new Godot.Collections.Array<Portion>(Globals.SetsData.PortionsDict.Values),
            "prova"
        );
    }

}
