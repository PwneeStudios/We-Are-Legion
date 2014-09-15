using FragSharpFramework;

namespace Terracotta
{
    public partial class UnitMigrate : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Units)
        {
            unit unit_here = Units[Here];

            if (unit_here.type == _10)
                unit_here.type = _3;

            return unit_here;
        }
    }

    public partial class World : SimShader
    {
        public void Migrate()
        {
            DataGroup.Corspes.Clear();

            //UnitMigrate.Apply(DataGroup.CurrentUnits, Output: DataGroup.Temp1);
            //CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentUnits);

            //UnitMigrate.Apply(DataGroup.PreviousUnits, Output: DataGroup.Temp1);
            //CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.PreviousUnits);
        }
    }
}
