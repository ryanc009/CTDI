namespace CandidateProject
{
    using EntityModels;
    using System.Data.Entity;

    public partial class CartonContext : DbContext
    {
        public CartonContext()
            : base("name=CartonContext")
        {
        }

        public virtual DbSet<Carton> Cartons { get; set; }
        public virtual DbSet<CartonDetail> CartonDetails { get; set; }
        public virtual DbSet<Equipment> Equipments { get; set; }
        public virtual DbSet<ModelType> ModelTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Bug #1: setting cascade delete to true so that when the user can delete cartons with items in it
            //  and entity framework will take care of detailing all the child relationships to CartonDetails
            //HOWEVER changing it here without have code-first migrations will not update the db itself through a migration
            //  so had to loop through the cartons carton detail records in the actual deleteconfirmed controller function and remove them
            modelBuilder.Entity<Carton>()
                .HasMany(e => e.CartonDetails)
                .WithRequired(e => e.Carton)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.CartonDetails)
                .WithRequired(e => e.Equipment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ModelType>()
                .HasMany(e => e.Equipments)
                .WithRequired(e => e.ModelType)
                .WillCascadeOnDelete(false);
        }
    }
}
