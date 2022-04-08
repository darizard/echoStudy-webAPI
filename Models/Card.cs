using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using echoStudy_webAPI.Data;
using System;
using echoStudy_webAPI.Areas.Identity.Data;

namespace echoStudy_webAPI.Models
{
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CardID { get; set; }

        [Required]
        [StringLength(255)]
        public string FrontText { get; set; }

        [Required]
        [StringLength(255)]
        public string BackText { get; set; }

        [Required]
        [StringLength(4000)]
        public string FrontAudio { get; set; }

        [Required]
        [StringLength(4000)]
        public string BackAudio { get; set; }

        [Required]
        [StringLength(50)]
        public Language FrontLang { get; set; }

        [Required]
        [StringLength(50)]
        public Language BackLang { get; set; }

        [ForeignKey("EchoUser")]
        public string UserId { get; set; } 

        public virtual Deck Deck { get; set; }

        // unique composite key of DeckID and DeckPosition configured in EchoStudyDB.cs
        [Required]
        [ForeignKey("Deck")]
        public int DeckID { get; set; }
        [Required]
        public uint DeckPosition { get; set; }

        // These values should be manually handled in the controller
        [ScaffoldColumn(false)]
        public DateTime DateCreated { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateUpdated { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateTouched { get; set; }
        // This specifically value should be set to 0 upon the creation of a new card and updated as it is studied
        [ScaffoldColumn(false)]
        [Range(0, 100, ErrorMessage = "Scores must be between values 0 and 100")]
        public int Score { get; set; }
    }
}