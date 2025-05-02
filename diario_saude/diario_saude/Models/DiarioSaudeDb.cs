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
            try
            {
                using var db = new DiarioSaudeDb(connectionString);
                
                // Verifica se as tabelas existem e tenta criar se não existirem
                try { db.CreateTable<RegistroDiario>(); } catch { Console.WriteLine("Tabela RegistroDiario já existe ou erro ao criar"); }
                try { db.CreateTable<Humor>(); } catch { Console.WriteLine("Tabela Humor já existe ou erro ao criar"); }
                try { db.CreateTable<QualidadeSono>(); } catch { Console.WriteLine("Tabela QualidadeSono já existe ou erro ao criar"); }
                try { db.CreateTable<Alimentacao>(); } catch { Console.WriteLine("Tabela Alimentacao já existe ou erro ao criar"); }
                try { db.CreateTable<AtividadeFisica>(); } catch { Console.WriteLine("Tabela AtividadeFisica já existe ou erro ao criar"); }
                try { db.CreateTable<Configuracao>(); } catch { Console.WriteLine("Tabela Configuracao já existe ou erro ao criar"); }

                // Insert default values for Humor if table is empty
                try
                {
                    if (!db.Humores.AsQueryable().Any())
                    {
                        Console.WriteLine("Inserindo valores padrão para Humor");
                        db.Insert(new Humor { Descricao = "Feliz" });
                        db.Insert(new Humor { Descricao = "Triste" });
                        db.Insert(new Humor { Descricao = "Ansioso" });
                        db.Insert(new Humor { Descricao = "Neutro" });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao inserir valores padrão em Humor: {ex.Message}");
                }

                // Insert default values for QualidadeSono if table is empty
                try
                {
                    if (!db.QualidadesSono.AsQueryable().Any())
                    {
                        Console.WriteLine("Inserindo valores padrão para QualidadeSono");
                        db.Insert(new QualidadeSono { Descricao = "Boa" });
                        db.Insert(new QualidadeSono { Descricao = "Média" });
                        db.Insert(new QualidadeSono { Descricao = "Ruim" });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao inserir valores padrão em QualidadeSono: {ex.Message}");
                }

                Console.WriteLine("Inicialização do banco de dados concluída com sucesso");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro grave ao inicializar o banco de dados: {ex.Message}");
                throw; // Re-throw para que a camada superior possa lidar com o erro
            }
        }
    }
} 