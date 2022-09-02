using Domain.Shared.DTOs;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.DataAccess.Models
{
    public sealed class ExcelDb
    {
        public ExcelDb()
        {
        }

        public ExcelDb(ExcelDto dto)
        {
            Id = dto.Id;
        }

        [Key]
        public new Guid Id { get; set; }
    }
}