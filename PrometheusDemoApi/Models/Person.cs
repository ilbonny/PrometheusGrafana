using System;

namespace PrometheusDemoApi.Models
{
    public class Person
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public Person(string id, string firstname, string lastname )
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
        }        
    }
}
