namespace DigiDental.DataAccess.DbObject
{
    public class DBEntities
    {
        private DigiDentalEntities dde;

        public DBEntities()
        {
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }
        }
    }
}
