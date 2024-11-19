using System;
using System.Collections.Generic;
using System.Linq;
using VR;

namespace BFDR
{
    /// <summary>Base class for all natality IJE converter classes</summary>
    public abstract class IJENatality : IJE
    {
        /// <summary>Default constructor</summary>
        public IJENatality() {}

        /// <summary>Constructor that takes a <c>NatalityRecord</c>.</summary>
        public IJENatality(NatalityRecord record, bool validate = true) {}

        /// <summary>Constructor that takes an IJE string</summary>
        public IJENatality(string ije, bool validate = true) {}

        /// <summary>FHIR based natality record.</summary>
        protected abstract NatalityRecord NatalityRecord
        {
            get;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Class helper methods for getting and settings IJE fields.
        //
        /////////////////////////////////////////////////////////////////////////////////
        

        ///
        /// <summary>Sets FirstName.</summary>
        /// 
        protected string[] Update_FirstName(string value, string[] givenNames)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                givenNames = new string[] { value.Trim() };
            }
            return givenNames;
        }
        ///
        /// <summary>Sets Middle Name.</summary>
        /// 
        protected string[] Update_MiddleName(string value, string[] givenNames, string ijeFirstName)
        {
          if (!String.IsNullOrWhiteSpace(value))
          {
              if (String.IsNullOrWhiteSpace(ijeFirstName)) throw new ArgumentException("Middle name cannot be set before first name");
                  if (givenNames != null)
                  {
                      List<string> names = givenNames.ToList();
                      if (names.Count() > 1) names[1] = value.Trim(); else names.Add(value.Trim());
                      return names.ToArray();
                  }
              }
          return givenNames;
        }

        ///
        /// <summary>Checks if the given race exists in the record for Mother.</summary>
        /// 
        protected string Get_MotherRace(string name)
        {
            Tuple<string, string>[] raceStatus = NatalityRecord.MotherRace.ToArray();

            Tuple<string, string> raceTuple = Array.Find(raceStatus, element => element.Item1 == name);
            if (raceTuple != null)
            {
                return (raceTuple.Item2).Trim();
            }
            return "";
        }

        /// <summary>Adds the given race to the record for Mother.</summary>
        protected void Set_MotherRace(string name, string value)
        {
            value = value.Trim();
            List<Tuple<string, string>> raceStatus = NatalityRecord.MotherRace.ToList();
            raceStatus.Add(Tuple.Create(name, value));
            NatalityRecord.MotherRace = raceStatus.Distinct().ToArray();
        }

        /// <summary>Checks if the given race exists in the record for Father.</summary>
        protected string Get_FatherRace(string name)
        {
            Tuple<string, string>[] raceStatus = NatalityRecord.FatherRace.ToArray();

            Tuple<string, string> raceTuple = Array.Find(raceStatus, element => element.Item1 == name);
            if (raceTuple != null)
            {
                return (raceTuple.Item2).Trim();
            }
            return "";
        }

        /// <summary>Adds the given race to the record for Father.</summary>
        protected void Set_FatherRace(string name, string value)
        {
            value = value.Trim();
            List<Tuple<string, string>> raceStatus = NatalityRecord.FatherRace.ToList();
            raceStatus.Add(Tuple.Create(name, value));
            NatalityRecord.FatherRace = raceStatus.Distinct().ToArray();
        }
    }
}
