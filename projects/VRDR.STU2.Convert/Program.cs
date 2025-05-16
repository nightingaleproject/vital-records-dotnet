using System.Reflection;

namespace VRDR.CLI
{
    partial class Program
    {
        static string commands =
@"* VRDR Command Line Interface - commands
  - help:  prints this help message  (no arguments)
  - jsonstu2-to-stu3:  Read in an VRDR STU2.2 file and convert to STU3 (2 arguments: path to input STU2.2 json input and path to output STU3 json output)
  - rdtripstu2-to-stu3:  Round trip an STU3 file to STU2 and back and check equivalence (1 arguments: path to input STU3 input)
";
        static int Main(string[] args)
        {
            if ((args.Length == 0) || ((args.Length == 1) && (args[0] == "help")))
            {
                Console.WriteLine(commands);
            }
            else if (args.Length >= 3 && args[0] == "jsonstu2-to-stu3")
            {
                //  - jsonstu2-to-stu3:  Read in an VRDR STU2.2 file and convert to STU3
                Console.WriteLine($"Converting json file {args[1]} to json file {args[2]} for VRDR STU3 conformance");
                ConvertVersion(args[2], args[1], ConversionDirection.STU2toSTU3, DataFormat.JSON);
            }
            else if (args.Length >= 2 && args[0] == "rdtripstu2-to-stu3")
            {
                //  -rdtripstu2-to-stu3:  Roundtrip STU2 json file to STU3 and compare content
                DeathRecord d1, d2;
                Console.WriteLine($"Roundtrip STU2 json file {args[1]} to STU3 and compare content");

                ConvertVersion("./tempSTU3.json", args[1], ConversionDirection.STU2toSTU3, DataFormat.JSON);
                ConvertVersion("./tempSTU2.json", "./tempSTU3.json", ConversionDirection.STU3toSTU2, DataFormat.JSON);
                d1 = new DeathRecord(File.ReadAllText(args[1]));
                d2 = new DeathRecord(File.ReadAllText("./tempSTU2.json"));
                return (CompareTwo(d1, d2));
            }
            return 0;
        }
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
                    Dictionary<string, string>? oneDict = (Dictionary<string, string>?)property.GetValue(d1);
                    Dictionary<string, string>? twoDict = (Dictionary<string, string>?)property.GetValue(d2);
                    // Ignore empty entries in the dictionary so they don't throw off comparisons.
                    one = oneDict != null ? String.Join(", ", oneDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "") : "";
                    //two = String.Join(", ", twoDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                    two = twoDict != null ? String.Join(", ", twoDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "") : "";
                }
                else if (property.PropertyType.ToString() == "System.String[]")
                {
                    var oneVal = property.GetValue(d1);
                    one = oneVal != null ? String.Join(", ", (string[])oneVal) : "";
                    var twoVal = property.GetValue(d2);
                    two = twoVal != null ? String.Join(", ", (string[])twoVal) : "";
                }
                else
                {
                    one = Convert.ToString(property.GetValue(d1)) ?? "";
                    //two = Convert.ToString(property.GetValue(d2));
                    two = Convert.ToString(property.GetValue(d2)) ?? "";
                }
                if (one?.ToLower() != two?.ToLower())
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
    }
}