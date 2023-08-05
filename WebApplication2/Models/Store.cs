using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Store
    {
        [Key]
        public Guid StoreNumber { get; set; }

        [Required(ErrorMessage = "Street name and number must be provided.")]
        public string StreetNameAndNumber { get; set; }

        [Required(ErrorMessage = "Province must be provided.")]
        [EnumDataType(typeof(CanadianProvince), ErrorMessage = "Invalid Canadian province.")]
        public CanadianProvince Province { get; set; }


        public ICollection<Laptop> Laptops { get; set; } = new HashSet<Laptop>();
    }

    public enum CanadianProvince
    {
        Alberta,
        BritishColumbia,
        Manitoba,
        NewBrunswick,
        NewfoundlandAndLabrador,
        NovaScotia,
        Ontario,
        PrinceEdwardIsland,
        Quebec,
        Saskatchewan
    }
}


