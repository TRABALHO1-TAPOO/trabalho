using LinqToDB.Mapping;

namespace DiarioSaude.Models
{
    [Table(Name = "QualidadeSono")]
    public class QualidadeSono
    {
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column, NotNull]
        public required string Descricao { get; set; }
    }
} 