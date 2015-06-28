namespace DBC.Services
{
    public class MessageServicesOptions
    {
        public string clientDomain { get; set; }
        public bool enableSsl { get; set; }
        public string host { get; set; }
        public string password { get; set; }
        public int port { get; set; }
        public string userName { get; set; }
        public string deliveryMethod { get; set; }
        public string from { get; set; }
        public string fromName { get; set; }
        public string pickupDirectoryLocation { get; set; }
    }
}