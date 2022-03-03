using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using echoStudy_webAPI.Data;

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
        public string AudioFile { get; set; }

        [Required]
        [StringLength(50)]
        public Language FrontLang { get; set; }

        [Required]
        [StringLength(50)]
        public Language BackLang { get; set; }

        public ICollection<Deck> Decks { get; set; }
    }
}