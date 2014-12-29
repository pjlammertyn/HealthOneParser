using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthOneParser.Dto;

namespace HealthOneParser
{
    public class Parser
    {
        #region Fields

        int lineNumber;

        #endregion

        #region Constructor

        public Parser()
        {
            ParserErrors = new Dictionary<int, IList<string>>();
        }

        #endregion

        #region Properties

        public IDictionary<int, IList<string>> ParserErrors { get; set; }

        #endregion

        #region Parser Methods

        public IEnumerable<Report> ParseReport(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            using (var reader = new StringReader(text))
            {
                return ParseReport(reader);
            }
        }

        public IEnumerable<Labo> ParseLabo(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            using (var reader = new StringReader(text))
            {
                return ParseLabo(reader);
            }
        }

        public IEnumerable<Labo> ParseLabo(TextReader reader)
        {
            var labos = new List<Labo>();
            lineNumber = -1;
            ParserErrors.Clear();

            var line = ReadLine(reader);
            if (line == null)
                return Enumerable.Empty<Labo>();
            Labo labo = null;
            do
            {
                var recordParts = line.Split('\\').ToList();

                if (recordParts.Count < 3)
                {
                    ParserErrors.AddItem(lineNumber, string.Format("Line doesn't consists of more than 2 parst sepeated by '\': {0}", line));
                    break;
                }

                if (recordParts.ElementAtOrDefault(0) == "A1")
                {
                    labo = new Labo();
                    labos.Add(labo);
                }

                if (labo == null)
                {
                    ParserErrors.AddItem(lineNumber, string.Format("Administrative block did not start with 'A1\': {0}", line));
                    break;
                }
                else
                    ParseLaboRecord(labo, recordParts);
            }
            while ((line = ReadLine(reader)) != null);

            return labos;
        }

        public IEnumerable<Report> ParseReport(TextReader reader)
        {
            var reports = new List<Report>();
            lineNumber = -1;
            ParserErrors.Clear();

            var line = ReadLine(reader);
            if (line == null)
                return Enumerable.Empty<Report>();
            Report report = null;
            do
            {
                var recordParts = line.Split('\\').ToList();

                if (recordParts.Count < 3)
                {
                    ParserErrors.AddItem(lineNumber, string.Format("Line doesn't consists of more than 2 parst sepeated by '\': {0}", line));
                    break;
                }

                if (recordParts.ElementAtOrDefault(0) == "A1")
                {
                    report = new Report();
                    reports.Add(report);
                }

                if (report == null)
                {
                    ParserErrors.AddItem(lineNumber, string.Format("Administrative block did not start with 'A1\': {0}", line));
                    break;
                }
                else
                    ParseReportRecord(report, recordParts);
            }
            while ((line = ReadLine(reader)) != null);

            return reports;
        }

        #endregion

        #region Private Parser Methods

        void ParseReportRecord(Report report, IList<string> recordParts)
        {
            switch (recordParts.ElementAtOrDefault(0))
            {
                case "A1":
                    ParseAdministrative1(recordParts, report);
                    break;
                case "A2":
                    ParseAdministrative2(recordParts, report);
                    break;
                case "A3":
                    ParseAdministrative3(recordParts, report);
                    break;
                case "A4":
                    ParseAdministrative4(recordParts, report);
                    break;
                case "A5":
                    ParseAdministrative5(recordParts, report);
                    break;
                case "L2":
                case "L3":
                case "L5":
                    ParseL5(recordParts, report);
                    break;
                default:
                    ParserErrors.AddItem(lineNumber, string.Format("Unknown record descriptor '{0}'", recordParts.ElementAtOrDefault(0)));
                    break;
            }
        }

        void ParseLaboRecord(Labo labo, IList<string> recordParts)
        {
            switch (recordParts.ElementAtOrDefault(0))
            {
                case "A1":
                    ParseAdministrative1(recordParts, labo);
                    break;
                case "A2":
                    ParseAdministrative2(recordParts, labo);
                    break;
                case "A3":
                    ParseAdministrative3(recordParts, labo);
                    break;
                case "A4":
                    ParseAdministrative4(recordParts, labo);
                    break;
                case "A5":
                    ParseAdministrative5(recordParts, labo);
                    break;
                case "L1":
                    labo.Results.Add(ParseL1(recordParts));
                    break;
                default:
                    ParserErrors.AddItem(lineNumber, string.Format("Unknown record descriptor '{0}'", recordParts.ElementAtOrDefault(0)));
                    break;
            }
        }

        LaboResult ParseL1(IList<string> recordParts)
        {
            if (recordParts.Count != 9 || !recordParts[8].IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Expected 8 parts in L1 but got {0} parts: '{1}'", recordParts.Count - 1, string.Join("\\", recordParts)));

            var result = new LaboResult();

            result.TestCode = recordParts.ElementAtOrDefault(2);
            result.TestName = recordParts.ElementAtOrDefault(3);
            result.ReferenceValues = recordParts.ElementAtOrDefault(4);
            result.Unit = recordParts.ElementAtOrDefault(5);
            result.Code = recordParts.ElementAtOrDefault(6);
            result.Result = recordParts.ElementAtOrDefault(7);

            return result;
        }

        void ParseL5(IList<string> recordParts, Report report)
        {
            if (recordParts.Count != 9 || !recordParts[8].IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Expected 8 parts in L5 (or L2 or L3) but got {0} parts: '{1}'", recordParts.Count - 1, string.Join("\\", recordParts)));

            report.Speciality = recordParts.ElementAtOrDefault(2);
            report.Text = string.Concat(report.Text, recordParts.ElementAtOrDefault(7), Environment.NewLine);
        }

        void ParseAdministrative1(IList<string> recordParts, IAdministrative adm)
        {
            if (recordParts.Count != 4 || !recordParts[3].IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Expected 3 parts in A1 but got {0} parts: '{1}'", recordParts.Count - 1, string.Join("\\", recordParts)));

            if (recordParts.ElementAtOrDefault(1).IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Protocol number is a required field on position 2: '{0}'", string.Join("\\", recordParts)));
            adm.ProtocolNumber = recordParts.ElementAtOrDefault(1);
            adm.Identification = recordParts.ElementAtOrDefault(2);
        }

        void ParseAdministrative2(IList<string> recordParts, IAdministrative adm)
        {
            if (recordParts.Count != 7 || !recordParts[6].IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Expected 6 parts in A2 but got {0} parts: '{1}'", recordParts.Count - 1, string.Join("\\", recordParts)));

            if (adm.Patient == null)
                adm.Patient = new Patient();
            if (recordParts.ElementAtOrDefault(2).IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Patient lastname is a required field on position 3: '{0}'", string.Join("\\", recordParts)));
            adm.Patient.LastName = recordParts.ElementAtOrDefault(2);
            if (recordParts.ElementAtOrDefault(3).IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Patient firstname is a required field on position 4: '{0}'", string.Join("\\", recordParts)));
            adm.Patient.FirstName = recordParts.ElementAtOrDefault(3);
            if (recordParts.ElementAtOrDefault(4).IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Patient sex is a required field on position 5: '{0}'", string.Join("\\", recordParts)));
            adm.Patient.Sex = recordParts.ElementAtOrDefault(4).Maybe(s =>
            {
                switch (s)
                {
                    case "F":
                    case "V":
                        return Sex.Female;
                    case "M":
                        return Sex.Male;
                    default:
                        return Sex.Unknown;
                }
            });
            if (recordParts.ElementAtOrDefault(5).IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Patient birthdate is  a required field on position 6: '{0}'", string.Join("\\", recordParts)));
            adm.Patient.BirthDate = recordParts.ElementAtOrDefault(5).Maybe(s => s.ToNullableDatetime("ddMMyyyy"));
        }

        void ParseAdministrative3(IList<string> recordParts, IAdministrative adm)
        {
            if (recordParts.Count != 6 || !recordParts[5].IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Expected 5 parts in A3 but got {0} parts: '{1}'", recordParts.Count - 1, string.Join("\\", recordParts)));

            if (adm.Patient == null)
                adm.Patient = new Patient();
            adm.Patient.Street = recordParts.ElementAtOrDefault(2);
            adm.Patient.PostalCode = recordParts.ElementAtOrDefault(3);
            adm.Patient.PostalName = recordParts.ElementAtOrDefault(4);
        }

        void ParseAdministrative4(IList<string> recordParts, IAdministrative adm)
        {
            if (recordParts.Count != 7 || !recordParts[6].IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Expected 6 parts in A4 but got {0} parts: '{1}'", recordParts.Count - 1, string.Join("\\", recordParts)));

            adm.RequestorId = recordParts.ElementAtOrDefault(2);
            if (recordParts.ElementAtOrDefault(3).IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Request date is a required field on position 4: '{0}'", string.Join("\\", recordParts)));
            adm.RequestDate = (recordParts.ElementAtOrDefault(3)).Maybe(v =>
            {
                var date = v.ToNullableDatetime("ddMMyyyy");
                DateTime? time = (recordParts.ElementAtOrDefault(4)).Maybe(vt => vt.ToNullableDatetime("HHmm"));
                return date.HasValue ? (time.HasValue ? date.Value.Add(time.Value.TimeOfDay) : date) : null;
            });
            if (recordParts.ElementAtOrDefault(5).IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Protocol status is a required field on position 6: '{0}'", string.Join("\\", recordParts)));
            adm.Status = recordParts.ElementAtOrDefault(5).Maybe(s =>
            {
                switch (s)
                {
                    case "P":
                        return ProtocolStatus.Partial;
                    case "C":
                        return ProtocolStatus.Complete;
                    default:
                        return ProtocolStatus.Unknown;
                }
            });
        }

        void ParseAdministrative5(IList<string> recordParts, IAdministrative adm)
        {
            if (recordParts.Count != 8 || !recordParts[7].IsNullOrEmpty())
                ParserErrors.AddItem(lineNumber, string.Format("Expected 8 parts in A5 but got {0} parts: '{1}'", recordParts.Count - 1, string.Join("\\", recordParts)));

            if (adm.Mutuality == null)
                adm.Mutuality = new Mutuality();
            adm.Mutuality.Id = recordParts.ElementAtOrDefault(2);
            adm.Mutuality.Rrn = recordParts.ElementAtOrDefault(3);
            adm.Mutuality.Coverage = recordParts.ElementAtOrDefault(4);
            adm.Mutuality.Holder = recordParts.ElementAtOrDefault(5);
            adm.Mutuality.Kg1 = recordParts.ElementAtOrDefault(6);
            adm.Mutuality.Kg2 = recordParts.ElementAtOrDefault(7);
        }

        #endregion

        #region Methods

        string ReadLine(TextReader reader)
        {
            lineNumber++;
            return reader.ReadLine();
        }

        #endregion
    }
}
