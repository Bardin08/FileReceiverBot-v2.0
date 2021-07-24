using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using FileReceiver.Dal.Entities.Enums;

namespace FileReceiver.Dal.Entities
{
    public class FileReceivingSessionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserEntity User { get; set; }

        public FileReceivingSessionStateDb SessionState { get; set; }
        public int FilesReceived { get; set; }
        public int MaxFiles { get; set; }
        public FileStorageTypeDb? Storage { get; set; }

        [DataType("jsonb")]
        public string Constrains { get; set; }

        public DateTimeOffset CreateData { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        public SessionEndReasonDb? SessionEndReason { get; set; }
    }
}
