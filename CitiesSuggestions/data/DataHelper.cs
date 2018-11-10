using System;
using System.Collections.Generic;
using System.IO;

namespace CitiesSuggestions
{
    public class DataHelper
    {
        public static List<CityData> LoadData()
        {
            var cities = new List<CityData>();
            var assmbly = System.Reflection.Assembly.GetExecutingAssembly();
            var stream = assmbly.GetManifestResourceStream("CitiesSuggestions.data.cities_canada-usa.tsv");
            StreamReader streamreader = new StreamReader(stream, System.Text.Encoding.UTF8);
            char[] delimiter = new char[] { '\t' };
            string[] columnheaders = streamreader.ReadLine().Split(delimiter);

            while (streamreader.Peek() > 0)
            {
                string[] fields = streamreader.ReadLine().Split(delimiter);
                var city = new CityData() { Id = Int32.Parse(fields[Array.IndexOf(columnheaders, "id")]),
                                            Name = fields[Array.IndexOf(columnheaders, "name")],
                                            Population = Int32.Parse(fields[Array.IndexOf(columnheaders, "population")]),
                                            CountryCode = fields[Array.IndexOf(columnheaders, "country")],
                                            Admin1Code = fields[Array.IndexOf(columnheaders, "admin1")],
                                            Location = new GeoCoordinatePortable.GeoCoordinate(
                                                Double.Parse(fields[Array.IndexOf(columnheaders, "lat")]),
                                                Double.Parse(fields[Array.IndexOf(columnheaders, "long")])
                                                )
                };
                cities.Add(city);
            }
            ReplaceNameWithFullName(cities);

            return cities;
        }

        private static void ReplaceNameWithFullName(List<CityData> cities)
        {
            Dictionary<string, string> countries = new Dictionary<string, string>();
            Dictionary<string, string> admin1 = new Dictionary<string, string>();

            countries.Add("CA", "Canada");
            countries.Add("US", "USA");

            // Canadian provinces
            // https://www.thoughtco.com/abbreviations-of-canadian-provinces-510809
            admin1.Add("01", "AB"); //Alberta
            admin1.Add("02", "BC"); //British Columbia  
            admin1.Add("03", "MB"); //Manitoba
            admin1.Add("04", "NB"); //New Brunswick
            admin1.Add("13", "NT"); //Northwest Territories
            admin1.Add("07", "NS"); //Nova Scotia
            admin1.Add("14", "NU"); //Nunavut
            admin1.Add("08", "ON"); //Ontario
            admin1.Add("09", "PE"); //Prince Edward Island
            admin1.Add("10", "QC"); //Quebec
            admin1.Add("11", "SK"); //Saskatchewan
            admin1.Add("12", "YT"); //Yukon
            admin1.Add("05", "NL"); //Newfoundland and Labrador

            // USA : same as admin1 code

            for(int i = 0; i<cities.Count; ++i)
            {
                string countryAddOn = cities[i].CountryCode; // fallback if not in dictionary
                if (countries.ContainsKey(cities[i].CountryCode))
                {
                    countryAddOn = countries[cities[i].CountryCode];
                }

                string admin1AddOn = cities[i].Admin1Code;
                if(admin1.ContainsKey(cities[i].Admin1Code))
                {
                    admin1AddOn = admin1[cities[i].Admin1Code];
                }

                cities[i].Name = $"{cities[i].Name}, {admin1AddOn}, {countryAddOn}";
            }
        }
    }
}
