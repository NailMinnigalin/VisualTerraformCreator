using BiMap;
using Godot;
using System;
using System.Collections.Generic;
using VisualTerraformCreator.Scenes.MainWorkWindow.Visualizer;
using VTСLib;

public partial class MainWorkWindow : Control
{
	[Export]
	public LabeledOptionControll PlateTypeALabeledOption = default!;
	[Export]
	public LabeledOptionControll PlateTypeBLabeledOption = default!;
	[Export]
	public Visualizer Visualizer = default!;
	[Export]
	public LabeledOptionControll BoundryTypeLabeledOption = default!;
	[Export]
	public Camera2d Camera2d = default!;

	private ITectonicPlateType _plateTypeA = new OceanicPlateType();
	private ITectonicPlateType _plateTypeB = new ContinentalPlateType();
	private PlateBoundaryType _plateBoundryType = PlateBoundaryType.Convergent;

	private BiMap<ITectonicPlateType, int> _plateTypeId = new();
	private BiMap<PlateBoundaryType, int> _boundryTypeId = new();

	public override void _Ready()
	{
		InitBiMap(_plateTypeId, GetPossiblePlateTypeList());
		InitBiMap(_boundryTypeId, GetPossiblePlateBoundaryTypeList());
		InitLabeledOptions();
		Visualizer.Initialize(Camera2d, _plateTypeA, _plateTypeB, _plateBoundryType);
	}

	private void InitBiMap<TLeft>(BiMap<TLeft, int> biMap, List<TLeft> itemList)
	{
		int id = 0;
		foreach (var item in itemList)
		{
			biMap.Add(item, id);
			id++;
		}
	}

	private void InitLabeledOptions()
	{
		InitLabeledOption(new InitLabeledOptionParamsForPlateTypeA(this));
		InitLabeledOption(new InitLabeledOptionParamsForPlateTypeB(this));
		InitLabeledOption(new InitLabeledOptionParamsForBoundryType(this));
	}

	private void InitLabeledOption<T>(InitLabeledOptionParams<T> initLabeledOptionParams)
	{
		LabeledOptionControll labeledOptionControll = initLabeledOptionParams.LabeledOptionControll;

		AddOptionItems(labeledOptionControll, initLabeledOptionParams.ItemsList, initLabeledOptionParams.GetItemName, initLabeledOptionParams.GetItemId);
		labeledOptionControll.OptionButton.Selected = initLabeledOptionParams.GetItemId(initLabeledOptionParams.CurrentItem);
		labeledOptionControll.OptionButton.ItemSelected += initLabeledOptionParams.ItemSelectedAction;
	}

	private void BoundryTypeSelected(long index)
	{
		_plateBoundryType = _boundryTypeId.GetRightToLeft((int)index);
		Visualizer.SetPlateBoundaryType(_plateBoundryType);
	}

	private void PlateTypeBItemSelected(long index)
	{
		_plateTypeB = _plateTypeId.GetRightToLeft((int)index);
		Visualizer.SetPlateTypeB(_plateTypeB);
	}

	private void PlateTypeAItemSelected(long index)
	{
		_plateTypeA = _plateTypeId.GetRightToLeft((int)index);
		Visualizer.SetPlateTypeA(_plateTypeA);
	}

	private void AddOptionItems<T>(LabeledOptionControll labeledOptionControll, List<T> itemsList, Func<T, string> GetItemName, Func<T, int> GetItemId)
	{
		foreach (var item in itemsList)
		{
			labeledOptionControll.OptionButton.AddItem(GetItemName(item), GetItemId(item));
		}
	}

	private List<ITectonicPlateType> GetPossiblePlateTypeList()
	{
		return new()
		{
			new ContinentalPlateType(),
			new OceanicPlateType()
		};
	}

	private List<PlateBoundaryType> GetPossiblePlateBoundaryTypeList()
	{
		return new()
		{
			PlateBoundaryType.Divergent,
			PlateBoundaryType.Convergent
		};
	}

	private string PlateTypeToText(ITectonicPlateType tectonicPlateType)
	{
		if (tectonicPlateType is ContinentalPlateType)
		{
			return "Continental";
		}
		else if (tectonicPlateType is OceanicPlateType)
		{
			return "Oceanic";
		}

		throw new ArgumentOutOfRangeException("Unkown argument type");
	}

	private string BoundaryTypeToText(PlateBoundaryType plateBoundaryType)
	{
		return plateBoundaryType switch
		{
			PlateBoundaryType.Convergent => "Convergent",
			PlateBoundaryType.Divergent => "Divergent",
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	/// <summary>
	/// Params required for initing Labeled Option Buttons
	/// </summary>
	/// <typeparam name="T"></typeparam>
	private abstract class InitLabeledOptionParams<T>
	{
		/// <summary>
		/// Main controll object
		/// </summary>
		public abstract LabeledOptionControll LabeledOptionControll { get; }
		/// <summary>
		/// List of all items for option button
		/// </summary>
		public abstract List<T> ItemsList { get; }
		/// <summary>
		/// Func that returns string representing an item for given item
		/// </summary>
		public abstract Func<T, string> GetItemName { get; }
		/// <summary>
		/// Func that returns item's id for given item
		/// </summary>
		public abstract Func<T, int> GetItemId { get; }
		/// <summary>
		/// Selected item at the time of initialization
		/// </summary>
		public abstract T CurrentItem { get; }
		/// <summary>
		/// Callback function that will be called when user selects new item
		/// </summary>
		public abstract OptionButton.ItemSelectedEventHandler ItemSelectedAction { get; }
	}

	private class InitLabeledOptionParamsForPlateTypeA : InitLabeledOptionParams<ITectonicPlateType>
	{
		public override LabeledOptionControll LabeledOptionControll => _mainWorkWindow.PlateTypeALabeledOption;

		public override List<ITectonicPlateType> ItemsList => _mainWorkWindow.GetPossiblePlateTypeList();

		public override Func<ITectonicPlateType, string> GetItemName => _mainWorkWindow.PlateTypeToText;

		public override Func<ITectonicPlateType, int> GetItemId => _mainWorkWindow._plateTypeId.GetLeftToRight;

		public override ITectonicPlateType CurrentItem => _mainWorkWindow._plateTypeA;

		public override OptionButton.ItemSelectedEventHandler ItemSelectedAction => _mainWorkWindow.PlateTypeAItemSelected;

		private MainWorkWindow _mainWorkWindow;

		public InitLabeledOptionParamsForPlateTypeA(MainWorkWindow mainWorkWindow)
		{
			_mainWorkWindow = mainWorkWindow;
		}
	}

	private class InitLabeledOptionParamsForPlateTypeB : InitLabeledOptionParams<ITectonicPlateType>
	{
		public override LabeledOptionControll LabeledOptionControll => _mainWorkWindow.PlateTypeBLabeledOption;

		public override List<ITectonicPlateType> ItemsList => _mainWorkWindow.GetPossiblePlateTypeList();

		public override Func<ITectonicPlateType, string> GetItemName => _mainWorkWindow.PlateTypeToText;

		public override Func<ITectonicPlateType, int> GetItemId => _mainWorkWindow._plateTypeId.GetLeftToRight;

		public override ITectonicPlateType CurrentItem => _mainWorkWindow._plateTypeB;

		public override OptionButton.ItemSelectedEventHandler ItemSelectedAction => _mainWorkWindow.PlateTypeBItemSelected;

		private MainWorkWindow _mainWorkWindow;

		public InitLabeledOptionParamsForPlateTypeB(MainWorkWindow mainWorkWindow)
		{
			_mainWorkWindow = mainWorkWindow;
		}
	}

	private class InitLabeledOptionParamsForBoundryType : InitLabeledOptionParams<PlateBoundaryType>
	{
		public override LabeledOptionControll LabeledOptionControll => _mainWorkWindow.BoundryTypeLabeledOption;

		public override List<PlateBoundaryType> ItemsList => _mainWorkWindow.GetPossiblePlateBoundaryTypeList();

		public override Func<PlateBoundaryType, string> GetItemName => _mainWorkWindow.BoundaryTypeToText;

		public override Func<PlateBoundaryType, int> GetItemId => _mainWorkWindow._boundryTypeId.GetLeftToRight;

		public override PlateBoundaryType CurrentItem => _mainWorkWindow._plateBoundryType;

		public override OptionButton.ItemSelectedEventHandler ItemSelectedAction => _mainWorkWindow.BoundryTypeSelected;

		private MainWorkWindow _mainWorkWindow;

		public InitLabeledOptionParamsForBoundryType(MainWorkWindow mainWorkWindow)
		{
			_mainWorkWindow = mainWorkWindow;
		}
	}
}
