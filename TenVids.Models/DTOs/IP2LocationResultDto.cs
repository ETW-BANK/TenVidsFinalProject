﻿
namespace TenVids.Models.DTOs
{
    public class IP2LocationResultDto
    {
        public string IP { get; set; }
        public string Country_Code { get; set; }
        public string Country_Name { get; set; }
        public string Region_Name { get; set; }
        public string City_Name { get; set; }
        public string Zip_Code { get; set; }
        public bool Is_Proxy { get; set; }

    }
}
