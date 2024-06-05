namespace CaesarsAPI.Models
{
    public class Guest : IEquatable<Guest>
    {
        public int id { get; set; }
        public string? title { get; set; }
        public string name_first { get; set; }
        public string name_last { get; set; }
        public string? suffix { get; set; }
        public DateTime dob { get; set; }
        public string email { get; set; }
        public string addr_street { get; set; }
        public string addr_city { get; set; }
        public string addr_state { get; set; }
        public string addr_zip { get; set; }
        public DateTime created { get; set; }

        //custom 
        public bool Equals(Guest? other)
        {
            return (this.id == other.id &&
                this.title == other.title &&
                this.name_first == other.name_first &&
                this.name_last == other.name_last &&
                this.suffix == other.suffix &&
                this.dob == other.dob &&
                this.email == other.email &&
                this.addr_street == other.addr_street &&
                this.addr_city == other.addr_city &&
                this.addr_state == other.addr_state &&
                this.addr_zip == other.addr_zip);
        }
    }
}
