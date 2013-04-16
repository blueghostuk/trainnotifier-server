using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TrainNotifier.Common.Exceptions;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    public static class AssociationJsonMapper
    {
        public static Association ParseJsonAssociation(dynamic s, IEnumerable<TiplocCode> tiplocs)
        {
            var a = new Association();
            a.TransactionType = TransactionTypeField.ParseDataString(DynamicValueToString(s.transaction_type));
            a.MainTrainUid = StringField.ParseDataString(DynamicValueToString(s.main_train_uid));
            a.AssocTrainUid = StringField.ParseDataString(DynamicValueToString(s.assoc_train_uid));
            a.StartDate = DynamicValueToDateTime(s.assoc_start_date);
            string tiplocCode = DynamicValueToString(s.location);
            TiplocCode tiploc = tiplocs.FirstOrDefault(t => t.Tiploc.Equals(tiplocCode, StringComparison.InvariantCultureIgnoreCase));
            if (tiploc == null)
            {
                throw new TiplocNotFoundException(tiplocCode)
                {
                    Code = tiplocCode
                };
            }
            a.Location = tiploc;
            switch (a.TransactionType)
            {
                case TransactionType.Create:
                    a.EndDate = DynamicValueToDateTime(s.assoc_end_date);
                    a.Schedule = ScheduleField.ParseDataString(DynamicValueToString(s.assoc_days));
                    a.STPIndicator = STPIndicatorField.ParseDataString(DynamicValueToString(s.CIF_stp_indicator));
                    a.AssociationType = TrainAssociationTypeField.ParseDataString(DynamicValueToString(s.category));
                    a.DateType = TrainAssociationDateField.ParseDataString(DynamicValueToString(s.date_indicator));
                    break;
            }

            return a;
        }

        private static string DynamicValueToString(dynamic value)
        {
            try
            {
                if (value is JValue)
                {
                    return ((JValue)value).Value.ToString();
                }
            }
            catch
            {
            }

            return null;
        }

        private static DateTime DynamicValueToDateTime(dynamic value)
        {
            return (DateTime)((JValue)value).Value;
        }
    }
}
