using System;

namespace Interfaces
{
    public interface IView
    {
        void ShowMessage(string message);
        void ClearInput();

        event Action<string> InsertRequested;
        event Action<string> SearchRequested;
        event Action<string> DeleteRequested;
    }
}
