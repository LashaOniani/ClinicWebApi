namespace ClinicWebApi.Packages
{
    public class PKG_Base
    {
        private string connStr;
        IConfiguration configuration;
        public PKG_Base(IConfiguration configuration)
        {
            this.configuration = configuration;
            connStr = this.configuration.GetConnectionString("OraConnStr");
        }
        protected string ConnectionStr { get { return connStr; } }
    }
}
