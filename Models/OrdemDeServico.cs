
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myapp.Models
{
    public class OrdemDeServico
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid ClientId { get; set; }
        public virtual Cliente Client { get; set; }

        [Required]
        public string Equipment { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string SerialNumber { get; set; }

        [Required]
        public string ProblemDescription { get; set; }

        public string TechnicalSolution { get; set; }

        public Guid StatusId { get; set; }
        public virtual Status Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid CreatedByUserId { get; set; }
        public virtual Usuario CreatedByUser { get; set; }

        public virtual ICollection<LogOS> Logs { get; set; }
    }
}
