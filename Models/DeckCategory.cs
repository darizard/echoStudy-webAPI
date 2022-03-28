using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using echoStudy_webAPI.Areas.Identity.Data;

namespace echoStudy_webAPI.Models
{
    public class DeckCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(4000)]
        public string Description { get; set; }

        public ICollection<Deck> Decks { get; set; }

        [Required]
        public EchoUser User { get; set; }

        // These values should be manually handled in the controller
        [ScaffoldColumn(false)]
        public DateTime DateCreated { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateUpdated { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateTouched { get; set; }
    }
}