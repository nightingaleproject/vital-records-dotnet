using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;
using System.Net.Http;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.ElementModel;
using Hl7.FhirPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRDR;
using VR;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace VRDR.CLI
{
    partial class Program
    {

            // CompareTwo: Perform a field by field comparison of d1 to d3
            private static int CompareTwo(DeathRecord d1, DeathRecord d3)
            {
                int good = 0;
                int bad = 0;
                List<PropertyInfo> properties = typeof(DeathRecord).GetProperties().ToList();
                foreach (PropertyInfo property in properties)
                {
                    // Console.WriteLine($"Property: Name: {property.Name.ToString()} Type: {property.PropertyType.ToString()}");
                    string one;
                    // string two;
                    string three;
                    if (property.PropertyType.ToString() == "System.Collections.Generic.Dictionary`2[System.String,System.String]")
                    {
                        Dictionary<string, string> oneDict = (Dictionary<string, string>)property.GetValue(d1);
                        //Dictionary<string, string> twoDict = (Dictionary<string, string>)property.GetValue(d2);
                        Dictionary<string, string> threeDict = (Dictionary<string, string>)property.GetValue(d3);
                        // Ignore empty entries in the dictionary so they don't throw off comparisons.
                        one = String.Join(", ", oneDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                        //two = String.Join(", ", twoDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                        three = String.Join(", ", threeDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                    }
                    else if (property.PropertyType.ToString() == "System.String[]")
                    {
                        one = String.Join(", ", (string[])property.GetValue(d1));
                        //two = String.Join(", ", (string[])property.GetValue(d2));
                        three = String.Join(", ", (string[])property.GetValue(d3));
                    }
                    else
                    {
                        one = Convert.ToString(property.GetValue(d1));
                        //two = Convert.ToString(property.GetValue(d2));
                        three = Convert.ToString(property.GetValue(d3));
                    }
                    if (one.ToLower() != three.ToLower())
                    {
                        Console.WriteLine("[***** MISMATCH *****]\t" + $"\"{one}\" (property: {property.Name}) does not equal \"{three}\"" + $"      1:\"{one}\"  3:\"{three}\"");
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

            // CompareTwo: Perform afield by field comparison of d1 to d2 to d3 and highlight differences
            private static int CompareThree(DeathRecord d1, DeathRecord d2, DeathRecord d3)
            {
                int good = 0;
                int bad = 0;
                List<PropertyInfo> properties = typeof(DeathRecord).GetProperties().ToList();
                foreach (PropertyInfo property in properties)
                {
                    // Console.WriteLine($"Property: Name: {property.Name.ToString()} Type: {property.PropertyType.ToString()}");
                    string one;
                    string two;
                    string three;
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
    }
}