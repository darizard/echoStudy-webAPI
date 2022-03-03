using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Models
{
    public class EchoStudyDB : DbContext
    {
        public EchoStudyDB(DbContextOptions<EchoStudyDB> options)
            : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<DeckCategory> DeckCategories{ get; set; }
        public DbSet<Session> Sessions{ get; set; }
        
    }
}
