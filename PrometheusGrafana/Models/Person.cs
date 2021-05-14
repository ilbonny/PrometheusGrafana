using System;

namespace PrometheusGrafana.Models
{
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}