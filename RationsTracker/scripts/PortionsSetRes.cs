using Godot;

public partial class PortionsSetRes : Resource
{
    [Export] public string SetName;
    [Export] public int PeriodScaleIndex;
    [Export] public int ResetDayIndex;
    
    // Period (daily, weekly,...)
    //[Export] public ;

    [Export] public Godot.Collections.Array<PortionRes> PortionsResList;

    public PortionsSetRes() 
    {  
        SetName = ""; 
        PeriodScaleIndex = 0; 
        ResetDayIndex = 0; 
        PortionsResList = new Godot.Collections.Array<PortionRes> {};  
    }

    public void AddPortion(Portion portion)
    {
        PortionsResList.Add(portion.Info);
    }
    public void RemovePortion(Portion portion)
    {
        PortionsResList.Remove(portion.Info);
    }
}