using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using FileReceiver.Dal.Entities.Enums;

namespace FileReceiver.Dal.Entities
{
    public class TransactionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserEntity User { get; set; }

        public TransactionTypeDb TransactionType { get; set; }
        [DataType("jsonb")]
        public string TransactionData { get; set; }

        public TransactionStateDb TransactionState { get; set; }
    }
}
