using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Hl7.FhirPath;
using Newtonsoft.Json.Linq;
using JsonDiffPatchDotNet;


namespace VRDR.CLI
{
    partial class Program
    {

        // CompareTwo: Perform a field by field comparison of d1 to d2
        private static int CompareTwo(DeathRecord d1, DeathRecord d2)
        {
            int good = 0;
            int bad = 0;
            List<PropertyInfo> properties = typeof(DeathRecord).GetProperties().ToList();
            foreach (PropertyInfo property in properties)
            {
                // Console.WriteLine($"Property: Name: {property.Name.ToString()} Type: {property.PropertyType.ToString()}");
                string one;
                string two;
                if (property.PropertyType.ToString() == "System.Collections.Generic.Dictionary`2[System.String,System.String]")
                {
                    Dictionary<string, string> oneDict = (Dictionary<string, string>)property.GetValue(d1);
                    Dictionary<string, string> twoDict = (Dictionary<string, string>)property.GetValue(d2);
                    // Ignore empty entries in the dictionary so they don't throw off comparisons.
                    one = String.Join(", ", oneDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                    //two = String.Join(", ", twoDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                    two = String.Join(", ", twoDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                }
                else if (property.PropertyType.ToString() == "System.String[]")
                {
                    one = String.Join(", ", (string[])property.GetValue(d1));
                    two = String.Join(", ", (string[])property.GetValue(d2));
                }
                else
                {
                    one = Convert.ToString(property.GetValue(d1));
                    //two = Convert.ToString(property.GetValue(d2));
                    two = Convert.ToString(property.GetValue(d2));
                }
                if (one.ToLower() != two.ToLower())
                {
                    Console.WriteLine("[***** MISMATCH *****]\t" + $"\"{one}\" (property: {property.Name}) does not equal \"{two}\"" + $"      1:\"{one}\"  3:\"{two}\"");
                    bad++;
                }
                else
                {
                    // We don't actually need to see all the matches and it makes it hard to see the mismatches
                    // Console.WriteLine("[MATCH]\t" + $"\"{one}\" (property: {property.Name}) equals \"{three}\"" + $"      1:\"{one}\" 2:\"{two}\" 3:\"{three}\"");
                    good++;
                }
            }
            Console.WriteLine($"\n{bad} mismatches out of {good + bad} total properties checked.");
            if (bad > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        // CompareThree: Perform afield by field comparison of d1 to d2 to d3 and highlight differences
        private static int CompareThree(DeathRecord d1, DeathRecord d2, DeathRecord d3)
        {
            int good = 0;
            int bad = 0;
            List<PropertyInfo> properties = typeof(DeathRecord).GetProperties().ToList();
            HashSet<string> skipPropertyNames = new HashSet<string>() { "UsualOccupationCoded", "UsualIndustryCoded", "DeathRecordIdentifier", "CausesOfDeath", "AgeAtDeathYears", "AgeAtDeathMonths", "AgeAtDeathDays", "AgeAtDeathHours", "AgeAtDeathMinutes" };
            foreach (PropertyInfo property in properties)
            {
                // Console.WriteLine($"Property: Name: {property.Name.ToString()} Type: {property.PropertyType.ToString()}");
                string one;
                string two;
                string three;
                if (skipPropertyNames.Contains(property.Name))
                {
                    continue;
                }
                if (property.PropertyType.ToString() == "System.Collections.Generic.Dictionary`2[System.String,System.String]")
                {
                    Dictionary<string, string> oneDict = (Dictionary<string, string>)property.GetValue(d1);
                    Dictionary<string, string> twoDict = (Dictionary<string, string>)property.GetValue(d2);
                    Dictionary<string, string> threeDict = (Dictionary<string, string>)property.GetValue(d3);
                    // Ignore empty entries in the dictionary so they don't throw off comparisons.
                    one = String.Join(", ", oneDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                    two = String.Join(", ", twoDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                    three = String.Join(", ", threeDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                }
                else if (property.PropertyType.ToString() == "System.String[]")
                {
                    one = String.Join(", ", (string[])property.GetValue(d1));
                    two = String.Join(", ", (string[])property.GetValue(d2));
                    three = String.Join(", ", (string[])property.GetValue(d3));
                }
                else
                {
                    one = Convert.ToString(property.GetValue(d1));
                    two = Convert.ToString(property.GetValue(d2));
                    three = Convert.ToString(property.GetValue(d3));
                }
                if (one.ToLower() != three.ToLower())
                {
                    Console.WriteLine("[***** MISMATCH *****]\t" + $"\"{one}\" (property: {property.Name}) does not equal \"{three}\"" + $"      1:\"{one}\" 2:\"{two}\" 3:\"{three}\"");
                    bad++;
                }
                else
                {
                    // We don't actually need to see all the matches and it makes it hard to see the mismatches
                    // Console.WriteLine("[MATCH]\t" + $"\"{one}\" (property: {property.Name}) equals \"{three}\"" + $"      1:\"{one}\" 2:\"{two}\" 3:\"{three}\"");
                    good++;
                }
            }
            Console.WriteLine($"\n{bad} mismatches out of {good + bad} total properties checked.");
            if (bad > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }



        // CompareJsonIgnoringOrderAndSpacing:
        // Compare the content of two json strings ignoring differences due to node ordering or white space.
        static bool CompareJsonIgnoringOrderAndSpacing(string json1, string json2)
        {
            var jdp = new JsonDiffPatch();
            var left = JToken.Parse(json1);
            var right = JToken.Parse(json2);

            // Sort both JSON objects
            var sortedLeft = SortJson(left);
            var sortedRight = SortJson(right);
            // bool same = (String.Compare(sortedLeft.ToString(), sortedRight.ToString()) == 0);
            // string samestring = (same?"same":"different");
            // Console.WriteLine("Two Json strings are " + samestring);
            // File.WriteAllText("sortedLeft.json", sortedLeft.ToString());
            // File.WriteAllText("sortedRight.json", sortedRight.ToString());

            // Compare the sorted JSON objects
            var diff = jdp.Diff(sortedLeft, sortedRight);
            string samestring = (diff == null ? "same" : "different");
            // Print the diff to the console
            if (diff != null)
            {
                Console.WriteLine("Differences:");
                Console.WriteLine(diff.ToString());
            }
            return diff == null;
        }

        static JToken SortJson(JToken token)
        {
            if (token is JObject jObj)
            {
                var properties = jObj.Properties().ToList();
                var sortedObj = new JObject();
                foreach (var prop in properties.OrderBy(p => p.Name))
                {
                    sortedObj.Add(prop.Name, SortJson(prop.Value));
                }
                return sortedObj;
            }
            else if (token is JArray jArr)
            {
                return new JArray(jArr.Select(SortJson).OrderBy(t => t.ToString()));
            }
            else
            {
                return token;
            }
        }

    }
}