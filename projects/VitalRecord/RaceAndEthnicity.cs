using System.Collections.Generic;
using Hl7.Fhir.ElementModel.Types;

namespace VR
{
    /// <summary> String representations of IJE Race fields </summary>
    public static class NvssRace
    {
        /// <summary> White </summary>
        public const string White = "White";
        /// <summary> BlackOrAfricanAmerican </summary>
        public const string BlackOrAfricanAmerican = "BlackOrAfricanAmerican";
        /// <summary> AmericanIndianOrAlaskanNative </summary>
        public const string AmericanIndianOrAlaskanNative = "AmericanIndianOrAlaskanNative";
        /// <summary> AsianIndian </summary>
        public const string AsianIndian = "AsianIndian";
        /// <summary> Chinese </summary>
        public const string Chinese = "Chinese";
        /// <summary> Filipino </summary>
        public const string Filipino = "Filipino";
        /// <summary> Japanese </summary>
        public const string Japanese = "Japanese";
        /// <summary> Korean </summary>
        public const string Korean = "Korean";
        /// <summary> Vietnamese </summary>
        public const string Vietnamese = "Vietnamese";
        /// <summary> OtherAsian </summary>
        public const string OtherAsian = "OtherAsian";
        /// <summary> NativeHawaiian </summary>
        public const string NativeHawaiian = "NativeHawaiian";
        /// <summary> GuamanianOrChamorro </summary>
        public const string GuamanianOrChamorro = "GuamanianOrChamorro";
        /// <summary> Samoan </summary>
        public const string Samoan = "Samoan";
        /// <summary> OtherPacificIslander </summary>
        public const string OtherPacificIslander = "OtherPacificIslander";
        /// <summary> OtherRace </summary>
        public const string OtherRace = "OtherRace";
        /// <summary> FirstAmericanIndianOrAlaskanNativeLiteral </summary>
        public const string FirstAmericanIndianOrAlaskanNativeLiteral = "FirstAmericanIndianOrAlaskanNativeLiteral";
        /// <summary> SecondAmericanIndianOrAlaskanNativeLiteral </summary>
        public const string SecondAmericanIndianOrAlaskanNativeLiteral = "SecondAmericanIndianOrAlaskanNativeLiteral";
        /// <summary> FirstOtherAsianLiteralFirst </summary>
        public const string FirstOtherAsianLiteral = "FirstOtherAsianLiteral";
        /// <summary> SecondOtherPacificIslanderLiteral </summary>
        public const string SecondOtherAsianLiteral = "SecondOtherAsianLiteral";
        /// <summary> FirstOtherPacificIslanderLiteral </summary>
        public const string FirstOtherPacificIslanderLiteral = "FirstOtherPacificIslanderLiteral";
        /// <summary> SecondOtherPacificIslanderLiteral </summary>
        public const string SecondOtherPacificIslanderLiteral = "SecondOtherPacificIslanderLiteral";
        /// <summary> FirstOtherRaceLiteral </summary>
        public const string FirstOtherRaceLiteral = "FirstOtherRaceLiteral";
        /// <summary> SecondOtherRaceLiteral </summary>
        public const string SecondOtherRaceLiteral = "SecondOtherRaceLiteral";
        /// <summary> MissingValueReason </summary>
        public const string MissingValueReason = "MissingValueReason";
        /// <summary> FirstEditedCode </summary>
        public const string FirstEditedCode = "FirstEditedCode";
        /// <summary> FirstEditedCode Display </summary>
        public const string FirstEditedCodeDisplay = "First Edited Code";
        /// <summary> SecondEditedCode </summary>
        public const string SecondEditedCode = "SecondEditedCode";
        /// <summary> SecondEditedCode Display </summary>
        public const string SecondEditedCodeDisplay = "Second Edited Code";
        /// <summary> ThirdEditedCode </summary>
        public const string ThirdEditedCode = "ThirdEditedCode";
        /// <summary> ThirdEditedCode Display </summary>
        public const string ThirdEditedCodeDisplay = "Third Edited Code";
        /// <summary> FirstEditedCode </summary>
        public const string FourthEditedCode = "FourthEditedCode";
        /// <summary> FourthEditedCode Display </summary>
        public const string FourthEditedCodeDisplay = "Fourth Edited Code";
        /// <summary> FirstEditedCode </summary>
        public const string FifthEditedCode = "FifthEditedCode";
        /// <summary> FifthEditedCode Display </summary>
        public const string FifthEditedCodeDisplay = "Fifth Edited Code";
        /// <summary> SixthEditedCode </summary>
        public const string SixthEditedCode = "SixthEditedCode";
        /// <summary> SixthEditedCode Display </summary>
        public const string SixthEditedCodeDisplay = "Sixth Edited Code";
        /// <summary> SeventhEditedCode </summary>
        public const string SeventhEditedCode = "SeventhEditedCode";
        /// <summary> SeventhEditedCode Display </summary>
        public const string SeventhEditedCodeDisplay = "Seventh Edited Code";
        /// <summary> EighthEditedCode </summary>
        public const string EighthEditedCode = "EighthEditedCode";
        /// <summary> EighthEditedCode Display </summary>
        public const string EighthEditedCodeDisplay = "Eighth Edited Code";
        /// <summary> FirstAmericanIndianCode </summary>
        public const string FirstAmericanIndianCode = "FirstAmericanIndianCode";
        /// <summary> FirstAmericanIndianCode Display </summary>
        public const string FirstAmericanIndianCodeDisplay = "First American Indian Code";
        /// <summary> SecondAmericanIndianCode </summary>
        public const string SecondAmericanIndianCode = "SecondAmericanIndianCode";
        /// <summary> SecondAmericanIndian Display </summary>
        public const string SecondAmericanIndianCodeDisplay = "Second American Indian Code";
        /// <summary> FirstOtherAsianCode </summary>
        public const string FirstOtherAsianCode = "FirstOtherAsianCode";
        /// <summary> FirstOtherAsianCode Display </summary>
        public const string FirstOtherAsianCodeDisplay = "First Other Asian Code";
        /// <summary> SecondOtherAsianCode </summary>
        public const string SecondOtherAsianCode = "SecondOtherAsianCode";
        /// <summary> SecondOtherAsianCode Display </summary>
        public const string SecondOtherAsianCodeDisplay = "Second Other Asian Code";
        /// <summary> FirstOtherPacificIslander </summary>
        public const string FirstOtherPacificIslanderCode = "FirstOtherPacificIslanderCode";
        /// <summary> FirstOtherPacificIslanderCode Display </summary>
        public const string FirstOtherPacificIslanderCodeDisplay = "First Other Pacific Islander Code";
        /// <summary> SecondOtherPacificIslanderCode </summary>
        public const string SecondOtherPacificIslanderCode = "SecondOtherPacificIslanderCode";
        /// <summary> SecondOtherPacificIslanderCode Display </summary>
        public const string SecondOtherPacificIslanderCodeDisplay = "Second Other Pacific Islander Code";
        /// <summary> FirstOtherRaceCode </summary>
        public const string FirstOtherRaceCode = "FirstOtherRaceCode";
        /// <summary> FirstOtherRaceCode Display </summary>
        public const string FirstOtherRaceCodeDisplay = "First Other Race Code";
        /// <summary> SecondOtherRaceCode </summary>
        public const string SecondOtherRaceCode = "SecondOtherRaceCode";
        /// <summary> SecondOtherRaceCode Display </summary>
        public const string SecondOtherRaceCodeDisplay = "Second Other Race Code";


        /// <summary> GetBooleanRaceCodes Returns a list of the Boolean Race Codes, Y or N values </summary>
        public static List<string> GetBooleanRaceCodes()
        {
            List<string> booleanRaceCodes = new List<string>();
            booleanRaceCodes.Add(NvssRace.White);
            booleanRaceCodes.Add(NvssRace.BlackOrAfricanAmerican);
            booleanRaceCodes.Add(NvssRace.AmericanIndianOrAlaskanNative);
            booleanRaceCodes.Add(NvssRace.AsianIndian);
            booleanRaceCodes.Add(NvssRace.Chinese);
            booleanRaceCodes.Add(NvssRace.Filipino);
            booleanRaceCodes.Add(NvssRace.Japanese);
            booleanRaceCodes.Add(NvssRace.Korean);
            booleanRaceCodes.Add(NvssRace.Vietnamese);
            booleanRaceCodes.Add(NvssRace.OtherAsian);
            booleanRaceCodes.Add(NvssRace.NativeHawaiian);
            booleanRaceCodes.Add(NvssRace.GuamanianOrChamorro);
            booleanRaceCodes.Add(NvssRace.Samoan);
            booleanRaceCodes.Add(NvssRace.OtherPacificIslander);
            booleanRaceCodes.Add(NvssRace.OtherRace);
            return booleanRaceCodes;
        }
        /// <summary> GetDisplayValueForCode returns the display value for a race code, or the code itself if none exists</summary>
        public static string GetDisplayValueForCode(string code)
        {
            switch (code)
            {
                case BlackOrAfricanAmerican:
                    return "Black Or African American";
                case AmericanIndianOrAlaskanNative:
                    return "American Indian Or Alaskan Native";
                case AsianIndian:
                    return "Asian Indian";
                case OtherAsian:
                    return "Other Asian";
                case NativeHawaiian:
                    return "Native Hawaiian";
                case GuamanianOrChamorro:
                    return "Guamanian Or Chamorro";
                case OtherPacificIslander:
                    return "Other Pacific Islander";
                case OtherRace:
                    return "Other Race";
                default:
                    return code;
            }
        }
        /// <summary> GetLiteralRaceCodes Returns a list of the literal Race Codes</summary>
        public static List<string> GetLiteralRaceCodes()
        {
            List<string> literalRaceCodes = new List<string>();
            literalRaceCodes.Add(NvssRace.FirstAmericanIndianOrAlaskanNativeLiteral);
            literalRaceCodes.Add(NvssRace.SecondAmericanIndianOrAlaskanNativeLiteral);
            literalRaceCodes.Add(NvssRace.FirstOtherAsianLiteral);
            literalRaceCodes.Add(NvssRace.SecondOtherAsianLiteral);
            literalRaceCodes.Add(NvssRace.FirstOtherPacificIslanderLiteral);
            literalRaceCodes.Add(NvssRace.SecondOtherPacificIslanderLiteral);
            literalRaceCodes.Add(NvssRace.FirstOtherRaceLiteral);
            literalRaceCodes.Add(NvssRace.SecondOtherRaceLiteral);
            return literalRaceCodes;
        }
    };
    /// <summary> String representations of IJE Ethnicity fields </summary>
    public static class NvssEthnicity
    {
        /// <summary> Mexican </summary>
        public const string Mexican = "HispanicMexican";
        /// <summary> Hispanic Mexican </summary>
        public const string MexicanDisplay = "Hispanic Mexican";
        /// <summary> Puerto Rican </summary>
        public const string PuertoRican = "HispanicPuertoRican";
        /// <summary> Hispanic Puerto Rican </summary>
        public const string PuertoRicanDisplay = "Hispanic Puerto Rican";
        /// <summary> Cuban </summary>
        public const string Cuban = "HispanicCuban";
        /// <summary> Hispanic Cuban </summary>
        public const string CubanDisplay = "Hispanic Cuban";
        /// <summary> Other </summary>
        public const string Other = "HispanicOther";
        /// <summary> Hispanic Other </summary>
        public const string OtherDisplay = "Hispanic Other";
        /// <summary> Edited Code </summary>
        public const string EditedCode = "HispanicCode";
        /// <summary> Edited Code Display </summary>
        public const string EditedCodeDisplay = "Hispanic Code";
        /// <summary> Literal </summary>
        public const string Literal = "HispanicLiteral";
        /// <summary> Hispanic Literal </summary>
        public const string LiteralDisplay = "Hispanic Literal";
        /// <summary> Code For Literal </summary>
        public const string CodeForLiteral = "HispanicCodeForLiteral";
        /// <summary> Hispanic Code for Literal </summary>
        public const string CodeForLiteralDisplay = "Hispanic Code for Literal";

    }


}