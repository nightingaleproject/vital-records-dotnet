using System;
using VR;

namespace BFDR
{
    /// <summary>Class <c>Connectathon</c> provides static methods for generating records used in Connectathon testing, used in Canary and in the CLI tool</summary>
    public class Connectathon
    {
        /// <summary>Retrieve all available pre-set records</summary>
        public static BirthRecord[] BirthRecords
        {
            get
            {
                return new BirthRecord[] {
                    YytrfCardenasRomero(),
                    XyugbnxZalbanaiz(),
                    NullMonroe()
                };
            }
        }

        /// <summary>Retrieve all available pre-set records</summary>
        public static FetalDeathRecord[] FetalDeathRecords
        {
            get
            {
                return new FetalDeathRecord[] {
                    Test1()
                };
            }
        }

        /// <summary>Generate a BirthRecord from one of 2 pre-set records, providing an optional certificate number and state</summary>
        public static BirthRecord FromId(int id, int? certificateNumber = null, string state = null, int? year = null)
        {
            BirthRecord record = null;
            switch (id)
            {
                case 1:
                    record = YytrfCardenasRomero();
                    break;
                case 2:
                    record = XyugbnxZalbanaiz();
                    break;
                case 3:
                    record = NullMonroe();
                    break;
            }

            if (record != null && certificateNumber != null)
            {
                record.CertificateNumber = certificateNumber.ToString();
            }

            if (record != null && state != null)
            {
                record.EventLocationJurisdiction = state;
            }

            if (record != null && year != null)
            {
                // Overwrite the record's year in it's date of birth attribute.
                record.DateOfBirth = year.ToString() + record.DateOfBirth.Substring(4);
            }

            return record;
        }

        /// <summary>Generate the Yytrf Cardenas Romero example record</summary>
        public static BirthRecord YytrfCardenasRomero()
        {
            string rawIJE = "2024TT0999910            1031F010101911487607784  1101199210130XXMX77000019AZUSY199112190UUU30HNNN                    YNNNNNNNNNNNNNN                                                                                                                                                                                                                                                                                                30HNNN                    YNNNNNNNNNNNNNN                                                                                                                                                                                                                                                                                                1N8888888888888888000501010001270N03000006201588888800000000320190731NNNNNN NN000NN NNNNNNNNYNNNNYNNNNYNN21XNNNNNN0539021001040199999999990YYYNNNNNNNNNNNNNNNNYYN        9999NXX                 20200102                                                 YYTRF                                             XMIDDLEXX                                         CARDENAS ROMERO                                          PIMA                     TUCSON                                            NORTHWEST MEDICAL CENTER                          ALEJANDRA                                                                                           ROMERO LEON                                                                                                                                                  ROMERO LEON                                                                                                                         6666 NORTH ORACLE ROAD100                         85705    PIMA                        TUCSON                      ARIZONA                     UNITED STATES               RAMON                                             FELIPE                                            CARDENAS OTERO                                           8888888888888888882930                                                                                                                ZZMX                                                                                                                                                                                                    BANNER UNIVERSITY MEDICAL CENTER - TUCSON                                                         MEXICO                                                  MEXICO                                                                                                 9999 NORTH PRIEST RD236                           85489                                MESA                        ARIZONA                     UNITED STATES               Y1             HEATHERSTEVENS                                    1932304839                                                                                     1393674        1393655                20200102                                                  1XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        ";
            IJEBirth ije = new IJEBirth(rawIJE);
            BirthRecord record = ije.ToRecord();
            return record;
        }


        /// <summary>Generate the Xyugbnx Zalbanaiz example record</summary>
        public static BirthRecord XyugbnxZalbanaiz()
        {
            string rawIJE = "2024TT0099990            1025M010101311568794535  1095199505150XXKU73000013AZUSY199506040YYX40NNNN                    NNNNNNNNNNNNNNY                                                                                                                                                                                    MIDDLE EASTERN                ARABIAN                                                                       60NNNN                    NNNNNNNNNNNNNNY                                                                                                                                                                                    MIDDLE EASTERN                ARABIAN                                                                       1N0604201812272018180503016501990N01000001201988888800000000220180418NNNNNN NN000NN NNNNNNNNNNNNNNYNNYYNN14NNNNNNN2277036009880202999999990NNNNNNNNNNNNNNNNNNNNYY        9999NXX                 20190102                                                 XYUGBNX                                           XMX                                               ZALBANAIZ                                                MARICOPA                 MESA                                              MOUNTAIN VISTA MEDICAL CENTER                     REEM                                              NASSER                                            ALHAMADI                                                                                                                                                     ALHAMADI                                                                                                                            999 N COLLEGE AVE5656                             85281    MARICOPA                    TEMPE                       ARIZONA                     UNITED STATES               OMAR                                              AHMED                                             ALBANAI                                                  8888888888888888882626                                                                                                                ZZKU                                                                                                                                                                                                                                                                                                      KUWAIT                                                  KUWAIT                                                                                                 888 N PRIEST AVE9999                              85429                                GLENDALE                    ARIZONA                     UNITED STATES               Y1             MANISHAAPUROHIT                                   1972721538                                                                                     1201183921     1200527124             20190102                                                  0XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        ";
            IJEBirth ije = new IJEBirth(rawIJE);
            BirthRecord record = ije.ToRecord();
            return record;
        }

        /// <summary>Generate the [Null] Monroe example record which is contains mostly null data</summary>
        public static BirthRecord NullMonroe()
        {
            string rawIJE = "2023  000000 0000000000000000N0302000                              00000000                   UUUU                                                                                                                                                                                                                                                                                                                                     UUUU                                                                                                                                                                                                                                                                                                                                     88888888           999                888888888888                 UUUUUUUUU   UUUUUU  UU   UUUUUUUUU    UUUUUUU          88             UUUUUUUUUUUUUUUUUUU UN            UUU                                                                                                                                                                              Monroe                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       U                                                                                                                                                               55555555555555566666666666666                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          ";
            IJEBirth ije = new IJEBirth(rawIJE);
            BirthRecord record = ije.ToRecord();
            return record;
        }

        // Writes record to a file named filename in a subdirectory of the current working directory
        // Note that you do this with docker, you will have to set a bind mount on the container
        /// <summary>Of historical interest for writing a record to a file</summary>
        public static string WriteRecordAsXml(VitalRecord record, string filename)
        {
            string parentPath = System.IO.Directory.GetCurrentDirectory() + "/connectathon_files";
            System.IO.Directory.CreateDirectory(parentPath);  // in case the directory does not exist
            string fullPath = parentPath + "/" + filename;
            Console.WriteLine("writing record to " + fullPath + " as XML");
            string xml = record.ToXml();
            System.IO.File.WriteAllText(@fullPath, xml);
            // Console.WriteLine(xml);
            return xml;
        }

        /// <summary>Generate a FetalDeathRecord from one of 2 pre-set records, providing an optional certificate number and state</summary>
        public static FetalDeathRecord FetalDeathRecordFromId(int id, int? certificateNumber = null, string state = null, int? year = null)
        {
            FetalDeathRecord record = null;
            switch (id)
            {
                case 1:
                    record = Test1();
                    break;
            }

            if (record != null && certificateNumber != null)
            {
                record.CertificateNumber = certificateNumber.ToString();
            }

            if (record != null && state != null)
            {
                record.EventLocationJurisdiction = state;
            }

            if (record != null && year != null)
            {
                // Overwrite the record's year in it's date of birth attribute.
                record.DateOfDelivery = year.ToString() + record.DateOfDelivery.Substring(4);
            }

            return record;
        }

        /// <summary>Generate Monroe example record</summary>
        public static FetalDeathRecord Test1()
        {
            string rawIJE = "2024MA0000080            1830M12132091            2040199801140PAFM36000209DEUSY200010200   90UUUU                    NNNNNNNNNNNNNYN                                                                                                                        CHUUK                                                                                                                                                                   2 99999999           50301820    U0200  992022      0000000099999999NNNN  NNN000  N       NN31X   N N 09990320NNYN0199999999990                    2623YXX20241213   NNNNNNNSevere Preeclamsia/Eclampsia                                                                                                                                                                                                                                                                                                                                                                                                        NYNNNNN                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                NONE IDENTIFIED                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 INTRAUTERINE FETAL DEMISE                                                                                                                                                                                                                                                                  Zeke                                                                                                Anwar                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        XXXX      S         14 TH                                             ST                         XXXX S 14 TH ST                                   XXXXX    XXXXXXXXX                   xxxxxx xxxx                 Delaware                    UNITED STATES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 0                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        ";
            IJEFetalDeath ije = new IJEFetalDeath(rawIJE);
            FetalDeathRecord record = ije.ToRecord();
            return record;
        }

    }
}