﻿using AirlineReservationSystem.Core.Contracts;
using AirlineReservationSystem.Core.Models.AdminArea.Flight;
using AirlineReservationSystem.Core.Models.User_Area;
using AirlineReservationSystem.Infrastructure;
using AirlineReservationSystem.Infrastructure.Models;
using AirlineReservationSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AirlineReservationSystem.Core.Services
{

    public class FlightService : IFlightService
    {

        private readonly IApplicatioDbRepository repo;

        public FlightService(
            IApplicatioDbRepository _repo)
        {
            repo = _repo;
        }

        public async Task<bool> AddFlight(AddFlightVM model)
        {
            bool addedSuccessfully = false;

            model.FlightInformation = model.FlightInformation.Replace('T',' ');
            var culture = CultureInfo.CreateSpecificCulture("de-DE");

            var flight = new Flight()
            {
                AircraftID = model.Aircraft,
                FlightInformation = DateTime.ParseExact(model.FlightInformation, "yyyy-MM-dd HH:mm", culture),
                FlightStatus = Infrastructure.Status.Scheduled,
                FromId = model.DepartureCity,
                ToId = model.ArrivalCity,
                StandardTicketPrice = decimal.Parse(model.StandardTicketPrice, CultureInfo.CurrentCulture),

            };


            await repo.AddAsync(flight);
            await repo.SaveChangesAsync();

            return addedSuccessfully;
        }

        public async Task<bool> CancelFlight(string FlightId)
        {
            bool canceled = false;
            var FlightToCancel = await repo.GetByIdAsync<Flight>(FlightId);

            try
            {
                FlightToCancel.FlightStatus = Status.Canceled;
                await repo.SaveChangesAsync();
                canceled = true;
            }
            catch (Exception)
            {

                throw;
            }

            return canceled;
        }

        public async Task<IEnumerable<AvailableFlightsVM>> GetAllAvailableFlights()
        {
            return await repo.All<Flight>()
                .Where(f => f.FlightStatus == Status.Scheduled)
                .Select(x =>
                new AvailableFlightsVM
                {
                    DepartureDestination = x.To.City,
                    ArrivalDestination = x.From.City,
                    DateAndTime = x.FlightInformation.ToString(),
                    Price = x.StandardTicketPrice.ToString(),
                    FlightId = x.FlightId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FlightsForCancellationVM>> GetFlightsForCancellation()
        {
            var bookingsCount = await repo.All<Booking>()
                .ToListAsync();

            var flights = await repo.All<Flight>()
                .Where(f => f.FlightStatus == Status.Scheduled)
                .Select(f => new FlightsForCancellationVM
                {
                    DepartureDestination = f.To.City,
                    ArrivalDestination = f.From.City,
                    FlightId = f.FlightId,
                    DateAndTime = f.FlightInformation.ToString(),
                    Price = f.StandardTicketPrice.ToString()
                })
                .ToListAsync();

            flights.ForEach(x => x.NumberOfBookings = bookingsCount.Where(y => x.FlightId == y.FlightId).Count().ToString());

            return flights;
        }
    }
}
