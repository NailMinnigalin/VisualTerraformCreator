namespace VTСLib
{
	public interface ITectonicPlateType : IEquatable<ITectonicPlateType>
	{
		/// <summary>
		/// Height in meters
		/// </summary>
		public int MinHeight { get; }

		/// <summary>
		/// Height in meters
		/// </summary>
		public int MaxHeight { get; }
	}

	/// <summary>
	/// Base implemenetation of ITectonicPlateType. Provide common implementations of some methods.
	/// </summary>
	public abstract class BaseTectonicPlateType : ITectonicPlateType
	{
		public abstract int MinHeight { get; }

		public abstract int MaxHeight { get; }

		public bool Equals(ITectonicPlateType? other)
		{
			if (other == null) 
				return false;

			return this.GetType() == other.GetType();
		}

		public override int GetHashCode() => this.GetType().GetHashCode();

		public override bool Equals(object? obj)
		{
			if (obj == null)
				return false;

			return this.GetType() == obj.GetType();
		}
	}

	public class ContinentalPlateType : BaseTectonicPlateType
	{
		public override int MinHeight => 300;

		public override int MaxHeight => 800;
	}

	public class OceanicPlateType : BaseTectonicPlateType
	{
		public override int MinHeight => -4500;

		public override int MaxHeight => -3700;
	}
}
