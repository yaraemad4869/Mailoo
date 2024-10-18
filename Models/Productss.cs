using Mailo.Data.Enums;
namespace Mailo.Models
{
    public class Productss : Product
    {
        public Sizes Size { get; set; }
        public Colors Colors { get; set; }
        public int Quantity { get; set; }

    }
}
