using System.Collections.Generic;

namespace WebApplication2.Models
{
    public class Brand
    {
        public int Id { get; set; }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length < 3)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Brand name must be at least three characters in length.");
                }
                _name = value;
            }
        }

        public ICollection<Laptop> Laptops { get; set; } = new List<Laptop>();
    }
}
