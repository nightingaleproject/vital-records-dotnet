using Xunit;
using BFDR;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BFDR.Tests
{
    public class ValueSets_Should
    {
        [Fact]
        public void ApgarTiming_Should_Have_Correct_Codes()
        {
            // Test that ApgarTiming codes array has expected entries
            Assert.Equal(5, ValueSets.ApgarTiming.Codes.GetLength(0));

            // Test specific code constants
            Assert.Equal("9272-6", ValueSets.ApgarTiming._1_Minute_Apgar_Score);
            Assert.Equal("9274-2", ValueSets.ApgarTiming._5_Minute_Apgar_Score);
            Assert.Equal("9271-8", ValueSets.ApgarTiming._10_Minute_Apgar_Score);
            Assert.Equal("443848000", ValueSets.ApgarTiming.Apgar_Score_At_15_Minutes_Observable_Entity);
            Assert.Equal("443849008", ValueSets.ApgarTiming.Apgar_Score_At_20_Minutes_Observable_Entity);
        }

        [Fact]
        public void ApgarTiming_Codes_Array_Should_Contain_Expected_Values()
        {
            var codes = ValueSets.ApgarTiming.Codes;

            // Check first row
            Assert.Equal("9272-6", codes[0, 0]);
            Assert.Equal("1 minute Apgar Score", codes[0, 1]);
            Assert.Equal(VR.CodeSystems.LOINC, codes[0, 2]);

            // Check last row
            Assert.Equal("443849008", codes[4, 0]);
            Assert.Equal("Apgar score at 20 minutes (observable entity)", codes[4, 1]);
            Assert.Equal(VR.CodeSystems.SCT, codes[4, 2]);
        }

        [Fact]
        public void BirthAndFetalDeathFinancialClass_Should_Have_Correct_Codes()
        {
            Assert.Equal(8, ValueSets.BirthAndFetalDeathFinancialClass.Codes.GetLength(0));

            Assert.Equal("33", ValueSets.BirthAndFetalDeathFinancialClass.Indian_Health_Service_Or_Tribe);
            Assert.Equal("2", ValueSets.BirthAndFetalDeathFinancialClass.Medicaid);
            Assert.Equal("99", ValueSets.BirthAndFetalDeathFinancialClass.No_Typology_Code_Available_For_Payment_Source);
            Assert.Equal("38", ValueSets.BirthAndFetalDeathFinancialClass.Other_Government_Federal_State_Local_Not_Specified);
            Assert.Equal("5", ValueSets.BirthAndFetalDeathFinancialClass.Private_Health_Insurance);
            Assert.Equal("81", ValueSets.BirthAndFetalDeathFinancialClass.Self_Pay);
            Assert.Equal("311", ValueSets.BirthAndFetalDeathFinancialClass.Tricare_Champus);
            Assert.Equal("9999", ValueSets.BirthAndFetalDeathFinancialClass.Unavailable_Unknown);
        }

        [Fact]
        public void BirthAttendantTitles_Should_Have_Correct_Codes()
        {
            Assert.Equal(6, ValueSets.BirthAttendantTitles.Codes.GetLength(0));

            Assert.Equal("112247003", ValueSets.BirthAttendantTitles.Medical_Doctor);
            Assert.Equal("76231001", ValueSets.BirthAttendantTitles.Osteopath);
            Assert.Equal("445521000124102", ValueSets.BirthAttendantTitles.Advanced_Practice_Midwife);
            Assert.Equal("445531000124104", ValueSets.BirthAttendantTitles.Lay_Midwife);
            Assert.Equal("OTH", ValueSets.BirthAttendantTitles.Other);
            Assert.Equal("UNK", ValueSets.BirthAttendantTitles.Unknown);
        }

        [Fact]
        public void BirthDeliveryOccurred_Should_Have_Correct_Codes()
        {
            Assert.Equal(8, ValueSets.BirthDeliveryOccurred.Codes.GetLength(0));

            Assert.Equal("22232009", ValueSets.BirthDeliveryOccurred.Hospital);
            Assert.Equal("91154008", ValueSets.BirthDeliveryOccurred.Free_Standing_Birthing_Center);
            Assert.Equal("408839006", ValueSets.BirthDeliveryOccurred.Planned_Home_Birth);
            Assert.Equal("408838003", ValueSets.BirthDeliveryOccurred.Unplanned_Home_Birth);
            Assert.Equal("67190003", ValueSets.BirthDeliveryOccurred.Free_Standing_Clinic);
            Assert.Equal("unknownplannedhomebirth", ValueSets.BirthDeliveryOccurred.Unknown_If_Planned_Home_Birth);
            Assert.Equal("OTH", ValueSets.BirthDeliveryOccurred.Other);
            Assert.Equal("UNK", ValueSets.BirthDeliveryOccurred.Unknown);
        }

        [Fact]
        public void BirthWeightEditFlags_Should_Have_Correct_Codes()
        {
            Assert.Equal(3, ValueSets.BirthWeightEditFlags.Codes.GetLength(0));

            Assert.Equal("0off", ValueSets.BirthWeightEditFlags.Off);
            Assert.Equal("1correctOutOfRange", ValueSets.BirthWeightEditFlags.Queried_Data_Correct_Out_Of_Range);
            Assert.Equal("2failedBirthWeightGestationEdit", ValueSets.BirthWeightEditFlags.Queried_Failed_Birthweight_Gestation_Edit);
        }

        [Fact]
        public void DeliveryRoutes_Should_Have_Correct_Codes()
        {
            Assert.Equal(4, ValueSets.DeliveryRoutes.Codes.GetLength(0));

            Assert.Equal("700000006", ValueSets.DeliveryRoutes.Vaginal_Delivery_Of_Fetus_Procedure);
            Assert.Equal("302383004", ValueSets.DeliveryRoutes.Forceps_Delivery_Procedure);
            Assert.Equal("61586001", ValueSets.DeliveryRoutes.Delivery_By_Vacuum_Extraction_Procedure);
            Assert.Equal("11466000", ValueSets.DeliveryRoutes.Cesarean_Section_Procedure);
        }

        [Fact]
        public void EstimateOfGestationEditFlags_Should_Have_Correct_Codes()
        {
            Assert.Equal(2, ValueSets.EstimateOfGestationEditFlags.Codes.GetLength(0));

            Assert.Equal("0off", ValueSets.EstimateOfGestationEditFlags.Off);
            Assert.Equal("1correctOutOfRange", ValueSets.EstimateOfGestationEditFlags.Queried_Data_Correct_Out_Of_Range);
        }

        [Fact]
        public void FertilityEnhancingDrugTherapyArtificialInsem_Should_Have_Correct_Codes()
        {
            Assert.Equal(3, ValueSets.FertilityEnhancingDrugTherapyArtificialInsem.Codes.GetLength(0));

            Assert.Equal("58533008", ValueSets.FertilityEnhancingDrugTherapyArtificialInsem.Artificial_Insemination_Procedure);
            Assert.Equal("265064001", ValueSets.FertilityEnhancingDrugTherapyArtificialInsem.Intrauterine_Artificial_Insemination_Procedure);
            Assert.Equal("445151000124101", ValueSets.FertilityEnhancingDrugTherapyArtificialInsem.Fertility_Enhancing_Drug_Therapy_Procedure_Procedure);
        }

        [Fact]
        public void FetalDeathCauseOrCondition_Should_Have_Correct_Codes()
        {
            Assert.Equal(13, ValueSets.FetalDeathCauseOrCondition.Codes.GetLength(0));

            Assert.Equal("44223004", ValueSets.FetalDeathCauseOrCondition.Premature_Rupture_Of_Membranes_Disorder);
            Assert.Equal("415105001", ValueSets.FetalDeathCauseOrCondition.Placental_Abruption_Disorder);
            Assert.Equal("237292005", ValueSets.FetalDeathCauseOrCondition.Placental_Insufficiency_Disorder);
            Assert.Equal("270500004", ValueSets.FetalDeathCauseOrCondition.Prolapsed_Cord_Disorder);
            Assert.Equal("11612004", ValueSets.FetalDeathCauseOrCondition.Chorioamnionitis_Disorder);
            Assert.Equal("702709008", ValueSets.FetalDeathCauseOrCondition.Congenital_Anomaly_Of_Fetus_Specify_Disorder);
            Assert.Equal("277489001", ValueSets.FetalDeathCauseOrCondition.Fetal_Trauma_Specify_Disorder);
            Assert.Equal("128270001", ValueSets.FetalDeathCauseOrCondition.Infectious_Disorder_Of_The_Fetus_Specify_Disorder);
            Assert.Equal("FCOD_maternalconditions", ValueSets.FetalDeathCauseOrCondition.Maternal_Conditions_Diseases_Specify_Disorder);
            Assert.Equal("FCOD_membranes", ValueSets.FetalDeathCauseOrCondition.Complications_Of_Placenta_Cord_Or_Membranes_Other_Specify_Disorder);
            Assert.Equal("FCOD_obstetricalcomplications", ValueSets.FetalDeathCauseOrCondition.Other_Obstetrical_Or_Pregnancy_Complications_Specify_Disorder);
            Assert.Equal("FCOD_fetalconditions", ValueSets.FetalDeathCauseOrCondition.Other_Fetal_Conditions_Disorder_Specify_Disorder);
            Assert.Equal("UNK", ValueSets.FetalDeathCauseOrCondition.Unknown);
        }

        [Fact]
        public void FetalDeathTimePoints_Should_Have_Correct_Codes()
        {
            Assert.Equal(4, ValueSets.FetalDeathTimePoints.Codes.GetLength(0));

            Assert.Equal("434681000124104", ValueSets.FetalDeathTimePoints.Antepartum_Fetal_Death_With_Cessation_Of_Labor);
            Assert.Equal("434671000124102", ValueSets.FetalDeathTimePoints.Antepartum_Fetal_Death_With_Continued_Labor);
            Assert.Equal("434631000124100", ValueSets.FetalDeathTimePoints.Fetal_Intrapartum_Death_After_First_Assessment);
            Assert.Equal("UNK", ValueSets.FetalDeathTimePoints.Unknown);
        }

        [Fact]
        public void FetalPresentations_Should_Have_Correct_Codes()
        {
            Assert.Equal(4, ValueSets.FetalPresentations.Codes.GetLength(0));

            Assert.Equal("70028003", ValueSets.FetalPresentations.Vertex_Presentation_Finding);
            Assert.Equal("6096002", ValueSets.FetalPresentations.Breech_Presentation_Finding);
            Assert.Equal("OTH", ValueSets.FetalPresentations.Other);
            Assert.Equal("UNK", ValueSets.FetalPresentations.Unknown);
        }

        [Fact]
        public void FetalRemainsDispositionMethod_Should_Have_Correct_Codes()
        {
            Assert.Equal(7, ValueSets.FetalRemainsDispositionMethod.Codes.GetLength(0));

            Assert.Equal("449971000124106", ValueSets.FetalRemainsDispositionMethod.Burial);
            Assert.Equal("449961000124104", ValueSets.FetalRemainsDispositionMethod.Cremation);
            Assert.Equal("449951000124101", ValueSets.FetalRemainsDispositionMethod.Donation);
            Assert.Equal("455401000124109", ValueSets.FetalRemainsDispositionMethod.Hospital_Disposition);
            Assert.Equal("449941000124103", ValueSets.FetalRemainsDispositionMethod.Removal_From_State);
            Assert.Equal("OTH", ValueSets.FetalRemainsDispositionMethod.Other);
            Assert.Equal("UNK", ValueSets.FetalRemainsDispositionMethod.Unknown);
        }

        [Fact]
        public void InfectionsDuringPregnancyLiveBirth_Should_Have_Correct_Codes()
        {
            Assert.Equal(7, ValueSets.InfectionsDuringPregnancyLiveBirth.Codes.GetLength(0));

            Assert.Equal("15628003", ValueSets.InfectionsDuringPregnancyLiveBirth.Gonorrhea_Disorder);
            Assert.Equal("76272004", ValueSets.InfectionsDuringPregnancyLiveBirth.Syphilis_Disorder);
            Assert.Equal("105629000", ValueSets.InfectionsDuringPregnancyLiveBirth.Chlamydia_Disorder);
            Assert.Equal("66071002", ValueSets.InfectionsDuringPregnancyLiveBirth.Hepatitis_B_Disorder);
            Assert.Equal("50711007", ValueSets.InfectionsDuringPregnancyLiveBirth.Hepatitis_C_Disorder);
            Assert.Equal("33839006", ValueSets.InfectionsDuringPregnancyLiveBirth.Genital_Herpes_Simplex_Disorder);
            Assert.Equal("OTH", ValueSets.InfectionsDuringPregnancyLiveBirth.Other);
        }

        [Fact]
        public void InformantRelationshipToMother_Should_Have_Correct_Codes()
        {
            Assert.Equal(4, ValueSets.InformantRelationshipToMother.Codes.GetLength(0));

            Assert.Equal("rel_fatherofbaby", ValueSets.InformantRelationshipToMother.Father_Of_Baby);
            Assert.Equal("rel_hospitalemployee", ValueSets.InformantRelationshipToMother.Hospital_Employee);
            Assert.Equal("rel_other", ValueSets.InformantRelationshipToMother.Other_With_Write_In_Text);
            Assert.Equal("rel_otherrelative", ValueSets.InformantRelationshipToMother.Other_Relative);
        }

        [Fact]
        public void LocationTypes_Should_Have_Correct_Codes()
        {
            Assert.Equal(3, ValueSets.LocationTypes.Codes.GetLength(0));

            Assert.Equal("loc_birth", ValueSets.LocationTypes.Birth_Location);
            Assert.Equal("loc_transfer-from", ValueSets.LocationTypes.Transfer_From_Location);
            Assert.Equal("loc_transfer-to", ValueSets.LocationTypes.Transfer_To_Location);
        }

        [Fact]
        public void NewbornCongenitalAnomalies_Should_Have_Correct_Codes()
        {
            Assert.Equal(13, ValueSets.NewbornCongenitalAnomalies.Codes.GetLength(0));

            Assert.Equal("89369001", ValueSets.NewbornCongenitalAnomalies.Anencephalus);
            Assert.Equal("67531005", ValueSets.NewbornCongenitalAnomalies.Meningomyelocele_Spina_Bifida);
            Assert.Equal("12770006", ValueSets.NewbornCongenitalAnomalies.Cyanotic_Congenital_Heart_Disease);
            Assert.Equal("17190001", ValueSets.NewbornCongenitalAnomalies.Congenital_Diaphragmatic_Hernia);
            Assert.Equal("18735004", ValueSets.NewbornCongenitalAnomalies.Congenital_Omphalocele);
            Assert.Equal("72951007", ValueSets.NewbornCongenitalAnomalies.Gastroschisis);
            Assert.Equal("67341007", ValueSets.NewbornCongenitalAnomalies.Longitudinal_Deficiency_Of_Limb_Limb_Reduction_Defect_Excluding_Congenital_Amputation_And_Dwarfing_Syndromes);
            Assert.Equal("80281008", ValueSets.NewbornCongenitalAnomalies.Cleft_Lip_With_Or_Without_Cleft_Palate);
            Assert.Equal("87979003", ValueSets.NewbornCongenitalAnomalies.Cleft_Palate);
            Assert.Equal("70156005", ValueSets.NewbornCongenitalAnomalies.Anomaly_Of_Chromosome_Pair_21);
            Assert.Equal("409709004", ValueSets.NewbornCongenitalAnomalies.Chromosomal_Disorder);
            Assert.Equal("416010008", ValueSets.NewbornCongenitalAnomalies.Hypospadias);
            Assert.Equal("OTH", ValueSets.NewbornCongenitalAnomalies.Other);
        }

        [Fact]
        public void NumberPreviousCesareansEditFlags_Should_Have_Correct_Codes()
        {
            Assert.Equal(2, ValueSets.NumberPreviousCesareansEditFlags.Codes.GetLength(0));

            Assert.Equal("0", ValueSets.NumberPreviousCesareansEditFlags.Edit_Passed);
            Assert.Equal("1failedVerified", ValueSets.NumberPreviousCesareansEditFlags.Edit_Failed_Verified);
        }

        [Fact]
        public void ObstetricProcedureOutcome_Should_Have_Correct_Codes()
        {
            Assert.Equal(2, ValueSets.ObstetricProcedureOutcome.Codes.GetLength(0));

            Assert.Equal("385669000", ValueSets.ObstetricProcedureOutcome.Successful_Qualifier_Value);
            Assert.Equal("385671000", ValueSets.ObstetricProcedureOutcome.Unsuccessful_Qualifier_Value);
        }

        [Fact]
        public void PerformedNotPerformedPlanned_Should_Have_Correct_Codes()
        {
            Assert.Equal(3, ValueSets.PerformedNotPerformedPlanned.Codes.GetLength(0));

            Assert.Equal("398166005", ValueSets.PerformedNotPerformedPlanned.Performed);
            Assert.Equal("262008008", ValueSets.PerformedNotPerformedPlanned.Not_Performed);
            Assert.Equal("397943006", ValueSets.PerformedNotPerformedPlanned.Planned);
        }

        [Fact]
        public void PregnancyReportEditFlags_Should_Have_Correct_Codes()
        {
            Assert.Equal(3, ValueSets.PregnancyReportEditFlags.Codes.GetLength(0));

            Assert.Equal("0", ValueSets.PregnancyReportEditFlags.Edit_Passed);
            Assert.Equal("1", ValueSets.PregnancyReportEditFlags.Edit_Failed_Data_Queried_And_Verified);
            Assert.Equal("2", ValueSets.PregnancyReportEditFlags.Edit_Failed_Data_Queried_But_Not_Verified);
        }

        [Fact]
        public void UnitsOfGestationalAge_Should_Have_Correct_Codes()
        {
            Assert.Equal(2, ValueSets.UnitsOfGestationalAge.Codes.GetLength(0));

            Assert.Equal("d", ValueSets.UnitsOfGestationalAge.Days);
            Assert.Equal("wk", ValueSets.UnitsOfGestationalAge.Weeks);
        }

        [Fact]
        public void CigaretteSmokingBeforeDuringPregnancy_Should_Have_Correct_Codes()
        {
            Assert.Equal(4, ValueSets.CigaretteSmokingBeforeDuringPregnancy.Codes.GetLength(0));

            Assert.Equal("64794-1", ValueSets.CigaretteSmokingBeforeDuringPregnancy.In_The_3_Months_Before_You_Got_Pregnant_How_Many_Cigarettes_Did_You_Smoke_On_An_Average_Day_Phenx);
            Assert.Equal("87298-6", ValueSets.CigaretteSmokingBeforeDuringPregnancy.Cigarettes_Smoked_Per_Day_By_Mother_1st_Trimester);
            Assert.Equal("87299-4", ValueSets.CigaretteSmokingBeforeDuringPregnancy.Cigarettes_Smoked_Per_Day_By_Mother_2nd_Trimester);
            Assert.Equal("64795-8", ValueSets.CigaretteSmokingBeforeDuringPregnancy.In_The_Last_3_Months_Of_Your_Pregnancy_How_Many_Cigarettes_Did_You_Smoke_On_An_Average_Day_Phenx);
        }

        [Fact]
        public void All_ValueSets_Should_Have_Valid_Code_Arrays()
        {
            // Test that all code arrays have the expected structure (3 columns: code, display, system)
            var valueSets = new[]
            {
                ValueSets.ApgarTiming.Codes,
                ValueSets.BirthAndFetalDeathFinancialClass.Codes,
                ValueSets.BirthAttendantTitles.Codes,
                ValueSets.BirthDeliveryOccurred.Codes,
                ValueSets.BirthWeightEditFlags.Codes,
                ValueSets.DeliveryRoutes.Codes,
                ValueSets.EstimateOfGestationEditFlags.Codes,
                ValueSets.FertilityEnhancingDrugTherapyArtificialInsem.Codes,
                ValueSets.FetalDeathCauseOrCondition.Codes,
                ValueSets.FetalDeathTimePoints.Codes,
                ValueSets.FetalPresentations.Codes,
                ValueSets.FetalRemainsDispositionMethod.Codes,
                ValueSets.InfectionsDuringPregnancyLiveBirth.Codes,
                ValueSets.InformantRelationshipToMother.Codes,
                ValueSets.LocationTypes.Codes,
                ValueSets.NewbornCongenitalAnomalies.Codes,
                ValueSets.NumberPreviousCesareansEditFlags.Codes,
                ValueSets.ObstetricProcedureOutcome.Codes,
                ValueSets.PerformedNotPerformedPlanned.Codes,
                ValueSets.PregnancyReportEditFlags.Codes,
                ValueSets.UnitsOfGestationalAge.Codes,
                ValueSets.CigaretteSmokingBeforeDuringPregnancy.Codes
            };

            foreach (var codeArray in valueSets)
            {
                // Each code array should have exactly 3 columns
                Assert.Equal(3, codeArray.GetLength(1));

                // Each row should have non-null values
                for (int i = 0; i < codeArray.GetLength(0); i++)
                {
                    Assert.NotNull(codeArray[i, 0]); // code
                    Assert.NotNull(codeArray[i, 1]); // display
                    Assert.NotNull(codeArray[i, 2]); // system

                    // Code and system should not be empty
                    Assert.NotEmpty(codeArray[i, 0]);
                    Assert.NotEmpty(codeArray[i, 2]);
                }
            }
        }

        [Fact]
        public void Code_Constants_Should_Match_Array_Values()
        {
            // Test a few key value sets to ensure constants match array values
            var apgarCodes = ValueSets.ApgarTiming.Codes;
            Assert.Contains(ValueSets.ApgarTiming._1_Minute_Apgar_Score,
                Enumerable.Range(0, apgarCodes.GetLength(0)).Select(i => apgarCodes[i, 0]));
            Assert.Contains(ValueSets.ApgarTiming._5_Minute_Apgar_Score,
                Enumerable.Range(0, apgarCodes.GetLength(0)).Select(i => apgarCodes[i, 0]));

            var financialCodes = ValueSets.BirthAndFetalDeathFinancialClass.Codes;
            Assert.Contains(ValueSets.BirthAndFetalDeathFinancialClass.Medicaid,
                Enumerable.Range(0, financialCodes.GetLength(0)).Select(i => financialCodes[i, 0]));
            Assert.Contains(ValueSets.BirthAndFetalDeathFinancialClass.Private_Health_Insurance,
                Enumerable.Range(0, financialCodes.GetLength(0)).Select(i => financialCodes[i, 0]));

            var attendantCodes = ValueSets.BirthAttendantTitles.Codes;
            Assert.Contains(ValueSets.BirthAttendantTitles.Medical_Doctor,
                Enumerable.Range(0, attendantCodes.GetLength(0)).Select(i => attendantCodes[i, 0]));
            Assert.Contains(ValueSets.BirthAttendantTitles.Osteopath,
                Enumerable.Range(0, attendantCodes.GetLength(0)).Select(i => attendantCodes[i, 0]));
        }

        [Fact]
        public void ValueSets_Should_Use_Correct_Code_Systems()
        {
            // Test that specific value sets use expected code systems
            var apgarCodes = ValueSets.ApgarTiming.Codes;
            for (int i = 0; i < apgarCodes.GetLength(0); i++)
            {
                Assert.True(apgarCodes[i, 2] == VR.CodeSystems.LOINC || apgarCodes[i, 2] == VR.CodeSystems.SCT);
            }

            var financialCodes = ValueSets.BirthAndFetalDeathFinancialClass.Codes;
            for (int i = 0; i < financialCodes.GetLength(0); i++)
            {
                Assert.Equal(VR.CodeSystems.NAHDO, financialCodes[i, 2]);
            }

            var attendantCodes = ValueSets.BirthAttendantTitles.Codes;
            for (int i = 0; i < attendantCodes.GetLength(0); i++)
            {
                Assert.True(attendantCodes[i, 2] == VR.CodeSystems.SCT || attendantCodes[i, 2] == VR.CodeSystems.NullFlavor_HL7_V3);
            }
        }

        [Fact]
        public void ValueSets_Should_Have_No_Duplicate_Codes()
        {
            // Test that each value set has unique codes
            var valueSets = new[]
            {
                ("ApgarTiming", ValueSets.ApgarTiming.Codes),
                ("BirthAndFetalDeathFinancialClass", ValueSets.BirthAndFetalDeathFinancialClass.Codes),
                ("BirthAttendantTitles", ValueSets.BirthAttendantTitles.Codes),
                ("BirthDeliveryOccurred", ValueSets.BirthDeliveryOccurred.Codes),
                ("DeliveryRoutes", ValueSets.DeliveryRoutes.Codes),
                ("FetalPresentations", ValueSets.FetalPresentations.Codes),
                ("NewbornCongenitalAnomalies", ValueSets.NewbornCongenitalAnomalies.Codes)
            };

            foreach (var (name, codeArray) in valueSets)
            {
                var codes = new List<string>();
                for (int i = 0; i < codeArray.GetLength(0); i++)
                {
                    codes.Add(codeArray[i, 0]);
                }

                var uniqueCodes = codes.Distinct().ToList();
                Assert.True(codes.Count == uniqueCodes.Count,
                    $"ValueSet {name} contains duplicate codes: {string.Join(", ", codes.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key))}");
            }
        }
    }
}
