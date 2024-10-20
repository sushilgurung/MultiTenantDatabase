namespace Domain.Entities
{
    public class Tenants
    {
        public virtual int Id { get; set; }
        public string Name { get; set; }
        public string BusinessName { get; set; }
        public string Logo { get; set; }
        public string DataSource { get; set; }
        public string Catalog { get; set; }
        public string UserId { get; set; }
        /// <summary>
        /// Need to encrypt by column encryption by SQL server 
        /// </summary>
        public string Password { get; set; }
    }
}
