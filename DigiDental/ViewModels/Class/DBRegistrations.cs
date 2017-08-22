using System;
using System.Linq;

namespace DigiDental.ViewModels.Class
{
    public class DBRegistrations
    {
        private DigiDentalEntities dde;
        private int Registration_ID;
        public DBRegistrations()
        {
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }
        }

        public int CreateRegistrationsAndGetID(Patients Patients, DateTime RegistrationDate)
        {
            var queryRegistration = from qr in dde.Registrations
                                    where qr.Patient_ID == Patients.Patient_ID && qr.Registration_Date == RegistrationDate.Date
                                    select qr;
            if (queryRegistration.Count() == 0)
            {
                var newRegistration = new Registrations
                {
                    Patient_ID = Patients.Patient_ID,
                    Registration_Date = RegistrationDate
                };
                dde.Registrations.Add(newRegistration);
                dde.SaveChanges();
                Registration_ID = newRegistration.Registration_ID;
            }
            else
            {
                Registration_ID = queryRegistration.First().Registration_ID;
            }

            return Registration_ID;
        }
    }
}
