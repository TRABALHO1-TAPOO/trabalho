using System;
using LinqToDB.Mapping;

namespace DiarioSaude.Models
{
    [Table(Name = "RegistroDiario")]
    public class RegistroDiario
    {
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column, NotNull]
        public DateTime Data { get; set; }

        [Column, NotNull]
        public int HumorId { get; set; }

        [Column, NotNull]
        public int SonoId { get; set; }

        [Column, NotNull]
        public int AlimentacaoId { get; set; }

        [Column, NotNull]
        public int AtividadeFisicaId { get; set; }
    }
} 