namespace GameDevTV.RTS.Environment
{
public interface IGatherable{
    public SupplySO Supply {get;}
    public int AmountRemaining {get;}
    public bool IsBusy {get;}

    public bool BeginGather(); // returns true if we're able to gather
    public int EndGather(); // gives amount gathered (might not be equal to SupplySO.AmountPerGather)
}
}