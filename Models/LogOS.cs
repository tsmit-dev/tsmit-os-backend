
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myapp.Models
{
    public class LogOS
    {
        [Key]
        public Guid Id { get; set; }

        public int OsId { get; set; }
        [ForeignKey("OsId")]
        public virtual OrdemDeServico OrdemDeServico { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Usuario User { get; set; }

        public DateTime ChangeTimestamp { get; set; }

        [Required]
        public string ChangeDescription { get; set; }

        [Column(TypeName = "jsonb")]
        public string Details { get; set; }
    }
}
