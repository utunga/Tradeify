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
/*
{
  "name": "1500 Amphitheatre Parkway, Mountain View, CA",
  "Status": {
    "code": 200,
    "request": "geocode"
  },
  "Placemark": [ {
    "id": "p1",
    "address": "1500 Amphitheatre Pkwy, Mountain View, CA 94043, USA",
    "AddressDetails": {
   "Accuracy" : 8,
   "Country" : {
      "AdministrativeArea" : {
         "AdministrativeAreaName" : "CA",
         "SubAdministrativeArea" : {
            "Locality" : {
               "LocalityName" : "Mountain View",
               "PostalCode" : {
                  "PostalCodeNumber" : "94043"
               },
               "Thoroughfare" : {
                  "ThoroughfareName" : "1500 Amphitheatre Pkwy"
               }
            },
            "SubAdministrativeAreaName" : "Santa Clara"
         }
      },
      "CountryName" : "USA",
      "CountryNameCode" : "US"
   }
},
    "ExtendedData": {
      "LatLonBox": {
        "north": 37.4264181,
        "south": 37.4201229,
        "east": -122.0773318,
        "west": -122.0836270
      }
    },
    "Point": {
      "coordinates": [ -122.0804822, 37.4232890, 0 ]
    }
  } ]
}
*/