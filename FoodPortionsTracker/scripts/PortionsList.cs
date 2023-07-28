using Godot;
using System;

public partial class PortionsList : Control
{
    private Window _popupWindow;
    private LineEdit _popupNameLineEdit;
    private SpinBox _popupTargetValueSpinBox;
    private VBoxContainer _portionsContainer;
    private Button _addPortionButton;

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
    private bool _movingPortionFlag = false;

    public override void _Ready()
    {
        _popupWindow = GetNode<Window>("%PopupWindow");
        _popupNameLineEdit = GetNode<LineEdit>("%NameLineEdit");
        _popupTargetValueSpinBox = GetNode<SpinBox>("%TargetValueSpinBox");
        _portionsContainer = GetNode<VBoxContainer>("%PortionsContainer");
        _addPortionButton = GetNode<Button>("%AddPortionButton");
    }
    public override void _PhysicsProcess(double delta)
    {
        if (!_movingPortionFlag)
            return;
        
        _movingPortionInfo.PortionToBeMoved.GlobalPosition = new Vector2(
             _movingPortionInfo.PortionToBeMoved.GlobalPosition.X, 
             GetGlobalMousePosition().Y - _movingPortionInfo.YShift
             );   

        if (_checkPortionMovedUp(_movingPortionInfo.PortionToBeMoved))
        {
            int index = _movingPortionInfo.PortionToBeMoved.GetIndex();
            float newY =  _portionsContainer.GetChild<Portion>(index-1).GlobalPosition.Y;
            _movingPortionInfo.YOnRelease = newY;
            _portionsContainer.MoveChild(_movingPortionInfo.PortionToBeMoved, index-1);
            return;
        }
        if (_checkPortionMovedDown(_movingPortionInfo.PortionToBeMoved))
        {
            int index = _movingPortionInfo.PortionToBeMoved.GetIndex();
            float newY =  _portionsContainer.GetChild<Portion>(index+1).GlobalPosition.Y;
            _movingPortionInfo.YOnRelease = newY;
            _portionsContainer.MoveChild(_movingPortionInfo.PortionToBeMoved, index+1);
            return;
        }
    }


    private bool _checkPortionMovedUp(Portion portion)
    {
        int index = portion.GetIndex();
        if (index == 0 || _portionsContainer.GetChildCount()<=2)
            return false;

        float previousY = _portionsContainer.GetChild<Portion>(index-1).GlobalPosition.Y;
        return previousY > portion.GlobalPosition.Y;
    }
    private bool _checkPortionMovedDown(Portion portion)
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
        portion.Init(portionRes);
        _portionsContainer.AddChild(portion);
        _portionsContainer.MoveChild(_addPortionButton, _portionsContainer.GetChildCount() - 1);

        Globals.SetsData.AddPortion(portionRes.PortionName, portion);
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
            portion.Init(portionRes);
            Globals.SetsData.AddPortion(portionRes.PortionName, portion);
        }

        foreach(Portion portion in Globals.SetsData.PortionsDict.Values)
        {
            _portionsContainer.AddChild(portion);
        }
        
        _portionsContainer.MoveChild(_addPortionButton, _portionsContainer.GetChildCount() - 1);
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
    public void OnPortionMoveButtonChanged(Portion portion, bool down)
    {
        if (down)
        {
            _movingPortionInfo.PortionToBeMoved = portion;
            _movingPortionInfo.YShift = GetGlobalMousePosition().Y - portion.GlobalPosition.Y;
            _movingPortionInfo.YOnRelease = portion.GlobalPosition.Y;
            _movingPortionFlag = true;
            return;
        }

        _movingPortionInfo.SnapPosition();
        _movingPortionInfo.PortionToBeMoved = null;
        _movingPortionFlag = false;
    }
}
