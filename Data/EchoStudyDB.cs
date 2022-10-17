using echoStudy_webAPI.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Models
{
    public class EchoStudyDB : IdentityDbContext<EchoUser>
    {
        public EchoStudyDB(DbContextOptions<EchoStudyDB> options)
            : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<DeckShare> DeckShares { get; set; }
        public DbSet<DeckCategory> DeckCategories{ get; set; }
        public DbSet<Session> Sessions{ get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }
    }
}
