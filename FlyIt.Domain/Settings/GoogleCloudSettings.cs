namespace FlyIt.Domain.Settings
{
    public class GoogleCloudSettings
    {
        public string GoogleCloudStorageBucket { get; set; }

        public GoogleCloudKey GoogleCloudKey { get; set; }
    }

    public class GoogleCloudKey
    {
        public string Type { get; set; }

        public string Project_Id { get; set; }

        public string Private_Key_Id { get; set; }

        public string Private_Key { get; set; }

        public string Client_Email { get; set; }

        public string Client_Id { get; set; }

        public string Auth_Uri { get; set; }

        public string Token_Uri { get; set; }

        public string Auth_Provider_X509_Cert_Url { get; set; }

        public string Client_X509_Cert_Url { get; set; }
    }
}