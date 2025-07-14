
using System;

namespace myapp.Services
{
    public interface ISystemSettingsService
    {
        Guid GetDefaultStatusId();
        Guid GetReadyForDeliveryStatusId();
    }

    public class SystemSettingsService : ISystemSettingsService
    {
        // In a real application, these values should come from a configuration file or a database table.
        private readonly Guid _defaultStatusId = new Guid("a1b2c3d4-e5f6-7788-99a0-b1c2d3e4f5a6"); // Example GUID
        private readonly Guid _readyForDeliveryStatusId = new Guid("f1e2d3c4-b5a6-9988-77a0-e1d2c3b4a5f6"); // Example GUID

        public Guid GetDefaultStatusId()
        {
            return _defaultStatusId;
        }

        public Guid GetReadyForDeliveryStatusId()
        {
            return _readyForDeliveryStatusId;
        }
    }
}
