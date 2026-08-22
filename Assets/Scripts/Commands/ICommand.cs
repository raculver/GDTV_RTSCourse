namespace GameDevTV.RTS.Commands
{
    public interface ICommand{
        public bool IsSingleUnitCommand{get;}
        bool CanHandle(CommandContext cxt);
        void Handle(CommandContext cxt);
    }
}