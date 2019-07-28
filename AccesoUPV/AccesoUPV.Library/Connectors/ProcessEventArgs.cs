using System;

namespace AccesoUPV.Library.Connectors
{
    public class ProcessEventArgs : EventArgs
    {
        public bool Succeeded { get; }
        public string Output { get; }
        public string Error { get; }

        public ProcessEventArgs(bool succeeded, string output, string error)
        {
            Succeeded = succeeded;
            Output = output;
            Error = error;
        }
    }
}
