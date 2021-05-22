namespace PrometheusGrafana.Configuration
{
    public class ApiConfiguration
    {
        public string Url { get; set; }
        public string GetPath { get; set; }
        public string AddPath { get; set; }
        public string ModifyPath { get; set; }
        public string DeletePath { get; set; }
    }
}