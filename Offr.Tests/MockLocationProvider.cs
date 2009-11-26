using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Offr.Location;

namespace Offr.Tests
{
   public  class MockLocationProvider:GoogleLocationProvider
    {

       #region json GooglResultSets
       #region cali
       private string cali =
           @"{
  ""name"": ""1600 Amphitheatre Parkway, Mountain View, CA"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""CA"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Mountain View"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""94043""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""1600 Amphitheatre Pkwy""
               }
            },
            ""SubAdministrativeAreaName"" : ""Santa Clara""
         }
      },
      ""CountryName"" : ""USA"",
      ""CountryNameCode"" : ""US""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 37.4251466,
        ""south"": 37.4188514,
        ""east"": -122.0811574,
        ""west"": -122.0874526
      }
    },
    ""Point"": {
      ""coordinates"": [ -122.0843700, 37.4217590, 0 ]
    }
  } ]
}";
       #endregion cali
       #region lambton

       private string lambton =
           @"{
  ""name"": ""20 Lambton Quay"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""20 Lambton Quay, Wellington 6011, New Zealand"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Wellington"",
         ""DependentLocality"" : {
            ""DependentLocalityName"" : ""Pipitea"",
            ""PostalCode"" : {
               ""PostalCodeNumber"" : ""6011""
            },
            ""Thoroughfare"" : {
               ""ThoroughfareName"" : ""20 Lambton Quay""
            }
         }
      },
      ""CountryName"" : ""New Zealand"",
      ""CountryNameCode"" : ""NZ""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": -41.2755477,
        ""south"": -41.2818430,
        ""east"": 174.7816820,
        ""west"": 174.7753867
      }
    },
    ""Point"": {
      ""coordinates"": [ 174.7785322, -41.2786929, 0 ]
    }
  } ]
}";
       #endregion lambton
       #region 20 Pitt Street,Sydney

       private string sydney = @"{
  ""name"": ""20 Pitt Street,Sydney"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""20 Pitt St, Sydney NSW 2000, Australia"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""NSW"",
         ""Locality"" : {
            ""LocalityName"" : ""Sydney"",
            ""PostalCode"" : {
               ""PostalCodeNumber"" : ""2000""
            },
            ""Thoroughfare"" : {
               ""ThoroughfareName"" : ""20 Pitt St""
            }
         }
      },
      ""CountryName"" : ""Australia"",
      ""CountryNameCode"" : ""AU""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": -33.8590423,
        ""south"": -33.8653375,
        ""east"": 151.2122955,
        ""west"": 151.2060002
      }
    },
    ""Point"": {
      ""coordinates"": [ 151.2091575, -33.8621908, 0 ]
    }
  } ]
}";
       #endregion sydney
       #region 30 Borough Rd, London

       private string london = @"{
  ""name"": ""30 Borough Rd, London"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""30 Borough Rd, Camberwell, Greater London SE1 0, UK"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Greater London"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Camberwell"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""SE1 0""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Borough Rd""
               }
            },
            ""SubAdministrativeAreaName"" : ""Southwark""
         }
      },
      ""CountryName"" : ""United Kingdom"",
      ""CountryNameCode"" : ""GB""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 51.5020438,
        ""south"": 51.4957486,
        ""east"": -0.0987282,
        ""west"": -0.1050234
      }
    },
    ""Point"": {
      ""coordinates"": [ -0.1018770, 51.4989035, 0 ]
    }
  } ]
}
";
       #endregion 30 Borough Rd, London
       #region Fitzroy street

       private string fitzroy = @"{
  ""name"": ""30 Fitzroy Street, New Plymouth"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""30 Fitzroy Rd, Taranaki 4312, New Zealand"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Taranaki"",
         ""DependentLocality"" : {
            ""DependentLocalityName"" : ""Fitzroy"",
            ""PostalCode"" : {
               ""PostalCodeNumber"" : ""4312""
            },
            ""Thoroughfare"" : {
               ""ThoroughfareName"" : ""30 Fitzroy Rd""
            }
         }
      },
      ""CountryName"" : ""New Zealand"",
      ""CountryNameCode"" : ""NZ""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": -39.0412202,
        ""south"": -39.0475154,
        ""east"": 174.1111772,
        ""west"": 174.1048820
      }
    },
    ""Point"": {
      ""coordinates"": [ 174.1080205, -39.0443705, 0 ]
    }
  } ]
}
";
       #endregion Fitzroy street
       #region France

       private string france = @"{
  ""name"": ""30+Rue+Baudin,+Paris,+France"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""30 Rue Baudin, 92400 Courbevoie, France"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Ile-de-France"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Courbevoie"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""92400""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Rue Baudin""
               }
            },
            ""SubAdministrativeAreaName"" : ""Hauts-de-Seine""
         }
      },
      ""CountryName"" : ""France"",
      ""CountryNameCode"" : ""FR""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 48.8991771,
        ""south"": 48.8928818,
        ""east"": 2.2546164,
        ""west"": 2.2483212
      }
    },
    ""Point"": {
      ""coordinates"": [ 2.2514747, 48.8960244, 0 ]
    }
  }, {
    ""id"": ""p2"",
    ""address"": ""30 Rue Baudin, 93700 Drancy, France"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Ile-de-France"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Drancy"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""93700""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Rue Baudin""
               }
            },
            ""SubAdministrativeAreaName"" : ""Seine-Saint-Denis""
         }
      },
      ""CountryName"" : ""France"",
      ""CountryNameCode"" : ""FR""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 48.9240886,
        ""south"": 48.9177934,
        ""east"": 2.4365100,
        ""west"": 2.4302148
      }
    },
    ""Point"": {
      ""coordinates"": [ 2.4333674, 48.9209465, 0 ]
    }
  }, {
    ""id"": ""p3"",
    ""address"": ""30 Rue Baudin, 93310 Le Pré-Saint-Gervais, France"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Ile-de-France"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Le Pré-Saint-Gervais"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""93310""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Rue Baudin""
               }
            },
            ""SubAdministrativeAreaName"" : ""Seine-Saint-Denis""
         }
      },
      ""CountryName"" : ""France"",
      ""CountryNameCode"" : ""FR""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 48.8911834,
        ""south"": 48.8848881,
        ""east"": 2.4082631,
        ""west"": 2.4019678
      }
    },
    ""Point"": {
      ""coordinates"": [ 2.4051252, 48.8880359, 0 ]
    }
  } ]
}
";

       #endregion France
       #region Dubai

       private string dubai = @"{
  ""name"": ""Sheikh+Zayed+Road,+Dubai,+UAE"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""Sheikh Zayed Rd - Dubai - United Arab Emirates"",
    ""AddressDetails"": {
   ""Accuracy"" : 6,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Dubai"",
         ""Locality"" : {
            ""LocalityName"" : ""Dubai"",
            ""Thoroughfare"" : {
               ""ThoroughfareName"" : ""Sheikh Zayed Rd""
            }
         }
      },
      ""CountryName"" : ""United Arab Emirates"",
      ""CountryNameCode"" : ""AE""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 25.2327938,
        ""south"": 25.0276677,
        ""east"": 55.3065861,
        ""west"": 55.1010852
      }
    },
    ""Point"": {
      ""coordinates"": [ 55.2071978, 25.1270151, 0 ]
    }
  }, {
    ""id"": ""p2"",
    ""address"": ""E 11 - Dubai - United Arab Emirates"",
    ""AddressDetails"": {
   ""Accuracy"" : 6,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Dubai"",
         ""Locality"" : {
            ""LocalityName"" : ""Dubai"",
            ""Thoroughfare"" : {
               ""ThoroughfareName"" : ""E 11""
            }
         }
      },
      ""CountryName"" : ""United Arab Emirates"",
      ""CountryNameCode"" : ""AE""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 25.2309623,
        ""south"": 25.2246670,
        ""east"": 55.2913885,
        ""west"": 55.2850932
      }
    },
    ""Point"": {
      ""coordinates"": [ 55.2876798, 25.2286509, 0 ]
    }
  }, {
    ""id"": ""p3"",
    ""address"": ""Sheikh Zayed Rd - Dubai - United Arab Emirates"",
    ""AddressDetails"": {
   ""Accuracy"" : 6,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Dubai"",
         ""Locality"" : {
            ""LocalityName"" : ""Dubai"",
            ""Thoroughfare"" : {
               ""ThoroughfareName"" : ""Sheikh Zayed Rd""
            }
         }
      },
      ""CountryName"" : ""United Arab Emirates"",
      ""CountryNameCode"" : ""AE""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 25.1748702,
        ""south"": 25.1685750,
        ""east"": 55.2456840,
        ""west"": 55.2393887
      }
    },
    ""Point"": {
      ""coordinates"": [ 55.2423692, 25.1714887, 0 ]
    }
  }, {
    ""id"": ""p4"",
    ""address"": ""E11 - Dubai - United Arab Emirates"",
    ""AddressDetails"": {
   ""Accuracy"" : 6,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Dubai"",
         ""Locality"" : {
            ""DependentLocality"" : {
               ""DependentLocalityName"" : ""المركز التجاري"",
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""E11""
               }
            },
            ""LocalityName"" : ""Dubai""
         }
      },
      ""CountryName"" : ""United Arab Emirates"",
      ""CountryNameCode"" : ""AE""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 25.2321143,
        ""south"": 25.2258190,
        ""east"": 55.2909234,
        ""west"": 55.2846281
      }
    },
    ""Point"": {
      ""coordinates"": [ 55.2877484, 25.2290042, 0 ]
    }
  } ]
}";

       #endregion Dubai
       #region Borough Rd

       private string borough = @"{
  ""name"": ""30 Borough Rd"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""30 Borough Rd, Currie, NC 28435, USA"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""NC"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Currie"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""28435""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Borough Rd""
               }
            },
            ""SubAdministrativeAreaName"" : ""Pender""
         }
      },
      ""CountryName"" : ""USA"",
      ""CountryNameCode"" : ""US""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 34.4740220,
        ""south"": 34.4677268,
        ""east"": -78.0933414,
        ""west"": -78.0996367
      }
    },
    ""Point"": {
      ""coordinates"": [ -78.0964955, 34.4708684, 0 ]
    }
  }, {
    ""id"": ""p2"",
    ""address"": ""30 Borough Rd, NH 03603, USA"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""NH"",
         ""SubAdministrativeArea"" : {
            ""PostalCode"" : {
               ""PostalCodeNumber"" : ""03603""
            },
            ""SubAdministrativeAreaName"" : ""Sullivan"",
            ""Thoroughfare"" : {
               ""ThoroughfareName"" : ""30 Borough Rd""
            }
         }
      },
      ""CountryName"" : ""USA"",
      ""CountryNameCode"" : ""US""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 43.3070973,
        ""south"": 43.3008021,
        ""east"": -72.3707296,
        ""west"": -72.3770249
      }
    },
    ""Point"": {
      ""coordinates"": [ -72.3738770, 43.3039426, 0 ]
    }
  }, {
    ""id"": ""p3"",
    ""address"": ""30 Borough Rd, Farmington, ME 04938, USA"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""ME"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Farmington"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""04938""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Borough Rd""
               }
            },
            ""SubAdministrativeAreaName"" : ""Franklin""
         }
      },
      ""CountryName"" : ""USA"",
      ""CountryNameCode"" : ""US""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 44.5566531,
        ""south"": 44.5503579,
        ""east"": -70.0888822,
        ""west"": -70.0951774
      }
    },
    ""Point"": {
      ""coordinates"": [ -70.0920265, 44.5534990, 0 ]
    }
  }, {
    ""id"": ""p4"",
    ""address"": ""30 Borough Rd, Concord, NH 03303, USA"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""NH"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Concord"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""03303""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Borough Rd""
               }
            },
            ""SubAdministrativeAreaName"" : ""Merrimack""
         }
      },
      ""CountryName"" : ""USA"",
      ""CountryNameCode"" : ""US""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 43.2695216,
        ""south"": 43.2632263,
        ""east"": -71.5834512,
        ""west"": -71.5897464
      }
    },
    ""Point"": {
      ""coordinates"": [ -71.5865978, 43.2663810, 0 ]
    }
  }, {
    ""id"": ""p5"",
    ""address"": ""30 Borough Rd, Jay, ME 04239, USA"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""ME"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Jay"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""04239""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Borough Rd""
               }
            },
            ""SubAdministrativeAreaName"" : ""Franklin""
         }
      },
      ""CountryName"" : ""USA"",
      ""CountryNameCode"" : ""US""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 44.5309720,
        ""south"": 44.5246767,
        ""east"": -70.1453110,
        ""west"": -70.1516062
      }
    },
    ""Point"": {
      ""coordinates"": [ -70.1484607, 44.5278311, 0 ]
    }
  }, {
    ""id"": ""p6"",
    ""address"": ""30 Borough Rd, Arlington, VT 05250, USA"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""VT"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Arlington"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""05250""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Borough Rd""
               }
            },
            ""SubAdministrativeAreaName"" : ""Bennington""
         }
      },
      ""CountryName"" : ""USA"",
      ""CountryNameCode"" : ""US""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 43.1161373,
        ""south"": 43.1098420,
        ""east"": -73.1000493,
        ""west"": -73.1063445
      }
    },
    ""Point"": {
      ""coordinates"": [ -73.1032060, 43.1129871, 0 ]
    }
  }, {
    ""id"": ""p7"",
    ""address"": ""30 Borough Rd, St Helens WA10 3, UK"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""St Helens"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""St Helens"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""WA10 3""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Borough Rd""
               }
            },
            ""SubAdministrativeAreaName"" : ""St Helens""
         }
      },
      ""CountryName"" : ""United Kingdom"",
      ""CountryNameCode"" : ""GB""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 53.4534003,
        ""south"": 53.4471051,
        ""east"": -2.7439203,
        ""west"": -2.7502156
      }
    },
    ""Point"": {
      ""coordinates"": [ -2.7470784, 53.4502559, 0 ]
    }
  }, {
    ""id"": ""p8"",
    ""address"": ""30 Borough Rd, Llwchwr, Swansea SA4 6, UK"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Swansea"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Llwchwr"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""SA4 6""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Borough Rd""
               }
            },
            ""SubAdministrativeAreaName"" : ""Swansea""
         }
      },
      ""CountryName"" : ""United Kingdom"",
      ""CountryNameCode"" : ""GB""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 51.6710257,
        ""south"": 51.6647304,
        ""east"": -4.0534078,
        ""west"": -4.0597030
      }
    },
    ""Point"": {
      ""coordinates"": [ -4.0565561, 51.6678853, 0 ]
    }
  }, {
    ""id"": ""p9"",
    ""address"": ""30 Borough Rd, Camberwell, Greater London SE1 0, UK"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Greater London"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Camberwell"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""SE1 0""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Borough Rd""
               }
            },
            ""SubAdministrativeAreaName"" : ""Southwark""
         }
      },
      ""CountryName"" : ""United Kingdom"",
      ""CountryNameCode"" : ""GB""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 51.5020438,
        ""south"": 51.4957486,
        ""east"": -0.0987282,
        ""west"": -0.1050234
      }
    },
    ""Point"": {
      ""coordinates"": [ -0.1018770, 51.4989035, 0 ]
    }
  }, {
    ""id"": ""p10"",
    ""address"": ""30 Borough Rd, Middlesbrough TS1 4, UK"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""Middlesbrough"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Middlesbrough"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""TS1 4""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""30 Borough Rd""
               }
            },
            ""SubAdministrativeAreaName"" : ""Middlesbrough""
         }
      },
      ""CountryName"" : ""United Kingdom"",
      ""CountryNameCode"" : ""GB""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 54.5762778,
        ""south"": 54.5699825,
        ""east"": -1.2360652,
        ""west"": -1.2423605
      }
    },
    ""Point"": {
      ""coordinates"": [ -1.2392113, 54.5731369, 0 ]
    }
  } ]
}
";

       #endregion Borough Rd
       #region Paekakariki
       string pae = @"{
  ""name"": ""Paekakariki"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""Paekakariki, Wellington, New Zealand"",
    ""AddressDetails"": {
   ""Accuracy"" : 4,
   ""Country"" : {
      ""CountryName"" : ""New Zealand"",
      ""CountryNameCode"" : ""NZ"",
      ""Locality"" : {
         ""LocalityName"" : ""Paekakariki""
      }
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": -40.9490804,
        ""south"": -41.0164664,
        ""east"": 175.0201377,
        ""west"": 174.8920783
      }
    },
    ""Point"": {
      ""coordinates"": [ 174.9561080, -40.9827820, 0 ]
    }
  } ]
}";
        #endregion Paekakariki
       #region Wellingtion City
       string welCity = @"{
  ""name"": ""Wellington City"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""Wellington, New Zealand"",
    ""AddressDetails"": {
   ""Accuracy"" : 4,
   ""Country"" : {
      ""CountryName"" : ""New Zealand"",
      ""CountryNameCode"" : ""NZ"",
      ""Locality"" : {
         ""LocalityName"" : ""Wellington""
      }
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": -41.2780951,
        ""south"": -41.2948638,
        ""east"": 174.7922244,
        ""west"": 174.7602096
      }
    },
    ""Point"": {
      ""coordinates"": [ 174.7762170, -41.2864800, 0 ]
    }
  } ]
}";
#endregion Wellington City
       #region Waiheke Island

       string waiheke = @"{
  ""name"": ""Waiheke Island"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""Waiheke Island, New Zealand"",
    ""AddressDetails"": {
   ""Accuracy"" : 1,
   ""Country"" : {
      ""AddressLine"" : [ ""Waiheke Island"" ],
      ""CountryName"" : ""New Zealand"",
      ""CountryNameCode"" : ""NZ""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": -36.7280038,
        ""south"": -36.8709575,
        ""east"": 175.2241170,
        ""west"": 174.9679982
      }
    },
    ""Point"": {
      ""coordinates"": [ 175.0960576, -36.7995140, 0 ]
    }
  } ]
}";
       
       #endregion Waiheke Island

       #endregion json resultset
       
       private Dictionary<string, GoogleResultSet> dummyResultSetMap;
       public MockLocationProvider()
       {
           dummyResultSetMap=new Dictionary<string, GoogleResultSet>();
           dummyResultSetMap.Add("1600 Amphitheatre Parkway, Mountain View, CA",deserialize(cali));
           dummyResultSetMap.Add("20 Lambton Quay", deserialize(lambton));
           dummyResultSetMap.Add("20 Pitt Street,Sydney", deserialize(sydney));
           dummyResultSetMap.Add("30 Borough Rd, London", deserialize(london));
           dummyResultSetMap.Add("30 Fitzroy Street, New Plymouth", deserialize(fitzroy));
           dummyResultSetMap.Add("30+Rue+Baudin,+Paris,+France",deserialize(france));
           dummyResultSetMap.Add("Sheikh+Zayed+Road,+Dubai,+UAE",deserialize(dubai));
           dummyResultSetMap.Add("30 Borough Rd",deserialize(borough));
           dummyResultSetMap.Add("Paekakariki", deserialize(pae));
           dummyResultSetMap.Add("Wellington City", deserialize(welCity));
           dummyResultSetMap.Add("Waiheke Island", deserialize(waiheke));

       }
       private GoogleResultSet deserialize(string data)
       {
           return (new JavaScriptSerializer()).Deserialize<GoogleResultSet>(data);

       }
       public override ILocation Parse(string addressText)
        {
            return Parse(addressText, null);
        }

       public override ILocation Parse(string addressText, string twitterLocation)
          {
              ILocation previouslyFound = LocationRepository.Get(addressText);
              if (previouslyFound != null) return previouslyFound;
              GoogleResultSet resultSet=null;
              try
              {
                  resultSet = dummyResultSetMap[addressText];
              }
              catch (KeyNotFoundException)
              {
                  Console.Write("Had to get Address from Google: \n"+addressText);
                  resultSet = GetResultSet(addressText);
              }
              return GetNewLocation(addressText, twitterLocation, resultSet);
          }
    }
}
