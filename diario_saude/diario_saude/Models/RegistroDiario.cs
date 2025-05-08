using System;
using LinqToDB.Mapping;

namespace DiarioSaude.Models
{
    [Table(Name = "RegistroDiario")]
    public class RegistroDiario
    {
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column(Name = "Data"), NotNull]
        public DateTime Data { get; set; }

        [Column(Name = "HumorId"), NotNull]
        public int HumorId { get; set; }

        [Column(Name = "SonoId"), NotNull]
        public int SonoId { get; set; }

        [Column(Name = "AlimentacaoId"), NotNull]
        public int AlimentacaoId { get; set; }

        [Column(Name = "AtividadeFisicaId"), NotNull]
        public int AtividadeFisicaId { get; set; }
    }
}