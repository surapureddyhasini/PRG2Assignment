//==========================================================
// Student Number : S10273987F
// Student Name : Surapureddy Hasini
// Partner Name : Ng Jia Ying
//==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10273987F_PRG2Assignment
{ 
    class SpecialOffer //each SPECIALOFFER belongs to one RESTUARANT
{ 

    public string OfferCode { get; set; }
    public string OfferDesc { get; set; }
    public double Discount { get; set; }


        // Constructor
    public SpecialOffer(string offerCode, string offerDesc, double discount)
        {
            OfferCode = offerCode;
            OfferDesc = offerDesc;
            Discount = discount;
        }

        public override string ToString()
    {
        return $"Offer Code: {OfferCode}\nDescription: {OfferDesc}\nDiscount: {Discount}%";
    }
}
}