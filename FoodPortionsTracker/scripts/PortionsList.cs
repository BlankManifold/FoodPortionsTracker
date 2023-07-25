using Godot;
using System;

public partial class PortionsList : Control
{
    private Window _popupWindow;
    private LineEdit _popupNameLineEdit;
    private SpinBox _popupTargetValueSpinBox;
    private VBoxContainer _portionsContainer;
    private Button _addPortionButton;

    public override void _Ready()
    {
        _popupWindow = GetNode<Window>("%PopupWindow");
        _popupNameLineEdit = GetNode<LineEdit>("%NameLineEdit");
        _popupTargetValueSpinBox = GetNode<SpinBox>("%TargetValueSpinBox");
        _portionsContainer = GetNode<VBoxContainer>("%PortionsContainer");
        _addPortionButton = GetNode<Button>("%AddPortionButton");
    }



    private void _CreatePortion(PortionRes portionRes = null)
    {
        if (portionRes == null)
            portionRes = new PortionRes();

        portionRes.MaxValue = (int)_popupTargetValueSpinBox.Value;
        portionRes.PortionName = _popupNameLineEdit.Text;

        Portion portion = Globals.PackedScenes.Portion.Instantiate<Portion>();

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
}
