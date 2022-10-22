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

        [ForeignKey("DeckOwner")]
        public virtual string UserId { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DateCreated { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateUpdated { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateTouched { get; set; }

        [ForeignKey("OrigDeck")]
        public int? OrigDeckId { get; set; }
        [ForeignKey("OrigAuthor")]
        public string OrigAuthorId { get; set; }

        public virtual ICollection<DeckCategory> DeckCategories { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
        public virtual EchoUser DeckOwner { get; set; }
        public virtual Deck OrigDeck { get; set; }
        public virtual EchoUser OrigAuthor { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? StudyPercent 
        { 
            get
            {
                if(_context is null)
                {
                    return 0.0;
                }

                _context.Decks.Include(d => d.Cards);

                if(Cards is null || !Cards.Any())
                {
                    return 0.0;
                }

                double studyCount = 0.0;
                foreach(Card c in Cards)
                {
                    if (c.Score != 0)
                    {
                        studyCount++;
                    }
                }
                return Math.Round(studyCount / Cards.Count, 4);
            } 
            private set { } 
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? MasteredPercent
        {
            get
            {
                if (_context is null)
                {
                    return 0.0;
                }

                _context.Decks.Include(d => d.Cards);

                if (Cards is null || !Cards.Any())
                {
                    return 0.0;
                }

                double masteredCount = 0.0;
                foreach (Card c in Cards)
                {
                    if (c.Score > 3)
                    {
                        masteredCount++;
                    }
                }
                return Math.Round(masteredCount / Cards.Count, 4);
            }
            private set { }
        }
    }


}