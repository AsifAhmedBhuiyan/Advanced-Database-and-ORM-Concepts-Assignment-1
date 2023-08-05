using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Laptop
    {
        [Key]
        public Guid Number { get; set; }

        private string _model;

        public string Model
        {
            get => _model;
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length < 3)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Laptop model name must be at least three characters in length.");
                }
                _model = value;
            }
        }

        private decimal _price;

        public decimal Price
        {
            get => _price;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Price cannot be less than zero.");
                }
                _price = value;
            }
        }

        public LaptopCondition Condition { get; set; }

        public int BrandId { get; set; }

        public Brand Brand { get; set; }

        public int InStockQuantity { get; set; }

        public Guid StoreNumber { get; set; }

        public Store Store { get; set; }
    }

    public enum LaptopCondition
    {
        New,
        Refurbished,
        Rental
    }
}
