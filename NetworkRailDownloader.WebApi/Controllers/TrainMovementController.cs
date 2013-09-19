using System;
using System.Collections.Generic;
using System.Web.Http;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Api;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Service;
using System.Linq;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class TrainMovementController : ApiController
    {
        private static readonly TrainMovementRepository _tmRepo = new TrainMovementRepository();

        [HttpGet]
        [Obsolete("Will be removed in future version")]
        public ViewModelTrainMovement GetById(string id)
        {
            return _tmRepo.GetTrainMovementById(id);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public TrainMovementResults GetForHeadcode(string headcode, DateTime date)
        {
            return FromResults(_tmRepo.GetTrainMovementByHeadcode(headcode, date));
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public SingleTrainMovementResult GetForUid(string trainUid, DateTime date)
        {
            return FromResults(_tmRepo.GetTrainMovementById(trainUid, date));
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public TrainMovementResults StartingAtLocation(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return FromResults(_tmRepo.StartingAtLocation(stanox, startDate, endDate));
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public TrainMovementResults StartingAtStation(string crsCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return FromResults(_tmRepo.StartingAtStation(crsCode, startDate, endDate));
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public TrainMovementResults CallingAtLocation(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return FromResults(_tmRepo.CallingAtLocation(stanox, startDate, endDate));
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public TrainMovementResults CallingAtStation(string crsCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return FromResults(_tmRepo.CallingAtStation(crsCode, startDate, endDate));
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public TrainMovementResults CallingAtLocations(string fromStanox, string toStanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return FromResults(_tmRepo.CallingBetweenLocations(fromStanox, toStanox, startDate, endDate));
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public TrainMovementResults CallingAtStations(string fromCrs, string toCrs, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return FromResults(_tmRepo.CallingBetweenStations(fromCrs, toCrs, startDate, endDate));
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public TrainMovementResults TerminatingAtLocation(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return FromResults(_tmRepo.TerminatingAtLocation(stanox, startDate, endDate));
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public TrainMovementResults TerminatingAtStation(string crsCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return FromResults(_tmRepo.TerminatingAtStation(crsCode, startDate, endDate));
        }
        private static SingleTrainMovementResult FromResults(TrainMovementResult result)
        {
            if (result == null)
                return null;

            SingleTrainMovementResult actual = new SingleTrainMovementResult
            {
                Movement = result
            };
            HashSet<StationTiploc> tiplocs = new HashSet<StationTiploc>();
            if (result.Actual != null)
            {
                if (result.Actual.ScheduleOrigin != null)
                {
                    tiplocs.Add(result.Actual.ScheduleOrigin);
                    result.Actual.ScheduleOriginStanoxCode = result.Actual.ScheduleOrigin.Stanox;
                }
                if (result.Actual.Stops != null && result.Actual.Stops.Any())
                {
                    result.Actual.Stops = result.Actual.Stops.Select(s =>
                    {
                        if (s.Tiploc != null)
                        {
                            tiplocs.Add(s.Tiploc);
                            s.TiplocStanoxCode = s.Tiploc.Stanox;
                        }
                        return s;
                    });
                }
            }

            if (result.Schedule != null && result.Schedule.Stops != null && result.Schedule.Stops.Any())
                result.Schedule.Stops = result.Schedule.Stops.Select(s =>
                {
                    if (s.Tiploc != null)
                    {
                        tiplocs.Add(s.Tiploc);
                        s.TiplocStanoxCode = s.Tiploc.Stanox;
                    }
                    return s;
                });

            if (result.Cancellations != null && result.Cancellations.Any())
            {
                result.Cancellations = result.Cancellations.Select(c =>
                {
                    if (c.CancelledAt != null)
                    {
                        tiplocs.Add(c.CancelledAt);
                        c.CancelledAtStanoxCode = c.CancelledAt.Stanox;
                    }
                    return c;
                });
            }

            if (result.ChangeOfOrigins != null && result.ChangeOfOrigins.Any())
            {
                result.ChangeOfOrigins = result.ChangeOfOrigins.Select(c =>
                {
                    if (c.NewOrigin != null)
                    {
                        tiplocs.Add(c.NewOrigin);
                        c.NewOriginStanoxCode = c.NewOrigin.Stanox;
                    }
                    return c;
                });
            }

            if (result.Reinstatements != null && result.Reinstatements.Any())
            {
                result.Reinstatements = result.Reinstatements.Select(r =>
                {
                    if (r.NewOrigin != null)
                    {
                        tiplocs.Add(r.NewOrigin);
                        r.NewOriginStanoxCode = r.NewOrigin.Stanox;
                    }
                    return r;
                });
            }
            actual.Tiplocs = tiplocs;

            return actual;
        }

        private static TrainMovementResults FromResults(IEnumerable<TrainMovementResult> results)
        {
            if (!results.Any())
                return null;

            TrainMovementResults actual = new TrainMovementResults();
            HashSet<StationTiploc> tiplocs = new HashSet<StationTiploc>();
            actual.Movements = results.Select(m =>
            {
                if (m.Actual != null)
                {
                    if (m.Actual.ScheduleOrigin != null)
                    {
                        tiplocs.Add(m.Actual.ScheduleOrigin);
                        m.Actual.ScheduleOriginStanoxCode = m.Actual.ScheduleOrigin.Stanox;
                    }
                    if (m.Actual.Stops != null && m.Actual.Stops.Any())
                    {
                        m.Actual.Stops = m.Actual.Stops.Select(s =>
                        {
                            if (s.Tiploc != null)
                            {
                                tiplocs.Add(s.Tiploc);
                                s.TiplocStanoxCode = s.Tiploc.Stanox;
                            }
                            return s;
                        });
                    }
                }

                if (m.Schedule != null && m.Schedule.Stops != null && m.Schedule.Stops.Any())
                    m.Schedule.Stops = m.Schedule.Stops.Select(s =>
                    {
                        if (s.Tiploc != null)
                        {
                            tiplocs.Add(s.Tiploc);
                            s.TiplocStanoxCode = s.Tiploc.Stanox;
                        }
                        return s;
                    });

                if (m.Cancellations != null && m.Cancellations.Any())
                {
                    m.Cancellations = m.Cancellations.Select(c =>
                    {
                        if (c.CancelledAt != null)
                        {
                            tiplocs.Add(c.CancelledAt);
                            c.CancelledAtStanoxCode = c.CancelledAt.Stanox;
                        }
                        return c;
                    });
                }

                if (m.ChangeOfOrigins != null && m.ChangeOfOrigins.Any())
                {
                    m.ChangeOfOrigins = m.ChangeOfOrigins.Select(c =>
                    {
                        if (c.NewOrigin != null)
                        {
                            tiplocs.Add(c.NewOrigin);
                            c.NewOriginStanoxCode = c.NewOrigin.Stanox;
                        }
                        return c;
                    });
                }

                if (m.Reinstatements != null && m.Reinstatements.Any())
                {
                    m.Reinstatements = m.Reinstatements.Select(r =>
                    {
                        if (r.NewOrigin != null)
                        {
                            tiplocs.Add(r.NewOrigin);
                            r.NewOriginStanoxCode = r.NewOrigin.Stanox;
                        }
                        return r;
                    });
                }

                return m;
            });
            actual.Tiplocs = tiplocs;

            return actual;
        }
    }
}
