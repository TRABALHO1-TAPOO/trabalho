using LinqToDB.Mapping;

namespace DiarioSaude.Models
{
    [Table(Name = "Alimentacao")]
    public class Alimentacao
    {
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column, NotNull]
        public required string Descricao { get; set; }

        [Column]
        public int? Calorias { get; set; }
    }
} 