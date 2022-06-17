using System;

namespace ConsoleUserInterface.Core {
    public interface IComponent {
        public ITransform Transform { get; }
        public bool ReceiveKey(ConsoleKeyInfo keyInfo);
    }
}
