namespace DZT.Lib.Funcs;

class Lurk
{
    public record LurkState(float InitialValue, float Tstart);

    static float GetValue(float t, LurkState state)
    {
        return t * state.InitialValue;
    }
}
