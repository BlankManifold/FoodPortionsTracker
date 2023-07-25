using Godot;
using System.Linq;
using System.Collections.Generic;


public partial class SelectPortionChildrenBox : VBoxContainer
{
    private VBoxContainer _childrenCheckBoxesContainer;
    private Godot.Collections.Dictionary<string, CheckBox> _checkBoxesDict = new Godot.Collections.Dictionary<string, CheckBox>();
    private Godot.Collections.Dictionary<string, bool> _initialCheckBoxesNameDict = new Godot.Collections.Dictionary<string, bool>();

    [Signal]
    public delegate void ConfirmedChangesEventHandler(Godot.Collections.Array<string> checkedChildren);
    public void Init(Godot.Collections.Array<string> portionTypes)
    {
        foreach (string type in portionTypes)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.ButtonPressed = false;
            checkBox.Text = type;

            _checkBoxesDict.Add(type, checkBox);
        }
    }
    public override void _Ready()
    {
        _childrenCheckBoxesContainer = GetNode<VBoxContainer>("%ChildrenCheckBoxesContainer");

        Init(Globals.SetsData.AllTypes);

        foreach (CheckBox checkBox in _checkBoxesDict.Values)
            _childrenCheckBoxesContainer.AddChild(checkBox);
    }

    public void Disable(string name)
    {
        _checkBoxesDict[name].Disabled = true;
    }
    public void Clear()
    {
        foreach (CheckBox checkBox in _checkBoxesDict.Values)
            checkBox.ButtonPressed = false;
    }
    public void UpdateCheckBoxes(Godot.Collections.Array<string> portionsTypes)
    {
        foreach (string type in portionsTypes)
        {
            _checkBoxesDict[type].ButtonPressed = true;
            _initialCheckBoxesNameDict[type] = true;
        }
    }
    private void _UpdateInitialDict()
    {
        foreach (KeyValuePair<string, CheckBox> item in _checkBoxesDict)
        {
            _initialCheckBoxesNameDict[item.Key] = item.Value.ButtonPressed;
        }
    }
    public void Reset()
    {
        foreach (KeyValuePair<string, bool> item in _initialCheckBoxesNameDict)
        {
            _checkBoxesDict[item.Key].ButtonPressed = item.Value;
        }
    }
    public void AddCheckBox(string type)
    {
        CheckBox checkBox = new CheckBox();
        checkBox.ButtonPressed = false;
        checkBox.Text = type;

        _checkBoxesDict.Add(type, checkBox);
        _childrenCheckBoxesContainer.AddChild(checkBox);
    }
    public void RemoveCheckBox(string type)
    {
        CheckBox checkBox = _checkBoxesDict[type];
        _checkBoxesDict.Remove(type);
        _childrenCheckBoxesContainer.RemoveChild(checkBox);
        checkBox.QueueFree();
    }
    public Godot.Collections.Array<string> GetCheckedPortionTypes()
    {
        return new Godot.Collections.Array<string>(
            _checkBoxesDict.Where(
                x => x.Value.ButtonPressed).Select(
                    x => x.Key)
            );
    }

    public void _on_confirm_button_button_down()
    {
        Visible = false;
        _UpdateInitialDict();
        EmitSignal(SignalName.ConfirmedChanges, GetCheckedPortionTypes());
    }
    public void _on_esc_button_button_down()
    {
        Visible = false;
        Reset();
    }
}
