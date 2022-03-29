using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using echoStudy_webAPI.Data;
using System;
using echoStudy_webAPI.Areas.Identity.Data;

namespace echoStudy_webAPI.Models
{
    public class Deck
    {
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

        [Required]
        public EchoUser User { get; set; }

        public virtual ICollection<DeckCategory> DeckCategories { get; set; }
        public virtual ICollection<Card> Cards { get; set; }

        // These values should be manually handled in the controller
        [ScaffoldColumn(false)]
        public DateTime DateCreated { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateUpdated { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateTouched { get; set; }
    }


}