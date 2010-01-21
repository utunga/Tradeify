namespace Offr.Location
{
    public class GoogleResultSet
    {
        public string name { get; set; }
        public StatusType Status { get; set; }

        public class StatusType
        {
            public string code { get; set; }
            public string request { get; set; }
        }

        public PlacemarkType[] Placemark { get; set; }

        public class PlacemarkType
        {
            public string id { get; set; }
            public string address { get; set; }
            public AddressDetailsType AddressDetails { get; set; }
            public class AddressDetailsType
            {
                public string accuracy { get; set; }
                public CountryType Country { get; set; }
                public class CountryType
                {
                    public AdministrativeAreaType.DependentLocalityType DependentLocality { get; set; }
                    public AdministrativeAreaType AdministrativeArea { get; set; }
                    public AdministrativeAreaType.SubAdministrativeAreaType.LocalityType Locality { get; set; }
                    public class AdministrativeAreaType{
                        public SubAdministrativeAreaType.LocalityType Locality{ get; set;}
                    
                        public string AdministrativeAreaName { get; set; }
                        public SubAdministrativeAreaType SubAdministrativeArea { get; set; }

                        public class SubAdministrativeAreaType
                        {
                            public string SubAdministrativeAreaName { get; set; }
                            public LocalityType Locality { get; set; }
                            public class LocalityType
                            {
                                public string LocalityName{ get; set;}
                                public PostalCodeType PostalCode { get; set; }
                                public ThoroughfareType Thoroughfare { get; set; }
                            }
                        }
                        public DependentLocalityType DependentLocality { get; set; }
                        public class DependentLocalityType
                        {
                            public string DependentLocalityName { get; set; }
                            public PostalCodeType PostalCode { get; set; }
                            public ThoroughfareType Thoroughfare { get; set; }
                        }
                        public class PostalCodeType
                        {
                            public string PostalCodeNumber { get; set; }
                        }
                        public class ThoroughfareType
                        {
                            public string ThoroughfareName { get; set; }
                        }
                    }
                    public string CountryName { get; set; }
                    public string CountryNameCode { get; set; }
                }
  
            }
            public ExtendedDataType ExtendedData { get; set; }
            public class ExtendedDataType
            {
                public LatLonBoxType LatLonBox { get; set; }

                public class LatLonBoxType
                {
                    public decimal north { get; set; }
                    public decimal south { get; set; }
                    public decimal east { get; set; }
                    public decimal west { get; set; }
                }
            }
            public PointType Point { get; set; }
            public class PointType
            {
                public decimal[] coordinates { get; set; }
            }
        }

        
       
        


    }

}
