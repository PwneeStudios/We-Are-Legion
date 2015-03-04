using FragSharpFramework;

namespace Game
{
    public partial class UnitMigrate : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Units)
        {
            unit unit_here = Units[Here];

            if (unit_here.type == _2)
                unit_here.type = _6;

            if (unit_here.type == _3)
                unit_here.type = _7;

            if (unit_here.type == _4)
                unit_here.type = _8;

            return unit_here;
        }
    }

    public partial class World : SimShader
    {
        public void Migrate()
        {
            Render.UnsetDevice();

            //DataGroup.Corpses.Clear();

            UnitMigrate.Apply(DataGroup.CurrentUnits, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentUnits);

            UnitMigrate.Apply(DataGroup.PreviousUnits, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.PreviousUnits);

            Render.UnsetDevice();
        }
    }
}
