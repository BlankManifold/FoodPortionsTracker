using Godot;

public partial class PortionsSetRes : Resource
{
    [Export] public string SetName;
    
    // Period (daily, weekly,...)
    //[Export] public ;

    [Export] public Godot.Collections.Array<PortionRes> PortionsResList;

    public PortionsSetRes() 
    {  
        SetName = ""; 
        PortionsResList = new Godot.Collections.Array<PortionRes> {};  
    }

}