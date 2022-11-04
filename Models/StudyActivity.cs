using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using echoStudy_webAPI.Areas.Identity.Data;
using echoStudy_webAPI.Data;
using System;
using System.Diagnostics;

namespace echoStudy_webAPI.Models
{
    public class StudyActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        // These three fields together must be unique -- configured in fluent api
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        [ForeignKey("Deck")]
        public int? DeckId { get; set; }
        public DateTime DateStudied { get; set; }

        public virtual EchoUser User { get; set; }
        public virtual Deck Deck { get; set; }
    }
}