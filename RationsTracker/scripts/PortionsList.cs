using Godot;

public partial class PortionsList : Control
{
    private Window _popupWindow;
    private LineEdit _popupNameLineEdit;
    private SpinBox _popupTargetValueSpinBox;
    private VBoxContainer _portionsContainer;
    private HBoxContainer _listButtonsContainer;

    private struct MovingPortionInfo
    {
        public  Portion PortionToBeMoved = null;
        public float YShift = 0f;
        public float YOnRelease = 0f;

        public MovingPortionInfo()
        {
            PortionToBeMoved = null;
            YShift = 0f;
            YOnRelease = 0f;
        }

        public void SnapPosition()
        {
            PortionToBeMoved.GlobalPosition = new Vector2(PortionToBeMoved.GlobalPosition.X, YOnRelease);
        }

    }
    private MovingPortionInfo _movingPortionInfo = new MovingPortionInfo();

    public override void _Ready()
    {
        _popupWindow = GetNode<Window>("%PopupWindow");
        _popupNameLineEdit = GetNode<LineEdit>("%NameLineEdit");
        _popupTargetValueSpinBox = GetNode<SpinBox>("%TargetValueSpinBox");
        _portionsContainer = GetNode<VBoxContainer>("%PortionsContainer");
        _listButtonsContainer = GetNode<HBoxContainer>("%ListButtonsContainer");

        SetPhysicsProcess(false);
    }
    public override void _PhysicsProcess(double delta)
    {   
        float newY = GetGlobalMousePosition().Y - _movingPortionInfo.YShift;
        _movingPortionInfo.PortionToBeMoved.GlobalPosition = new Vector2(_movingPortionInfo.PortionToBeMoved.GlobalPosition.X,newY);   

        if (_CheckPortionMovedUp(_movingPortionInfo.PortionToBeMoved))
        {
            int index = _movingPortionInfo.PortionToBeMoved.GetIndex();
            _movingPortionInfo.YOnRelease = newY;
            _portionsContainer.MoveChild(_movingPortionInfo.PortionToBeMoved, index-1);
            return;
        }
        if (_CheckPortionMovedDown(_movingPortionInfo.PortionToBeMoved))
        {
            int index = _movingPortionInfo.PortionToBeMoved.GetIndex();
            _movingPortionInfo.YOnRelease = newY;
            _portionsContainer.MoveChild(_movingPortionInfo.PortionToBeMoved, index+1);
            return;
        }
    }

    private bool _CheckPortionMovedUp(Portion portion)
    {
        int index = portion.GetIndex();
        if (index == 0 || _portionsContainer.GetChildCount()<=2)
            return false;

        float previousY = _portionsContainer.GetChild<Portion>(index-1).GlobalPosition.Y;
        return previousY > portion.GlobalPosition.Y;
    }
    private bool _CheckPortionMovedDown(Portion portion)
    {
        int index = portion.GetIndex();
        if (index == _portionsContainer.GetChildCount()-2 || _portionsContainer.GetChildCount()<=2)
            return false;

        float nextY = _portionsContainer.GetChild<Portion>(index+1).GlobalPosition.Y;
        return nextY < portion.GlobalPosition.Y;
    }
    private void _CreatePortion(PortionRes portionRes = null)
    {
        if (portionRes == null)
            portionRes = new PortionRes();

        portionRes.MaxValue = (int)_popupTargetValueSpinBox.Value;
        portionRes.PortionName = _popupNameLineEdit.Text;

        Portion portion = Globals.PackedScenes.Portion.Instantiate<Portion>();

        portion.MoveButtonChanged += OnPortionMoveButtonChanged;
        // portion.PortionNameChanged += OnPortionNameChanged;
        portion.Init(portionRes);
        Globals.SetsData.AddPortion(portionRes.PortionName, portion);

        _portionsContainer.AddChild(portion);
        _portionsContainer.MoveChild(_listButtonsContainer, _portionsContainer.GetChildCount() - 1);
        GetTree().CallGroup(
            "portions",
			Portion.MethodName.AddSelectionCheckBox,
			new Variant[] { portionRes.PortionName }
        );
    }

    
    public void LoadPortions(PortionsSetRes portionsSetRes)
    {
        foreach(PortionRes portionRes in portionsSetRes.PortionsResList)
        {
            Portion portion = Globals.PackedScenes.Portion.Instantiate<Portion>();
            portion.MoveButtonChanged += OnPortionMoveButtonChanged;
            // portion.PortionNameChanged += OnPortionNameChanged;

            portion.Init(portionRes);
            Globals.SetsData.AddPortion(portionRes.PortionName, portion);
        }

        foreach(Portion portion in Globals.SetsData.PortionsDict.Values)
        {
            _portionsContainer.AddChild(portion);
        }
        
        _portionsContainer.MoveChild(_listButtonsContainer, _portionsContainer.GetChildCount() - 1);
    }
    
    
    public void _on_add_portion_button_button_down()
    {
        _popupWindow.Show();
        _popupNameLineEdit.GrabFocus();
    }
    public void _on_popup_window_close_requested()
    {
        _popupWindow.Hide();
    }
    public void _on_add_button_button_down()
    {
        _CreatePortion();
        _popupWindow.Hide();
    }
    public void _on_delete_all_portion_button_button_down()
    {
        foreach(Portion portion in Globals.SetsData.PortionsDict.Values)
        {
            _portionsContainer.RemoveChild(portion);
            Globals.SetsData.RemovePortion(portion.Info.PortionName);
            portion.QueueFree();
        }   
    }
    public void _on_reset_all_portion_button_button_down()
    {
        foreach(Portion portion in Globals.SetsData.PortionsDict.Values)
        {
            portion.SubstractValueToProgressBar(portion.Info.IntrisicValue);
            portion.Info.IntrisicValue = 0;
        }   
    }
    public void OnPortionMoveButtonChanged(Portion portion, bool down)
    {
        if (down)
        {
            _movingPortionInfo.PortionToBeMoved = portion;
            _movingPortionInfo.YShift = GetGlobalMousePosition().Y - portion.GlobalPosition.Y;
            _movingPortionInfo.YOnRelease = portion.GlobalPosition.Y;
            SetPhysicsProcess(true);
            return;
        }

        _movingPortionInfo.SnapPosition();
        _movingPortionInfo.PortionToBeMoved = null;
        SetPhysicsProcess(false);
    }
    // public void OnPortionNameChanged(string newName)
    // {
        
    // }

}
