using ExcelHelper;

namespace ExcelHelper.Models
{
    public class ExcelObj : ExcelModel
    {
        public ExcelObj()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        public ExcelObj(Guid id)
        {
            Id = id;
        }
    }
}