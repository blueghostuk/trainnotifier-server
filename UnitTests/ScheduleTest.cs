using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Service;
using Newtonsoft.Json;
using TrainNotifier.Common.Exceptions;
using System.Diagnostics;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class ScheduleTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            TrainCategory cat = TrainCategory.LondonUndergroundMetro;
            Assert.AreEqual(1, (int)cat);
            cat = TrainCategory.UnadvertisedPassenger;
            Assert.AreEqual(2, (int)cat);

            string schedule = "{\"JsonScheduleV1\":{\"CIF_bank_holiday_running\":null,\"CIF_stp_indicator\":\"N\",\"CIF_train_uid\":\"F34390\",\"applicable_timetable\":\"Y\",\"atoc_code\":\"LT\",\"new_schedule_segment\":{\"traction_class\":\"\",\"uic_code\":\"\"},\"schedule_days_runs\":\"0000010\",\"schedule_end_date\":\"2013-11-02\",\"schedule_segment\":{\"signalling_id\":\"2O17\",\"CIF_train_category\":\"OL\",\"CIF_headcode\":\"\",\"CIF_course_indicator\":1,\"CIF_train_service_code\":\"24682004\",\"CIF_business_sector\":\"??\",\"CIF_power_type\":\"EMU\",\"CIF_timing_load\":null,\"CIF_speed\":\"045\",\"CIF_operating_characteristics\":null,\"CIF_train_class\":null,\"CIF_sleepers\":null,\"CIF_reservations\":null,\"CIF_connection_indicator\":null,\"CIF_catering_code\":null,\"CIF_service_branding\":\"\",\"schedule_location\":[{\"location_type\":\"LO\",\"record_identity\":\"LO\",\"tiploc_code\":\"ELCT\",\"tiploc_instance\":null,\"departure\":\"0521H\",\"public_departure\":\"0521\",\"platform\":null,\"line\":null,\"engineering_allowance\":null,\"pathing_allowance\":null,\"performance_allowance\":null},{\"location_type\":\"LI\",\"record_identity\":\"LI\",\"tiploc_code\":\"TRNHMGN\",\"tiploc_instance\":null,\"arrival\":\"0531\",\"departure\":\"0531H\",\"pass\":null,\"public_arrival\":\"0531\",\"public_departure\":\"0531\",\"platform\":null,\"line\":null,\"path\":null,\"engineering_allowance\":null,\"pathing_allowance\":null,\"performance_allowance\":null},{\"location_type\":\"LI\",\"record_identity\":\"LI\",\"tiploc_code\":\"GNRSBRY\",\"tiploc_instance\":null,\"arrival\":\"0534\",\"departure\":\"0534H\",\"pass\":null,\"public_arrival\":\"0534\",\"public_departure\":\"0534\",\"platform\":\"1\",\"line\":null,\"path\":null,\"engineering_allowance\":null,\"pathing_allowance\":null,\"performance_allowance\":null},{\"location_type\":\"LI\",\"record_identity\":\"LI\",\"tiploc_code\":\"KEWGRDN\",\"tiploc_instance\":null,\"arrival\":\"0536H\",\"departure\":\"0537\",\"pass\":null,\"public_arrival\":\"0537\",\"public_departure\":\"0537\",\"platform\":null,\"line\":null,\"path\":null,\"engineering_allowance\":null,\"pathing_allowance\":null,\"performance_allowance\":null},{\"location_type\":\"LI\",\"record_identity\":\"LI\",\"tiploc_code\":\"RICHNLL\",\"tiploc_instance\":null,\"arrival\":\"0541\",\"departure\":\"0547\",\"pass\":null,\"public_arrival\":\"0541\",\"public_departure\":\"0547\",\"platform\":\"7\",\"line\":null,\"path\":null,\"engineering_allowance\":null,\"pathing_allowance\":null,\"performance_allowance\":null},{\"location_type\":\"LI\",\"record_identity\":\"LI\",\"tiploc_code\":\"KEWGRDN\",\"tiploc_instance\":\"2\",\"arrival\":\"0550\",\"departure\":\"0550H\",\"pass\":null,\"public_arrival\":\"0550\",\"public_departure\":\"0550\",\"platform\":null,\"line\":null,\"path\":null,\"engineering_allowance\":null,\"pathing_allowance\":null,\"performance_allowance\":null},{\"location_type\":\"LI\",\"record_identity\":\"LI\",\"tiploc_code\":\"GNRSBRY\",\"tiploc_instance\":\"2\",\"arrival\":\"0552H\",\"departure\":\"0553\",\"pass\":null,\"public_arrival\":\"0553\",\"public_departure\":\"0553\",\"platform\":\"2\",\"line\":null,\"path\":null,\"engineering_allowance\":null,\"pathing_allowance\":null,\"performance_allowance\":null},{\"location_type\":\"LI\",\"record_identity\":\"LI\",\"tiploc_code\":\"TRNHMGN\",\"tiploc_instance\":\"2\",\"arrival\":\"0556H\",\"departure\":\"0557H\",\"pass\":null,\"public_arrival\":\"0557\",\"public_departure\":\"0557\",\"platform\":null,\"line\":null,\"path\":null,\"engineering_allowance\":null,\"pathing_allowance\":null,\"performance_allowance\":null},{\"location_type\":\"LT\",\"record_identity\":\"LT\",\"tiploc_code\":\"TOWERHL\",\"tiploc_instance\":null,\"arrival\":\"0636H\",\"public_arrival\":\"0637\",\"platform\":null,\"path\":null}]},\"schedule_start_date\":\"2013-10-26\",\"train_status\":\"1\",\"transaction_type\":\"Create\"}}";

ScheduleTrain train = null;

            //var tiplocs = new TiplocRepository().GetTiplocs().ToList();

            var data = JsonConvert.DeserializeObject<dynamic>(schedule);
            try
            {
                train = ScheduleTrainMapper.ParseJsonTrain(data.JsonScheduleV1, null /*tiplocs*/);
            }
            catch (TiplocNotFoundException tnfe)
            {
                Trace.TraceError("Could not add Schedule: {0}", tnfe);
            }
            catch (Exception e)
            {
                Trace.TraceError("Could not add Schedule: {0}", e);
            }
            Trace.TraceInformation(train.Headcode);
        }
    }
}
