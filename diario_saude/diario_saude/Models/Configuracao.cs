using LinqToDB.Mapping;

namespace DiarioSaude.Models
{
    [Table(Name = "Configuracao")]
    public class Configuracao
    {
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column, NotNull]
        public required string CaminhoBanco { get; set; }

        [Column, NotNull]
        public required string Tema { get; set; } // "claro" ou "escuro"
    }
} 