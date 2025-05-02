using LinqToDB.Mapping;

namespace DiarioSaude.Models
{
    [Table(Name = "AtividadeFisica")]
    public class AtividadeFisica
    {
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column, NotNull]
        public required string TipoAtividade { get; set; }

        [Column, NotNull]
        public int DuracaoMinutos { get; set; }
    }
} 