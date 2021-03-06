﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Api;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class TrainMovementController : ApiController
    {
        private static readonly TrainMovementRepository _tmRepo = new TrainMovementRepository();

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        [Obsolete("Will be removed in future version")]
        public IHttpActionResult GetById(string id)
        {
            TrainMovementLink tm = _tmRepo.GetTrainMovementById(id);
            if (tm != null)
                return Ok(tm);
            else
                return NotFound();
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IHttpActionResult GetForHeadcode(string headcode, DateTime date, bool returnTiplocs = true)
        {
            return FromResults(_tmRepo.GetTrainMovementByHeadcode(headcode, date), returnTiplocs);
        }

        [HttpGet]
        [CachingActionFilterAttribute(30)]
        public IHttpActionResult GetForHeadcodeByLocation(string headcode, string crsCode, string platform)
        {
            TrainMovementLink link = _tmRepo.GetTrainMovementByHeadcodeAndLocation(headcode, crsCode, platform);

            if (link != null)
                return Ok(link);
            else
                return NotFound();
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IHttpActionResult GetForUid(string trainUid, DateTime date, bool returnTiplocs = true)
        {
            return FromResults(_tmRepo.GetTrainMovementById(trainUid, date), returnTiplocs);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IHttpActionResult StartingAtLocation(string stanox, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, string powerType = null,
            bool returnTiplocs = true)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            PowerType? pt = PowerTypeField.ParseDataString(powerType);
            return FromResults(_tmRepo.StartingAtLocation(stanox, startDate, endDate, atocCode, pt), returnTiplocs);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IHttpActionResult StartingAtStation(string crsCode, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, string powerType = null,
            bool returnTiplocs = true)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            PowerType? pt = PowerTypeField.ParseDataString(powerType);
            return FromResults(_tmRepo.StartingAtStation(crsCode, startDate, endDate, atocCode, pt), returnTiplocs);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IHttpActionResult CallingAtLocation(string stanox, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, string powerType = null,
            bool returnTiplocs = true)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            PowerType? pt = PowerTypeField.ParseDataString(powerType);
            return FromResults(_tmRepo.CallingAtLocation(stanox, startDate, endDate, atocCode, pt), returnTiplocs);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IHttpActionResult CallingAtStation(string crsCode, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, string powerType = null,
            bool returnTiplocs = true)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            PowerType? pt = PowerTypeField.ParseDataString(powerType);
            return FromResults(_tmRepo.CallingAtStation(crsCode, startDate, endDate, atocCode, pt), returnTiplocs);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IHttpActionResult CallingAtLocations(string fromStanox, string toStanox, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, string powerType = null,
            bool returnTiplocs = true)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            PowerType? pt = PowerTypeField.ParseDataString(powerType);
            return FromResults(_tmRepo.CallingBetweenLocations(fromStanox, toStanox, startDate, endDate, atocCode, pt), returnTiplocs);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IHttpActionResult CallingAtStations(string fromCrs, string toCrs, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, string powerType = null,
            bool returnTiplocs = true)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            PowerType? pt = PowerTypeField.ParseDataString(powerType);
            return FromResults(_tmRepo.CallingBetweenStations(fromCrs, toCrs, startDate, endDate, atocCode, pt), returnTiplocs);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IHttpActionResult TerminatingAtLocation(string stanox, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, string powerType = null,
            bool returnTiplocs = true)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            PowerType? pt = PowerTypeField.ParseDataString(powerType);
            return FromResults(_tmRepo.TerminatingAtLocation(stanox, startDate, endDate, atocCode, pt),returnTiplocs);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IHttpActionResult TerminatingAtStation(string crsCode, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, string powerType = null,
            bool returnTiplocs = true)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            PowerType? pt = PowerTypeField.ParseDataString(powerType);
            return FromResults(_tmRepo.TerminatingAtStation(crsCode, startDate, endDate, atocCode, pt),returnTiplocs);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IHttpActionResult GetNearestTrain(double lat, double lon, int limit = 5, bool returnTiplocs = true)
        {
            return FromResults(_tmRepo.NearestTrains(lat, lon, limit), returnTiplocs);
        }

        private IHttpActionResult FromResults(TrainMovementResult result, bool returnTiplocs = true)
        {
            if (result == null)
                return Ok();

            SingleTrainMovementResult actual = new SingleTrainMovementResult
            {
                Movement = result
            };
            ICollection<StationTiploc> tiplocs = null;
            if (returnTiplocs)
                tiplocs = new HashSet<StationTiploc>();
            if (result.Actual != null)
            {
                if (result.Actual.ScheduleOrigin != null)
                {
                    if (returnTiplocs)
                    {
                        tiplocs.Add(result.Actual.ScheduleOrigin);
                    }
                    result.Actual.ScheduleOriginStanoxCode = result.Actual.ScheduleOrigin.Stanox;
                }
                if (result.Actual.Stops != null && result.Actual.Stops.Any())
                {
                    result.Actual.Stops = result.Actual.Stops.Select(s =>
                    {
                        if (s.Tiploc != null)
                        {
                            if (returnTiplocs)
                            {
                                tiplocs.Add(s.Tiploc);
                            }
                            s.TiplocStanoxCode = s.Tiploc.Stanox;
                        }
                        return s;
                    });
                }
            }

            if (result.Schedule != null && result.Schedule.Stops != null && result.Schedule.Stops.Any())
            {
                result.Schedule.Stops = result.Schedule.Stops.Select(s =>
                {
                    if (s.Tiploc != null)
                    {
                        if (returnTiplocs)
                        {
                            tiplocs.Add(s.Tiploc);
                        }
                        s.TiplocStanoxCode = s.Tiploc.Stanox;
                    }
                    return s;
                });
            }

            if (result.Cancellations != null && result.Cancellations.Any())
            {
                result.Cancellations = result.Cancellations.Select(c =>
                {
                    if (c.CancelledAt != null)
                    {
                        if (returnTiplocs)
                        {
                            tiplocs.Add(c.CancelledAt);
                        }
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
                        if (returnTiplocs)
                        {
                            tiplocs.Add(c.NewOrigin);
                        }
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
                        if (returnTiplocs)
                        {
                            tiplocs.Add(r.NewOrigin);
                        }
                        r.NewOriginStanoxCode = r.NewOrigin.Stanox;
                    }
                    return r;
                });
            }
            actual.Tiplocs = tiplocs ?? Enumerable.Empty<StationTiploc>();

            return Ok(actual);
        }

        private IHttpActionResult FromResults(IEnumerable<TrainMovementResult> results, bool returnTiplocs = true)
        {
            TrainMovementResults actual = new TrainMovementResults();

            if (!results.Any())
            {
                actual.Movements = Enumerable.Empty<TrainMovementResult>();
                actual.Tiplocs = Enumerable.Empty<StationTiploc>();
                return Ok(actual);
            }

            ICollection<StationTiploc> tiplocs = null;
            if (returnTiplocs)
                tiplocs = new HashSet<StationTiploc>();
            actual.Movements = results.Select(m =>
            {
                if (m.Actual != null)
                {
                    if (m.Actual.ScheduleOrigin != null)
                    {
                        if (returnTiplocs)
                        {
                            tiplocs.Add(m.Actual.ScheduleOrigin);
                        }
                        m.Actual.ScheduleOriginStanoxCode = m.Actual.ScheduleOrigin.Stanox;
                    }
                    if (m.Actual.Stops != null && m.Actual.Stops.Any())
                    {
                        m.Actual.Stops = m.Actual.Stops.Select(s =>
                        {
                            if (s.Tiploc != null)
                            {
                                if (returnTiplocs)
                                {
                                    tiplocs.Add(s.Tiploc);
                                }
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
                            if (returnTiplocs)
                            {
                                tiplocs.Add(s.Tiploc);
                            }
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
                            if (returnTiplocs)
                            {
                                tiplocs.Add(c.CancelledAt);
                            }
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
                            if (returnTiplocs)
                            {
                                tiplocs.Add(c.NewOrigin);
                            }
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
                            if (returnTiplocs)
                            {
                                tiplocs.Add(r.NewOrigin);
                            }
                            r.NewOriginStanoxCode = r.NewOrigin.Stanox;
                        }
                        return r;
                    });
                }

                return m;
            });
            actual.Tiplocs = tiplocs ?? Enumerable.Empty<StationTiploc>();

            return Ok(actual);
        }
    }
}
