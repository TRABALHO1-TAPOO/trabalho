using System;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;

namespace DiarioSaude.Models
{
    public class DiarioSaudeDb : DataConnection
    {
        public DiarioSaudeDb(string connectionString) : base(ProviderName.SQLite, connectionString)
        {
        }

        public ITable<RegistroDiario> RegistrosDiarios => this.GetTable<RegistroDiario>();
        public ITable<Humor> Humores => this.GetTable<Humor>();
        public ITable<QualidadeSono> QualidadesSono => this.GetTable<QualidadeSono>();
        public ITable<Alimentacao> Alimentacoes => this.GetTable<Alimentacao>();
        public ITable<AtividadeFisica> AtividadesFisicas => this.GetTable<AtividadeFisica>();
        public ITable<Configuracao> Configuracoes => this.GetTable<Configuracao>();

        public static void CreateDatabase(string connectionString)
        {
            using var db = new DiarioSaudeDb(connectionString);
            
            db.CreateTable<RegistroDiario>();
            db.CreateTable<Humor>();
            db.CreateTable<QualidadeSono>();
            db.CreateTable<Alimentacao>();
            db.CreateTable<AtividadeFisica>();
            db.CreateTable<Configuracao>();

            // Insert default values for Humor
            if (!db.Humores.AsQueryable().Any())
            {
                db.Insert(new Humor { Descricao = "Feliz" });
                db.Insert(new Humor { Descricao = "Triste" });
                db.Insert(new Humor { Descricao = "Ansioso" });
                db.Insert(new Humor { Descricao = "Neutro" });
            }

            // Insert default values for QualidadeSono
            if (!db.QualidadesSono.AsQueryable().Any())
            {
                db.Insert(new QualidadeSono { Descricao = "Boa" });
                db.Insert(new QualidadeSono { Descricao = "Média" });
                db.Insert(new QualidadeSono { Descricao = "Ruim" });
            }
        }
    }
} 