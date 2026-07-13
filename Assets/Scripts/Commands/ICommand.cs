using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    public interface ICommand{
        bool CanHandle(AbstractCommandable commandable, RaycastHit hit);
        void Handle(AbstractCommandable commandable, RaycastHit hit);
    }
}