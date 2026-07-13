namespace GameDevTV.RTS.Commands
{
    public interface ICommand{
        bool CanHandle(CommandContext cxt);
        void Handle(CommandContext cxt);
    }
}