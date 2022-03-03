using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using echoStudy_webAPI.Data;

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

        public ICollection<DeckCategory> DeckCategories { get; set; }
        public ICollection<Card> Cards { get; set; }
    }


}