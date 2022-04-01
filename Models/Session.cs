using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using echoStudy_webAPI.Areas.Identity.Data;
using echoStudy_webAPI.Data;
using System;

namespace echoStudy_webAPI.Models
{

    public class Session
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SessionID { get; set; }

        [Required]
        public PlayOrder PlayOrder { get; set; }

        [Required]
        public PlayType LearnReview { get; set; }

        [Required]
        public int MaxCards { get; set; }

        [Required]
        public Platform Platform { get; set; }

        [Required]
        public string Device { get; set; }

        [ForeignKey("EchoUser")]
        public virtual EchoUser User { get; set; }

        [Required]
        public int DeckID { get; set; }

        [Required]
        public Deck Deck { get; set; }

        [Required]
        public virtual ICollection<Card> CardsPlayed { get; set; }

        // These values should be manually handled in the controller
        [ScaffoldColumn(false)]
        public DateTime DateCreated { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateLastStudied{ get; set; }
    }
}