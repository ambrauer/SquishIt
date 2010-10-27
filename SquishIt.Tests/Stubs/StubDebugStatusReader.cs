using System;
using SquishIt.Framework.Utilities;

namespace SquishIt.Tests.Stubs
{
    public class StubDebugStatusReader: IDebugStatusReader
    {
        private bool? isDebuggingEnabled;

        public StubDebugStatusReader(){}

        public StubDebugStatusReader(bool isDebuggingEnabled)
        {
            this.isDebuggingEnabled = isDebuggingEnabled;
        }

        #region IDebugStatusReader Members

        public bool IsDebuggingEnabled()
        {
            return isDebuggingEnabled ?? true;
        }

        public void ForceDebug()
        {
            isDebuggingEnabled = true;
        }

        public void ForceRelease()
        {
            isDebuggingEnabled = false;
        }

        public bool IsForced()
        {
            return isDebuggingEnabled.HasValue;
        }

        #endregion
    }
}