using geoDataRest.Data;
using geoDataRest.Models;
using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace geoDataRest.Controllers
{
    public class GeodataController : ApiController
    {
        // GET api/<controller>
        public List<GeoData> Get()
        {
            // Testing only method, lists all rows in the database
            return GeoDataData.List();
        }

        // GET api/<controller>/5
        public int GetRegionAverage(string region)
        {
            // Returns the average weekly trips for a string Region
            return GeoDataData.GetRegionAverage(region);
        }

        // GET api/<controller>/5
        public int GetBoxAverage(string lat1, string long1, string lat2, string long2 )
        {
            // Returns the average weekly trips for a boxed region 
            return GeoDataData.GetBoxAverage(lat1, long1, lat2, long2);
        }

        // POST api/<controller>
        [HttpPost]
        public bool Post([FromBody] GeoData oGeodata)
        {
            // Main data ingesting method for single data, ideally for endpoints
            return GeoDataData.Register(oGeodata);
        }


        // Endpoint for CSV ingest
        [HttpPost]
        public IHttpActionResult PostCSV()
        {
            // We receive the CSV as a form-data file
            var httpRequest = HttpContext.Current.Request;
            var postedFile = httpRequest.Files["File"];

            if (postedFile == null)
                return InternalServerError();       // if we dont recive a file, return
            
            bool header = true; // flag to discard the column names

            using (StreamReader reader = new StreamReader(postedFile.InputStream))
            {
                do
                {
                    string textLine = reader.ReadLine();
                    string[] columns = textLine.Split(',');

                    // if the format is right, we should receive 5 columns
                    if(columns.Length==5)
                    {
                        // flag to discard the first line
                        if (!header)
                        {

                            // we populate the object from the data received 
                            GeoData oGD = new GeoData();
                            oGD.Region = columns[0];
                            oGD.Origin_coord = columns[1];
                            oGD.Destination_coord = columns[2];
                            oGD.Datetime = Convert.ToDateTime(columns[3]);
                            oGD.Datasource = columns[4];

                            // and register it with the stored procedure 
                            GeoDataData.Register(oGD);
                        }
                        else
                            header = false;
                    }
                } while (reader.Peek() != -1); // as long as we receive lines, keep going
            }
            return Ok();
        }
    }
}