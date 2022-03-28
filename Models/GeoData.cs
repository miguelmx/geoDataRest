using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Linq;
using System.Web;

namespace geoDataRest.Models
{
    public class GeoData
    {
		// main object model based on the CSV adding ID

        public int Id { get; set; }
		public string Region { get; set; }

		// I choose to pass the coordinates as text to convert them at the database side
		public string Origin_coord { get; set; }
		public string Destination_coord { get; set; }
		public DateTime Datetime { get; set; }
		public string Datasource { get; set; }
	}
}