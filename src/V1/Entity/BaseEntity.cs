using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Entity
{
    public class BaseEntity : IDisposable
    {
        private readonly SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

        private bool _disposed;

        protected BaseEntity(){}

        public long CreateDate { get; set; }

        public long UpdateDate { get; set; }

        public long? DeleteDate { get; set; }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BaseEntity()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _handle.Dispose();
            }

            _disposed = true;
        } 
        #endregion
    }
}
