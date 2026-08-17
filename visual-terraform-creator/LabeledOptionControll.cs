using Godot;

[Tool]
public partial class LabeledOptionControll : Control
{
	[Export]
	public string LabelText
	{
		get
		{
			return _labelString;
		}
		set
		{
			_labelString = value;
			
			if (Label != null) //Engine can try to set text before _Ready()
			{
				Label.Text = _labelString;
			}
		}
	}

	public Label Label;
	public OptionButton OptionButton;

	private string _labelString = string.Empty;

	public override void _Ready()
	{
		Label = GetNode<Label>("HBoxContainer/Label");
		OptionButton = GetNode<OptionButton>("HBoxContainer/OptionButton");

		if (Label != null)
		{
			Label.Text = LabelText;
		}
	}
}
