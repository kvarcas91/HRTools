using Domain.DataValidation.AWAL;
using Domain.DataValidation.Resignation;
using Domain.Models.AWAL;
using Domain.Models.Resignations;
using FluentAssertions;
using System;
using Xunit;

namespace HRTools.Test.Validations
{

    public class ValidationTest
    {

        [Theory]
        [InlineData("01/01/0001", "", "", false)]
        [InlineData("01/01/0001", "some reason", "", false)]
        [InlineData("01/01/0001", "some reason", "google.com", false)]
        [InlineData("01/01/0001", "", "google.com", false)]
        [InlineData("01/01/0001", "some reason", "amazon.com", false)]
        [InlineData("01/01/2022", "", "", false)]
        [InlineData("01/01/2022", "some reason", "", false)]
        [InlineData("01/01/2022", "some reason", "google.com", false)]
        [InlineData("01/01/2022", "", "google.com", false)]
        [InlineData("01/01/2022", "", "amazon.com", false)]
        [InlineData("01/01/2022", "some reason", "amazon.com", true)]
        public void ResignationValidation(string date, string reason, string link, bool expectedPass)
        {
            var resignationEntry = new ResignationEntry
            {
                LastWorkingDay = DateTime.Parse(date),
                ReasonForResignation = reason,
                TTLink = link
            };
            var resignationEntinty = new ResignationEntity(resignationEntry);

            var validation = new ResignationValidation();
            var response = validation.Validate(resignationEntinty);

            response.Success.Should().Be(expectedPass);
        }

        [Theory]
        // No data
        [InlineData("06/12/2015","01/01/0001", "01/01/0001", "01/01/0001", "01/01/0001", "", false)]

        // first NCNS date is missing
        [InlineData("06/12/2015", "01/01/0001", "01/01/2022", "01/01/2022", "01/01/2022", "some outcome", false)]
        [InlineData("06/12/2015", "01/01/0001", "01/01/0001", "01/01/2022", "01/01/2022", "some outcome", false)]
        [InlineData("06/12/2015", "01/01/0001", "01/01/2022", "01/01/0001", "01/01/2022", "some outcome", false)]
        [InlineData("06/12/2015", "01/01/0001", "01/01/2022", "01/01/2022", "01/01/0001", "some outcome", false)]
        [InlineData("06/12/2015", "01/01/0001", "01/01/2022", "01/01/2022", "01/01/2022", "", false)]

        // Has first NCNS date
        [InlineData("06/12/2015", "01/01/2022", "01/01/0001", "01/01/0001", "01/01/0001", "", true)]

        // Awal 1 request before NCNS date or on the same day for non-probation population
        [InlineData("06/12/2015", "02/01/2022", "01/01/2022", "01/01/0001", "01/01/0001", "", false)]
        [InlineData("06/12/2015", "02/01/2022", "02/01/2022", "01/01/0001", "01/01/0001", "", false)]

        // AWAL 1 sent on time
        [InlineData("06/12/2015", "01/01/2022", "02/01/2022", "01/01/0001", "01/01/0001", "", true)]

        // AWAL 2 sent without AWAL 1
        [InlineData("06/12/2015", "01/01/2022", "01/01/0001", "09/01/2022", "01/01/0001", "", false)]

        // AWAL 2 requested earlier than it should've been
        [InlineData("01/12/2015", "01/01/2022", "02/01/2022", "07/01/2022", "20/01/2022", "", false)]
        [InlineData("01/12/2015", "01/01/2022", "02/01/2022", "05/01/2022", "20/01/2022", "", false)]

        //AWAL 2 sent on time
        [InlineData("01/12/2015", "01/01/2022", "02/01/2022", "08/01/2022", "20/01/2022", "", true)]

        // Disciplinary meeting scheduled without AWAL dates
        [InlineData("06/12/2015", "01/01/2022", "01/01/0001", "01/01/0001", "01/01/2022", "", false)]
        [InlineData("06/12/2015", "01/01/2022", "01/01/2022", "01/01/0001", "01/01/2022", "", false)]
        [InlineData("06/12/2015", "01/01/2022", "01/01/0001", "01/01/2022", "01/01/2022", "", false)]

        // Outcome without disciplinary date
        [InlineData("06/12/2015", "01/01/2022", "02/01/2022", "09/01/2022", "01/01/0001", "some outcome", false)]
        public void AwalValidation(string employmentStartDate, string ncnsDate, string awal1Date, string awal2Date, string disciplinaryDate, string outcome, bool expectedPass)
        {
            var awalEntity = new AwalEntity
            {
                EmploymentStartDate = DateTime.Parse(employmentStartDate),
                FirstNCNSDate = DateTime.Parse(ncnsDate),
                Awal1SentDate = DateTime.Parse(awal1Date),
                Awal2SentDate = DateTime.Parse(awal2Date),
                DisciplinaryDate = DateTime.Parse(disciplinaryDate),
                Outcome = outcome
            };

            var validation = new AwalValidation();
            var response = validation.Validate(awalEntity);

            response.Success.Should().Be(expectedPass);
        }

    }
}
