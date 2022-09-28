using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using echoStudy_webAPI.Data;
using System;
using echoStudy_webAPI.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace echoStudy_webAPI.Models
{
    public class Deck
    {
        private readonly EchoStudyDB _context;
        public Deck()
        {
        }

        public Deck(EchoStudyDB context)
        {
            _context = context;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeckID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(4000)]
        public string Description { get; set; }
        
        [Required]
        [StringLength(40)]
        public Access Access { get; set; }

        [Required]
        [StringLength(50)]
        public Language DefaultFrontLang { get; set; }

        [Required]
        [StringLength(50)]
        public Language DefaultBackLang { get; set; }

        [ForeignKey("EchoUser")]
        public virtual string UserId { get; set; }

        public virtual ICollection<DeckCategory> DeckCategories { get; set; }
        public virtual ICollection<Card> Cards { get; set; }

        // These values should be manually handled in the controller
        [ScaffoldColumn(false)]
        public DateTime DateCreated { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateUpdated { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateTouched { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double StudyPercent 
        { 
            get
            {
                var cardsWithScores = from c in _context.Cards
                                      where c.DeckID == this.DeckID
                                      select c;

                if (!cardsWithScores.Any()) return 0.0;

                double studyCount = 0.0;
                foreach(Card c in cardsWithScores.ToList())
                {
                    if (c.DateTouched != c.DateCreated && c.DateTouched >= c.DateUpdated)
                    {
                        studyCount++;
                    }
                }
                return Math.Round(studyCount / Cards.Count, 4);
            } 
            private set { } 
        }
    }


}