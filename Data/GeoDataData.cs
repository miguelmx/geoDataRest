using geoDataRest.Models;
using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.SqlServer;

namespace geoDataRest.Data
{
    public class GeoDataData
    {
        public static bool Register(GeoData geodata)
        {
            // Main procedure to ingest data, we receive the Geodata object and pass it as parameter to 
            // the register stored procedure

            using (SqlConnection oConnection = new SqlConnection(Connection.connectionDS))
            {
                // Using a command to call the stored procedure and adding the parameters
                SqlCommand cmd = new SqlCommand("sp_register_data", oConnection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@region", geodata.Region);
                cmd.Parameters.AddWithValue("@origin_coord", geodata.Origin_coord);
                cmd.Parameters.AddWithValue("@destination_coord", geodata.Destination_coord);
                cmd.Parameters.AddWithValue("@datetime", geodata.Datetime);
                cmd.Parameters.AddWithValue("@datasource", geodata.Datasource);
                try
                {
                    oConnection.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static List<GeoData> List()
        {
            // A list method to inspect the loaded data (testing only, not meant for production)
            List<GeoData> oListGeodata = new List<GeoData>();
            using (SqlConnection oConecction = new SqlConnection(Connection.connectionDS))
            {
                // A stored procedure with a plain select
                SqlCommand cmd = new SqlCommand("sp_list", oConecction);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                try
                {
                    oConecction.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            // we take the data reader values as objects and format them as text to show
                            // geography objects have lat and long values as floats
                            // we take values 2 and 3 for origin and destination coordinates

                            dynamic org = dr.GetValue(2);
                            dynamic dst = dr.GetValue(3);
                            var text = string.Format("POINT({0:R} {1:R})", org.Long, org.Lat);
                            var text2 = string.Format("POINT({0:R} {1:R})", dst.Long, dst.Lat);
                            
                            oListGeodata.Add(new GeoData()
                            {
                                Id = Convert.ToInt32(dr["id"]),
                                Region = Convert.ToString(dr["region"]),
                                Origin_coord = text,        // the transalated lat and long for origin
                                Destination_coord =text2,   // the transalated lat and long for destination
                                Datetime = Convert.ToDateTime(dr["datetime"]),
                                Datasource = Convert.ToString(dr["datasource"])
                            });
                        }
                    }
                    return oListGeodata; // finally, we return the object list
                }
                catch(Exception ex)
                {
                    return oListGeodata;
                }
            }
        }
    
        public static int  GetRegionAverage(string region)
        {
            // A simple gateway to read the average weekly trips for a defined region
            
            using (SqlConnection oConecction = new SqlConnection(Connection.connectionDS))
            {
                // We use a SqlCommand to call the stored procedure and pass the parameter
                SqlCommand cmd = new SqlCommand("[sp_get_region_average]", oConecction);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@region", region);

                try
                {
                    oConecction.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // We return the average weeekly trips ... an integer
                            return Convert.ToInt32(dr[0]);
                        }
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        // Get the weekly average trips inside a coordinates box
        public static int GetBoxAverage(string lat1, string long1, string lat2, string long2)
        {
            // Simple gateway to a SQL stored procedure. We provide box parameters as text 
            // so we can translate the values at the server 

            using (SqlConnection oConecction = new SqlConnection(Connection.connectionDS)) 
            {
                //A command to execute the stored procedure and its parameters
                SqlCommand cmd = new SqlCommand("[sp_get_box_average]", oConecction);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@lat1", lat1);
                cmd.Parameters.AddWithValue("@long1", long1);
                cmd.Parameters.AddWithValue("@lat2", lat2);
                cmd.Parameters.AddWithValue("@long2", long2);
                try
                {
                    oConecction.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // we return the weekly average trips... an integer
                            return Convert.ToInt32(dr[0]); 
                        }
                    }
                    return 0;

                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

    }
}