using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace E_Vote_System.Models
{ 


    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string  FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")] 
        public string  LastName { get; set; }
        
        [Display(Name = "Address")] 
        public string  Address { get; set; }

        public string ProfilePicture { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }

        public List<ElectionModel> ElectionModels { get; set; }

        public List<VoteModel> VotesCast { get; set; }

        [NotMapped]
        public string FullName { get { return GetFullName(); } }


        public string GetFullName()
        {
            try
            {

                return this.FirstName + " " + this.LastName;

            }catch(Exception e)
            {
                Utils.LogException(e);
                return null;
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ElectionTypes>().ToTable("tb_ElectionTypes");
            modelBuilder.Entity<ElectionModel>().ToTable("tb_Elections");
            modelBuilder.Entity<ElectionPositionModel>().ToTable("tb_ElectionPositions");
            modelBuilder.Entity<ElectionCandidateModel>().ToTable("tb_ElectionCandidates");
            modelBuilder.Entity<VoteModel>().ToTable("tb_Votes");
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<ElectionTypes> ElectionTypes { get; set; }
        public DbSet<ElectionModel> ElectionModels { get; set; }
        public DbSet<ElectionPositionModel> ElectionPositionModels { get; set; }
        public DbSet<ElectionCandidateModel> ElectionCandidateModels { get; set; }
        public DbSet<VoteModel> VoteModels { get; set; }
    }
}