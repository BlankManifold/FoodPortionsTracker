using Godot;
using System.Linq;
using System.Collections.Generic;

public partial class Portion : MarginContainer
{
    private ProgressBar _progressBar;
    private Label _progressBarLabel;
    private LineEdit _nameLabel;
    private PortionOptionsBox _portionOptionsBox;
    private ColorRect _colorRect;
    private PortionRes _info = null;
    public PortionRes Info
    {
        get { return _info; }
    }

    [Signal]
    public delegate void MoveButtonChangedEventHandler(Portion portion, bool down);
    [Signal]
    public delegate void PortionNameChangedEventHandler(string newName);


    public void Init(PortionRes info)
    {
        _info = info;
    }
    public override void _Ready()
    {
        AddToGroup("portions");

        _progressBar = GetNode<ProgressBar>("%ProgressBar");
        _progressBarLabel = GetNode<Label>("%ProgressBarLabel");
        _nameLabel = GetNode<LineEdit>("%NameLabel");
        _portionOptionsBox = GetNode<PortionOptionsBox>("%PortionOptionsBox");
        _colorRect = GetNode<ColorRect>("%ColorRect");

        if (_info == null)
            _info = new PortionRes();

        _nameLabel.Text = _info.PortionName;
        _InitProgressBar(_info);
        _portionOptionsBox.UpdateCheckBoxes(_info.LowerPortions, _info.UpperPortions);
        _portionOptionsBox.Disable(_info.PortionName);
        _colorRect.Color = _info.PortionColor;
    }


    private void _InitProgressBar(PortionRes info)
    {
        _progressBar.MinValue = info.MinValue;
        _progressBar.MaxValue = info.MaxValue;
        _progressBar.Value = info.Value;
        _UpdateProgressBarLabel(info.Value);
    }
    private void _UpdateProgressBarLabel(int value)
    {
        _progressBarLabel.Text = $"{value}/{_info.MaxValue}";
    }
    private void _UpdateUpperPortions(int delta, bool add)
    {
        if (add)
        {
            foreach (string type in _info.UpperPortions)
            {
                Globals.SetsData.PortionsDict[type].AddValueToProgressBar(delta);
            }
        }
        else
        {
            foreach (string type in _info.UpperPortions)
            {
                Globals.SetsData.PortionsDict[type].SubstractValueToProgressBar(delta);
            }
        }
    }


    public void RemoveLowerPortion(Portion portion)
    {
        _info.LowerPortions.Remove(portion.Info.PortionName);
        portion.Info.UpperPortions.Remove(_info.PortionName);
        portion.DisableSelectionCheckBox(_info.PortionName, false);

        SubstractValueToProgressBar(portion.Info.Value);
    }
    public void AddLowerPortion(Portion portion)
    {
        _info.LowerPortions.Add(portion.Info.PortionName);
        portion.Info.UpperPortions.Add(_info.PortionName);
        portion.DisableSelectionCheckBox(_info.PortionName, true);
        AddValueToProgressBar(portion.Info.Value);
    }
    public void AddSelectionCheckBox(string type)
    {
        _portionOptionsBox.AddCheckBox(type);
        if (type == _info.PortionName)
            _portionOptionsBox.Disable(_info.PortionName);
    }
    public void RemoveSelectionCheckBox(string type)
    {
        _portionOptionsBox.RemoveCheckBox(type);
    }
    public void DisableSelectionCheckBox(string type, bool disable)
    {
        _portionOptionsBox.Disable(type, disable);
    }
    public void AddValueToProgressBar(int delta)
    {
        _info.Value += delta;
        _progressBar.Value += delta;

        _UpdateProgressBarLabel(_info.Value);
        _UpdateUpperPortions(delta, true);
    }
    public void SubstractValueToProgressBar(int delta)
    {
        if (_info.Value > _info.MinValue)
        {
            _info.Value -= delta;
            if (_info.Value < _info.MaxValue)
                _progressBar.Value -= delta;

            _UpdateProgressBarLabel(_info.Value);
            _UpdateUpperPortions(delta, false);
        }
    }
    public void UpdateCheckBoxName(string oldName, string newName)
    {
        if (_info.UpperPortions.Contains(oldName))
        {
            _info.UpperPortions.Remove(oldName);
            _info.UpperPortions.Add(newName);
        }
        if (_info.LowerPortions.Contains(oldName))
        {
            _info.LowerPortions.Remove(oldName);
            _info.LowerPortions.Add(newName);
        }

        _portionOptionsBox.UpdateCheckBoxName(oldName, newName);
    }

    public void _on_minus_button_button_down()
    {
        _info.IntrisicValue = (_info.IntrisicValue > 0) ? _info.IntrisicValue - 1 : 0;
        SubstractValueToProgressBar(1);
    }
    public void _on_plus_button_button_down()
    {
        _info.IntrisicValue += 1;
        AddValueToProgressBar(1);
    }
    public void _on_settings_button_toggled(bool pressed)
    {
        _portionOptionsBox.Visible = pressed;
    }
    public void _on_portion_options_box_confirmed_changes(Godot.Collections.Array<string> checkedChildren)
    {
        IEnumerable<string> typesToBeAdded = checkedChildren.Except<string>(_info.LowerPortions);
        IEnumerable<string> typesToBeRemoved = _info.LowerPortions.Except<string>(checkedChildren);

        if (typesToBeAdded.Count<string>() != 0)
        {
            foreach (string type in typesToBeAdded)
            {
                Portion portion = Globals.SetsData.PortionsDict[type];
                AddLowerPortion(portion);
            }
        }

        if (typesToBeRemoved.Count<string>() != 0)
        {
            foreach (string type in typesToBeRemoved)
            {
                Portion portion = Globals.SetsData.PortionsDict[type];
                RemoveLowerPortion(portion);
            }
        }
    }
    public void _on_portion_options_box_delete_portion()
    {
        foreach (string type in _info.UpperPortions)
        {
            Portion portion = Globals.SetsData.PortionsDict[type];
            portion.RemoveLowerPortion(this);
        }

        GetTree().CallGroup(
            "portions",
            Portion.MethodName.RemoveSelectionCheckBox,
            new Variant[] { _info.PortionName }
        );
        Globals.SetsData.RemovePortion(_info.PortionName);
        QueueFree();
    }
    public void _on_portion_options_box_color_changed(Color color)
    {
        _colorRect.Color = color;
        _info.PortionColor = color;
    }
    public void _on_move_button_button_down()
    {
        EmitSignal(SignalName.MoveButtonChanged, new Variant[] { this, true });
    }
    public void _on_move_button_button_up()
    {
        EmitSignal(SignalName.MoveButtonChanged, new Variant[] { this, false });
    }
    public void _on_name_label_text_submitted(string newText)
    {
        if (!Globals.SetsData.ContainsPortionType(newText))
        {
            string oldName = _info.PortionName;

            GetTree().CallGroup(
                "portions",
                Portion.MethodName.UpdateCheckBoxName,
                new Variant[] {oldName, newText}
                );

            _info.PortionName = newText;
            // EmitSignal(SignalName.PortionNameChanged, new Variant[] { newText });
        }

        _nameLabel.ReleaseFocus();
    }

}
