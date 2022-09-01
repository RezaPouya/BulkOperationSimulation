using DTOs.Base;
using System;

namespace Operator.DTOs
{
    [Serializable]
    public class ExcelDto : ExcelModel
    {
        public ExcelDto()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        public ExcelDto(Guid id)
        {
            Id = id;
        }
    }
}