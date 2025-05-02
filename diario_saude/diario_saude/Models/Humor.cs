using LinqToDB.Mapping;

namespace DiarioSaude.Models
{
    [Table(Name = "Humor")]
    public class Humor
    {
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column, NotNull]
        public required string Descricao { get; set; }
    }
} 