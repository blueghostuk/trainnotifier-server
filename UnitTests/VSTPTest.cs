﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Common.Model.VSTP;
using System.Diagnostics;
using Newtonsoft.Json;
using TrainNotifier.Service;
using System.Linq;
using TrainNotifier.Common.Model.Exceptions;

namespace UnitTests
{
    [TestClass]
    public class VstpTests
    {
        [TestMethod]
        public void TestVSTP()
        {
            const string msg = "{\"VSTPCIFMsgV1\":{\"schemaLocation\":\"http://xml.networkrail.co.uk/ns/2008/Train itm_vstp_cif_messaging_v1.xsd\",\"classification\":\"industry\",\"timestamp\":\"1376123424000\",\"owner\":\"Network Rail\",\"originMsgId\":\"2013-08-10T08:30:24-00:00vstp.networkrail.co.uk\",\"Sender\":{\"organisation\":\"Network Rail\",\"application\":\"TOPS\",\"component\":\"VSTP\"},\"schedule\":{\"schedule_id\":\"\",\"transaction_type\":\"Create\",\"schedule_start_date\":\"2013-08-10\",\"schedule_end_date\":\"2013-08-10\",\"schedule_days_runs\":\"0000010\",\"applicable_timetable\":\"Y\",\"CIF_bank_holiday_running\":\" \",\"CIF_train_uid\":\" 08698\",\"train_status\":\"2\",\"CIF_stp_indicator\":\"N\",\"schedule_segment\":[{\"signalling_id\":\"\",\"uic_code\":\"\",\"atoc_code\":\"\",\"CIF_train_category\":\"\",\"CIF_headcode\":\"\",\"CIF_course_indicator\":\"\",\"CIF_train_service_code\":\"55460180\",\"CIF_business_sector\":\"\",\"CIF_power_type\":\"D\",\"CIF_timing_load\":\"\",\"CIF_speed\":\"\",\"CIF_operating_characteristics\":\"\",\"CIF_train_class\":\"\",\"CIF_sleepers\":\"\",\"CIF_reservations\":\"0\",\"CIF_connection_indicator\":\"\",\"CIF_catering_code\":\"\",\"CIF_service_branding\":\"\",\"CIF_traction_class\":\"\",\"schedule_location\":[{\"scheduled_arrival_time\":\" \",\"scheduled_departure_time\":\"124400\",\"scheduled_pass_time\":\" \",\"public_arrival_time\":\" \",\"public_departure_time\":\"00\",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\" \",\"CIF_activity\":\"TB\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"HMSHBRF\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"125000\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"WACRJN\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"130900\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"NNTNABJ\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"131100\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"NNTN\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"131800\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"HINCKLY\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"132430\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"CROFTS\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"133400\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"WGSTNNJ\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"133800\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"LESTRSJ\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"134030\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"LESTER\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"135900\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"SYSTNSJ\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"135930\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"SYSTNEJ\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"140700\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"MLTNFBY\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"141100\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"MLTNSDG\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"141830\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"MLTNWHI\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"142100\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"OAKHASH\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"142230\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"LAGHJN\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"142400\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"OAKHAM\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"142830\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"MANTONJ\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"143200\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"MANTLFF\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"143400\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"KETTON\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"144000\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"STAMUFF\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"144330\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"HELPSTN\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"144930\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"NENGLNN\"}}},{\"scheduled_arrival_time\":\"145230\",\"scheduled_departure_time\":\"161830\",\"scheduled_pass_time\":\"      \",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"PBROVGB\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"162100\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"PBROEFL\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"162330\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"PBRO\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"162500\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"PBROE\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"163200\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"WHTLSEA\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"164130\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"MRCH\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"165730\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"ELYYNJN\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"170030\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"ELYY\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"170130\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"ELYYDLN\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"171030\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"SOHAM\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"171830\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"CHPNJN\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"172200\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"KENNETT\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"173330\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"BSTEDMS\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"175000\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"HAGHLYJ\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"175200\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"STWMRKT\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"180300\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"IPSWESJ\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"180430\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"IPSWICH\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"180600\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"IPSWHJN\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"181800\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"MANNTNJ\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"181900\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"MANNTEJ\"}}},{\"scheduled_arrival_time\":\"      \",\"scheduled_departure_time\":\"      \",\"scheduled_pass_time\":\"182200\",\"public_arrival_time\":\"      \",\"public_departure_time\":\"      \",\"CIF_platform\":\"\",\"CIF_line\":\"\",\"CIF_path\":\"\",\"CIF_activity\":\"\",\"CIF_engineering_allowance\":\"\",\"CIF_pathing_allowance\":\"\",\"CIF_performance_allowance\":\"\",\"location\":{\"tiploc\":{\"tiploc_id\":\"MISTLEY\"}}},{\"scheduled_arrival_time\":\"183800\",\"scheduled_departure_time\":\" \",\"scheduled_pass_time\":\" \",\"public_arrival_time\":\"00\",\"public_departure_time\":\" \",\"CIF_platform\":\"\",\"CIF_line\":\" \",\"CIF_path\":\"\",\"CIF_activity\":\"TF\",\"CIF_engineering_allowance\":\" \",\"CIF_pathing_allowance\":\" \",\"CIF_performance_allowance\":\" \",\"location\":{\"tiploc\":{\"tiploc_id\":\"PRKSGBR\"}}}]}]}}}";
            ScheduleTrain train = null;

            var tiplocs = new TiplocRepository().GetTiplocs().ToList();

            var data = JsonConvert.DeserializeObject<dynamic>(msg);
            try
            {
                train = VSTPMapper.ParseJsonVSTP(data, tiplocs);
            }
            catch (TiplocNotFoundException tnfe)
            {
                Trace.TraceError("Could not add VSTP: {0}", tnfe);
            }
            catch (Exception e)
            {
                Trace.TraceError("Could not add VSTP: {0}", e);
            }
            Trace.TraceInformation(train.Headcode);
        }
    }
}
