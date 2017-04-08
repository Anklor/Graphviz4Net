
namespace Graphviz4Net.Dot
{
    public sealed class BoundingBox
    {
        private readonly string dotRepresentation;

        internal BoundingBox(string dotRepresentation)
        {
            this.dotRepresentation = dotRepresentation;

            if (dotRepresentation != null)
            {
                var parts = dotRepresentation.Split(',');
                if (parts.Length >= 4)
                {
                    LeftX = Utils.ParseInvariantNullableDouble(parts[0]);
                    LowerY = Utils.ParseInvariantNullableDouble(parts[1]);
                    RightX = Utils.ParseInvariantNullableDouble(parts[2]);
                    UpperY = Utils.ParseInvariantNullableDouble(parts[3]);
                }
            }
        }

        public bool HasAllValues
        {
            get
            {
                return LeftX.HasValue &&
                    RightX.HasValue &&
                    LowerY.HasValue &&
                    UpperY.HasValue;
            }
        }

        public double? LeftX { get; set; }

        public double? LowerY { get; set; }

        public double? RightX { get; set; }

        public double? UpperY { get; set; }

        internal bool Equals(string bb)
        {
            return dotRepresentation == bb;
        }
    }
}
