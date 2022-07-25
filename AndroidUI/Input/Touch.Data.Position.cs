namespace AndroidUI.Input
{
    public partial class Touch
    {
        public partial class Data
        {
            public class Position : ICloneable
            {
                public float x, y;

                public Position(float x, float y)
                {
                    this.x = x;
                    this.y = y;
                }

                public Position()
                {
                }

                public object Clone()
                {
                    return new Position(x, y);
                }

                public override string ToString()
                {
                    return ToString(x, y);
                }

                internal static string ToString(float x, float y)
                {
                    return "" + x + ", " + y;
                }
            }
        }
    }
}
