using LinqToDB.Mapping;

namespace DiarioSaude.Models
{
    [Table(Name = "Configuracao")]
    public class Configuracao
    {
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column, NotNull]
        public string CaminhoBanco { get; set; }

        [Column, NotNull]
        public string Tema { get; set; } // "claro" ou "escuro"
    }
} 