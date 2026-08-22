namespace GameDevTV.RTS.Units
{
    public interface ISelectable
    {
        void Select();
        void Deselect();
        bool IsSelected {get;}
    }
}