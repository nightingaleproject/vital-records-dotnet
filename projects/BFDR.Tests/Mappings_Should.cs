using Xunit;
using BFDR;
using System.Linq;

namespace BFDR.Tests
{
    public class Mappings_Should
    {
        [Fact]
        public void BirthAndFetalDeathFinancialClass_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.BirthAndFetalDeathFinancialClass.IJEToFHIR;
            var fhirToIje = Mappings.BirthAndFetalDeathFinancialClass.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            foreach (var kvp in fhirToIje)
            {
                Assert.True(ijeToFhir.ContainsKey(kvp.Value), $"IJE to FHIR mapping missing for IJE code: {kvp.Value}");
                Assert.Equal(kvp.Key, ijeToFhir[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("2", ijeToFhir["1"]); // Medicaid
            Assert.Equal("5", ijeToFhir["2"]); // Private Health Insurance
            Assert.Equal("9999", ijeToFhir["9"]); // Unavailable/Unknown
        }

        [Fact]
        public void BirthAttendantTitles_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.BirthAttendantTitles.IJEToFHIR;
            var fhirToIje = Mappings.BirthAttendantTitles.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("112247003", ijeToFhir["1"]); // Medical Doctor
            Assert.Equal("76231001", ijeToFhir["2"]); // Osteopath
            Assert.Equal("UNK", ijeToFhir["9"]); // Unknown
        }

        [Fact]
        public void BirthDeliveryOccurred_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.BirthDeliveryOccurred.IJEToFHIR;
            var fhirToIje = Mappings.BirthDeliveryOccurred.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("22232009", ijeToFhir["1"]); // Hospital
            Assert.Equal("91154008", ijeToFhir["2"]); // Free Standing Birthing Center
            Assert.Equal("unknownplannedhomebirth", ijeToFhir["5"]); // Unknown If Planned Home Birth
        }

        [Fact]
        public void BirthWeightEditFlags_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.BirthWeightEditFlags.IJEToFHIR;
            var fhirToIje = Mappings.BirthWeightEditFlags.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("0off", ijeToFhir["0"]); // Off
            Assert.Equal("1correctOutOfRange", ijeToFhir["1"]); // Queried Data Correct Out Of Range
            Assert.Equal("2failedBirthWeightGestationEdit", ijeToFhir["2"]); // Queried Failed Birthweight Gestation Edit
        }

        [Fact]
        public void DeliveryRoutes_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.DeliveryRoutes.IJEToFHIR;
            var fhirToIje = Mappings.DeliveryRoutes.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("700000006", ijeToFhir["1"]); // Vaginal Delivery
            Assert.Equal("302383004", ijeToFhir["2"]); // Forceps Delivery
            Assert.Equal("61586001", ijeToFhir["3"]); // Vacuum Extraction
            Assert.Equal("11466000", ijeToFhir["4"]); // Cesarean Section
        }

        [Fact]
        public void EstimateOfGestationEditFlags_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.EstimateOfGestationEditFlags.IJEToFHIR;
            var fhirToIje = Mappings.EstimateOfGestationEditFlags.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("0off", ijeToFhir["0"]); // Off
            Assert.Equal("1correctOutOfRange", ijeToFhir["1"]); // Queried Data Correct Out Of Range
        }

        [Fact]
        public void FetalDeathCauseOrCondition_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.FetalDeathCauseOrCondition.IJEToFHIR;
            var fhirToIje = Mappings.FetalDeathCauseOrCondition.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("44223004", ijeToFhir["COD18a1"]); // Premature Rupture Of Membranes
            Assert.Equal("415105001", ijeToFhir["COD18a2"]); // Placental Abruption
            Assert.Equal("UNK", ijeToFhir["COD18a7"]); // Unknown
        }

        [Fact]
        public void FetalDeathTimePoints_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.FetalDeathTimePoints.IJEToFHIR;
            var fhirToIje = Mappings.FetalDeathTimePoints.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("434681000124104", ijeToFhir["N"]); // Antepartum Fetal Death With Cessation Of Labor
            Assert.Equal("434671000124102", ijeToFhir["L"]); // Antepartum Fetal Death With Continued Labor
            Assert.Equal("434631000124100", ijeToFhir["A"]); // Fetal Intrapartum Death After First Assessment
            Assert.Equal("UNK", ijeToFhir["U"]); // Unknown
        }

        [Fact]
        public void FetalPresentation_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.FetalPresentation.IJEToFHIR;
            var fhirToIje = Mappings.FetalPresentation.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("70028003", ijeToFhir["1"]); // Vertex Presentation
            Assert.Equal("6096002", ijeToFhir["2"]); // Breech Presentation
            Assert.Equal("OTH", ijeToFhir["3"]); // Other
            Assert.Equal("UNK", ijeToFhir["9"]); // Unknown
        }

        [Fact]
        public void InfectionsDuringPregnancyLiveBirth_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.InfectionsDuringPregnancyLiveBirth.IJEToFHIR;
            var fhirToIje = Mappings.InfectionsDuringPregnancyLiveBirth.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("15628003", ijeToFhir["GON"]); // Gonorrhea
            Assert.Equal("76272004", ijeToFhir["SYPH"]); // Syphilis
            Assert.Equal("105629000", ijeToFhir["CHAM"]); // Chlamydia
            Assert.Equal("66071002", ijeToFhir["HEPB"]); // Hepatitis B
            Assert.Equal("50711007", ijeToFhir["HEPC"]); // Hepatitis C
        }

        [Fact]
        public void NewbornCongenitalAnomalies_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.NewbornCongenitalAnomalies.IJEToFHIR;
            var fhirToIje = Mappings.NewbornCongenitalAnomalies.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("89369001", ijeToFhir["ANEN"]); // Anencephalus
            Assert.Equal("67531005", ijeToFhir["MNSB"]); // Meningomyelocele/Spina Bifida
            Assert.Equal("12770006", ijeToFhir["CCHD"]); // Cyanotic Congenital Heart Disease
            Assert.Equal("416010008", ijeToFhir["HYPO"]); // Hypospadias
        }

        [Fact]
        public void NumberPreviousCesareansEditFlags_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.NumberPreviousCesareansEditFlags.IJEToFHIR;
            var fhirToIje = Mappings.NumberPreviousCesareansEditFlags.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("0", ijeToFhir["0"]); // Edit Passed
            Assert.Equal("1failedVerified", ijeToFhir["1"]); // Edit Failed Verified
        }

        [Fact]
        public void PerformedNotPerformedPlanned_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.PerformedNotPerformedPlanned.IJEToFHIR;
            var fhirToIje = Mappings.PerformedNotPerformedPlanned.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("398166005", ijeToFhir["Y"]); // Performed
            Assert.Equal("262008008", ijeToFhir["N"]); // Not Performed
            Assert.Equal("397943006", ijeToFhir["P"]); // Planned
        }

        [Fact]
        public void PregnancyReportEditFlags_Should_Have_Bidirectional_Mappings()
        {
            var ijeToFhir = Mappings.PregnancyReportEditFlags.IJEToFHIR;
            var fhirToIje = Mappings.PregnancyReportEditFlags.FHIRToIJE;

            // Test that mappings are bidirectional
            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR code: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }

            // Test specific mappings
            Assert.Equal("0", ijeToFhir["0"]); // Edit Passed
            Assert.Equal("1", ijeToFhir["1"]); // Edit Failed Data Queried And Verified
            Assert.Equal("2", ijeToFhir["2"]); // Edit Failed Data Queried But Not Verified
        }

        [Fact]
        public void All_Mappings_Should_Have_Non_Empty_Dictionaries()
        {
            // Test that all mapping dictionaries are not null and not empty
            Assert.NotNull(Mappings.BirthAndFetalDeathFinancialClass.IJEToFHIR);
            Assert.NotNull(Mappings.BirthAndFetalDeathFinancialClass.FHIRToIJE);
            Assert.NotEmpty(Mappings.BirthAndFetalDeathFinancialClass.IJEToFHIR);
            Assert.NotEmpty(Mappings.BirthAndFetalDeathFinancialClass.FHIRToIJE);

            Assert.NotNull(Mappings.BirthAttendantTitles.IJEToFHIR);
            Assert.NotNull(Mappings.BirthAttendantTitles.FHIRToIJE);
            Assert.NotEmpty(Mappings.BirthAttendantTitles.IJEToFHIR);
            Assert.NotEmpty(Mappings.BirthAttendantTitles.FHIRToIJE);

            Assert.NotNull(Mappings.BirthDeliveryOccurred.IJEToFHIR);
            Assert.NotNull(Mappings.BirthDeliveryOccurred.FHIRToIJE);
            Assert.NotEmpty(Mappings.BirthDeliveryOccurred.IJEToFHIR);
            Assert.NotEmpty(Mappings.BirthDeliveryOccurred.FHIRToIJE);

            Assert.NotNull(Mappings.DeliveryRoutes.IJEToFHIR);
            Assert.NotNull(Mappings.DeliveryRoutes.FHIRToIJE);
            Assert.NotEmpty(Mappings.DeliveryRoutes.IJEToFHIR);
            Assert.NotEmpty(Mappings.DeliveryRoutes.FHIRToIJE);

            Assert.NotNull(Mappings.NewbornCongenitalAnomalies.IJEToFHIR);
            Assert.NotNull(Mappings.NewbornCongenitalAnomalies.FHIRToIJE);
            Assert.NotEmpty(Mappings.NewbornCongenitalAnomalies.IJEToFHIR);
            Assert.NotEmpty(Mappings.NewbornCongenitalAnomalies.FHIRToIJE);
        }

        [Fact]
        public void All_Mappings_Should_Have_Equal_Count_In_Both_Directions()
        {
            // Test that IJE->FHIR and FHIR->IJE mappings have the same count
            Assert.Equal(Mappings.BirthAndFetalDeathFinancialClass.IJEToFHIR.Count,
                        Mappings.BirthAndFetalDeathFinancialClass.FHIRToIJE.Count);

            Assert.Equal(Mappings.BirthAttendantTitles.IJEToFHIR.Count,
                        Mappings.BirthAttendantTitles.FHIRToIJE.Count);

            Assert.Equal(Mappings.BirthDeliveryOccurred.IJEToFHIR.Count,
                        Mappings.BirthDeliveryOccurred.FHIRToIJE.Count);

            Assert.Equal(Mappings.BirthWeightEditFlags.IJEToFHIR.Count,
                        Mappings.BirthWeightEditFlags.FHIRToIJE.Count);

            Assert.Equal(Mappings.DeliveryRoutes.IJEToFHIR.Count,
                        Mappings.DeliveryRoutes.FHIRToIJE.Count);

            Assert.Equal(Mappings.EstimateOfGestationEditFlags.IJEToFHIR.Count,
                        Mappings.EstimateOfGestationEditFlags.FHIRToIJE.Count);

            Assert.Equal(Mappings.FetalDeathCauseOrCondition.IJEToFHIR.Count,
                        Mappings.FetalDeathCauseOrCondition.FHIRToIJE.Count);

            Assert.Equal(Mappings.FetalDeathTimePoints.IJEToFHIR.Count,
                        Mappings.FetalDeathTimePoints.FHIRToIJE.Count);

            Assert.Equal(Mappings.FetalPresentation.IJEToFHIR.Count,
                        Mappings.FetalPresentation.FHIRToIJE.Count);

            Assert.Equal(Mappings.InfectionsDuringPregnancyLiveBirth.IJEToFHIR.Count,
                        Mappings.InfectionsDuringPregnancyLiveBirth.FHIRToIJE.Count);

            Assert.Equal(Mappings.NewbornCongenitalAnomalies.IJEToFHIR.Count,
                        Mappings.NewbornCongenitalAnomalies.FHIRToIJE.Count);

            Assert.Equal(Mappings.NumberPreviousCesareansEditFlags.IJEToFHIR.Count,
                        Mappings.NumberPreviousCesareansEditFlags.FHIRToIJE.Count);

            Assert.Equal(Mappings.PerformedNotPerformedPlanned.IJEToFHIR.Count,
                        Mappings.PerformedNotPerformedPlanned.FHIRToIJE.Count);

            Assert.Equal(Mappings.PregnancyReportEditFlags.IJEToFHIR.Count,
                        Mappings.PregnancyReportEditFlags.FHIRToIJE.Count);
        }

        [Fact]
        public void Mappings_Should_Not_Have_Null_Or_Empty_Keys_Or_Values()
        {
            // Test BirthAndFetalDeathFinancialClass
            foreach (var kvp in Mappings.BirthAndFetalDeathFinancialClass.IJEToFHIR)
            {
                Assert.NotNull(kvp.Key);
                Assert.NotNull(kvp.Value);
                Assert.NotEmpty(kvp.Key);
                Assert.NotEmpty(kvp.Value);
            }

            foreach (var kvp in Mappings.BirthAndFetalDeathFinancialClass.FHIRToIJE)
            {
                Assert.NotNull(kvp.Key);
                Assert.NotNull(kvp.Value);
                Assert.NotEmpty(kvp.Key);
                Assert.NotEmpty(kvp.Value);
            }

            // Test DeliveryRoutes
            foreach (var kvp in Mappings.DeliveryRoutes.IJEToFHIR)
            {
                Assert.NotNull(kvp.Key);
                Assert.NotNull(kvp.Value);
                Assert.NotEmpty(kvp.Key);
                Assert.NotEmpty(kvp.Value);
            }

            foreach (var kvp in Mappings.DeliveryRoutes.FHIRToIJE)
            {
                Assert.NotNull(kvp.Key);
                Assert.NotNull(kvp.Value);
                Assert.NotEmpty(kvp.Key);
                Assert.NotEmpty(kvp.Value);
            }

            // Test NewbornCongenitalAnomalies
            foreach (var kvp in Mappings.NewbornCongenitalAnomalies.IJEToFHIR)
            {
                Assert.NotNull(kvp.Key);
                Assert.NotNull(kvp.Value);
                Assert.NotEmpty(kvp.Key);
                Assert.NotEmpty(kvp.Value);
            }

            foreach (var kvp in Mappings.NewbornCongenitalAnomalies.FHIRToIJE)
            {
                Assert.NotNull(kvp.Key);
                Assert.NotNull(kvp.Value);
                Assert.NotEmpty(kvp.Key);
                Assert.NotEmpty(kvp.Value);
            }
        }

        [Fact]
        public void Mappings_Should_Have_No_Duplicate_Values_Within_Same_Direction()
        {
            // Test that IJE->FHIR mappings don't have duplicate FHIR values
            var birthFinancialIjeToFhirValues = Mappings.BirthAndFetalDeathFinancialClass.IJEToFHIR.Values.ToList();
            Assert.Equal(birthFinancialIjeToFhirValues.Count, birthFinancialIjeToFhirValues.Distinct().Count());

            var deliveryRoutesIjeToFhirValues = Mappings.DeliveryRoutes.IJEToFHIR.Values.ToList();
            Assert.Equal(deliveryRoutesIjeToFhirValues.Count, deliveryRoutesIjeToFhirValues.Distinct().Count());

            var anomaliesIjeToFhirValues = Mappings.NewbornCongenitalAnomalies.IJEToFHIR.Values.ToList();
            Assert.Equal(anomaliesIjeToFhirValues.Count, anomaliesIjeToFhirValues.Distinct().Count());

            // Test that FHIR->IJE mappings don't have duplicate IJE values
            var birthFinancialFhirToIjeValues = Mappings.BirthAndFetalDeathFinancialClass.FHIRToIJE.Values.ToList();
            Assert.Equal(birthFinancialFhirToIjeValues.Count, birthFinancialFhirToIjeValues.Distinct().Count());

            var deliveryRoutesFhirToIjeValues = Mappings.DeliveryRoutes.FHIRToIJE.Values.ToList();
            Assert.Equal(deliveryRoutesFhirToIjeValues.Count, deliveryRoutesFhirToIjeValues.Distinct().Count());

            var anomaliesFhirToIjeValues = Mappings.NewbornCongenitalAnomalies.FHIRToIJE.Values.ToList();
            Assert.Equal(anomaliesFhirToIjeValues.Count, anomaliesFhirToIjeValues.Distinct().Count());
        }
    }
}
