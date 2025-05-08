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

        public ITable<RegistroDiario> RegistroDiario => this.GetTable<RegistroDiario>();
        public ITable<Humor> Humores => this.GetTable<Humor>();
        public ITable<QualidadeSono> QualidadesSono => this.GetTable<QualidadeSono>();
        public ITable<Alimentacao> Alimentacoes => this.GetTable<Alimentacao>();
        public ITable<AtividadeFisica> AtividadesFisicas => this.GetTable<AtividadeFisica>();
        public ITable<Configuracao> Configuracoes => this.GetTable<Configuracao>();

        public bool TableExists<T>()
        {
            var tableName = this.MappingSchema.GetEntityDescriptor(typeof(T)).Name;
            var result = this.Query<string>($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'").FirstOrDefault();
            return !string.IsNullOrEmpty(result);
        }

        public static void CreateDatabase(string connectionString)
        {
            try
            {
                Console.WriteLine("Iniciando a criação do banco de dados...");
                using var db = new DiarioSaudeDb(connectionString);

                if (!db.TableExists<RegistroDiario>())
                {
                    db.CreateTable<RegistroDiario>();
                    Console.WriteLine("Tabela RegistroDiario criada com sucesso.");
                }

                if (!db.TableExists<Humor>())
                {
                    db.CreateTable<Humor>();
                    Console.WriteLine("Tabela Humor criada com sucesso.");
                }

                if (!db.TableExists<QualidadeSono>())
                {
                    db.CreateTable<QualidadeSono>();
                    Console.WriteLine("Tabela QualidadeSono criada com sucesso.");
                }

                if (!db.TableExists<Alimentacao>())
                {
                    db.CreateTable<Alimentacao>();
                    Console.WriteLine("Tabela Alimentacao criada com sucesso.");
                }

                if (!db.TableExists<AtividadeFisica>())
                {
                    db.CreateTable<AtividadeFisica>();
                    Console.WriteLine("Tabela AtividadeFisica criada com sucesso.");
                }

                if (!db.TableExists<Configuracao>())
                {
                    db.CreateTable<Configuracao>();
                    Console.WriteLine("Tabela Configuracao criada com sucesso.");
                }

                // Inserir valores padrão para Humor
                if (!db.Humores.ToList().Any())
                {
                    Console.WriteLine("Inserindo valores padrão para Humor");
                    db.Insert(new Humor { Descricao = "Feliz" });
                    db.Insert(new Humor { Descricao = "Triste" });
                    db.Insert(new Humor { Descricao = "Ansioso" });
                    db.Insert(new Humor { Descricao = "Neutro" });
                }

                // Inserir valores padrão para QualidadeSono
                if (!db.QualidadesSono.ToList().Any())
                {
                    Console.WriteLine("Inserindo valores padrão para QualidadeSono");
                    db.Insert(new QualidadeSono { Descricao = "Boa" });
                    db.Insert(new QualidadeSono { Descricao = "Regular" });
                    db.Insert(new QualidadeSono { Descricao = "Ruim" });
                }

                Console.WriteLine("Inicialização do banco de dados concluída com sucesso");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro grave ao inicializar o banco de dados: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}