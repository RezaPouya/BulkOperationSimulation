using Domain.Shared.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Shared.DTOs
{
    [Serializable]
    public class ExcelDto : ExcelModel
    {
        public ExcelDto()
        {
            this.Id = Guid.NewGuid();
        }

        public ExcelDto(Guid id)
        {
            Id = id;
        }

        [Column("Id", Order = 1)]
        public Guid Id { get; set; }
    }
}