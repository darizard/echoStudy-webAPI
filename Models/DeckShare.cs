using echoStudy_webAPI.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace echoStudy_webAPI.Models
{
    public class DeckShare
    {
        //required, EF Core defaults to ON DELETE CASCADE behavior (good)
        [ForeignKey("ClonedDeck")]
        [Key]
        public int ClonedDeckId { get; set; }

        //optional, EF Core defaults to ON DELETE SET NULL behavior (good)
        [ForeignKey("SourceDeck")]
        public int? SourceDeckId { get; set; }

        //required, EF Core defaults to ON DELETE CASCADE behavior (good)
        [ForeignKey("OrigAuthor")]
        [Required]
        public string OrigAuthorId { get; set; }

        [Required]
        public DateTime ShareDate { get; set; }

        public virtual Deck ClonedDeck { get; set; }
        public virtual Deck SourceDeck { get; set; }
        public virtual EchoUser OrigAuthor { get; set; }
    }
}