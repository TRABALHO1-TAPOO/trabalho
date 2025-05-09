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

                InsertDummyRegistrosDiarios(connectionString);

                Console.WriteLine("Inicialização do banco de dados concluída com sucesso");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro grave ao inicializar o banco de dados: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public static void InsertDummyRegistrosDiarios(string connectionString)
        {
            try
            {
                Console.WriteLine("Inserindo registros diários fictícios no banco de dados...");
                using var db = new DiarioSaudeDb(connectionString);

                // Verifica se há registros diários existentes
                if (!db.RegistroDiario.Any())
                {
                    // Insere dados fictícios de Alimentacao
                    var alimentacao1 = new Alimentacao { Descricao = "Café da manhã saudável", Calorias = 300 };
                    var alimentacao2 = new Alimentacao { Descricao = "Almoço balanceado", Calorias = 600 };
                    var alimentacao3 = new Alimentacao { Descricao = "Jantar leve", Calorias = 400 };
                    var alimentacao4 = new Alimentacao { Descricao = "Lanche da tarde", Calorias = 200 };
                    var alimentacao5 = new Alimentacao { Descricao = "Café da manhã reforçado", Calorias = 400 };
                    var alimentacao6 = new Alimentacao { Descricao = "Almoço vegetariano", Calorias = 500 };
                    var alimentacao7 = new Alimentacao { Descricao = "Jantar com sopa", Calorias = 300 };

                    var alimentacaoId1 = db.InsertWithInt32Identity(alimentacao1);
                    var alimentacaoId2 = db.InsertWithInt32Identity(alimentacao2);
                    var alimentacaoId3 = db.InsertWithInt32Identity(alimentacao3);
                    var alimentacaoId4 = db.InsertWithInt32Identity(alimentacao4);
                    var alimentacaoId5 = db.InsertWithInt32Identity(alimentacao5);
                    var alimentacaoId6 = db.InsertWithInt32Identity(alimentacao6);
                    var alimentacaoId7 = db.InsertWithInt32Identity(alimentacao7);

                    // Insere dados fictícios de AtividadeFisica
                    var atividade1 = new AtividadeFisica { TipoAtividade = "Caminhada", DuracaoMinutos = 30 };
                    var atividade2 = new AtividadeFisica { TipoAtividade = "Corrida", DuracaoMinutos = 45 };
                    var atividade3 = new AtividadeFisica { TipoAtividade = "Yoga", DuracaoMinutos = 60 };
                    var atividade4 = new AtividadeFisica { TipoAtividade = "Natação", DuracaoMinutos = 40 };
                    var atividade5 = new AtividadeFisica { TipoAtividade = "Ciclismo", DuracaoMinutos = 50 };
                    var atividade6 = new AtividadeFisica { TipoAtividade = "Treinamento funcional", DuracaoMinutos = 35 };
                    var atividade7 = new AtividadeFisica { TipoAtividade = "Pilates", DuracaoMinutos = 45 };

                    var atividadeId1 = db.InsertWithInt32Identity(atividade1);
                    var atividadeId2 = db.InsertWithInt32Identity(atividade2);
                    var atividadeId3 = db.InsertWithInt32Identity(atividade3);
                    var atividadeId4 = db.InsertWithInt32Identity(atividade4);
                    var atividadeId5 = db.InsertWithInt32Identity(atividade5);
                    var atividadeId6 = db.InsertWithInt32Identity(atividade6);
                    var atividadeId7 = db.InsertWithInt32Identity(atividade7);

                    // Obtém IDs das tabelas relacionadas
                    var humorId = db.Humores.FirstOrDefault()?.Id ?? 1; // Assume ID 1 se não houver dados
                    var sonoId = db.QualidadesSono.FirstOrDefault()?.Id ?? 1;

                    // Insere registros diários fictícios
                    db.Insert(new RegistroDiario
                    {
                        Data = DateTime.Now.AddDays(-1),
                        HumorId = humorId,
                        SonoId = sonoId,
                        AlimentacaoId = alimentacaoId1,
                        AtividadeFisicaId = atividadeId1
                    });

                    db.Insert(new RegistroDiario
                    {
                        Data = DateTime.Now.AddDays(-2),
                        HumorId = humorId,
                        SonoId = sonoId,
                        AlimentacaoId = alimentacaoId2,
                        AtividadeFisicaId = atividadeId2
                    });

                    db.Insert(new RegistroDiario
                    {
                        Data = DateTime.Now.AddDays(-3),
                        HumorId = humorId,
                        SonoId = sonoId,
                        AlimentacaoId = alimentacaoId3,
                        AtividadeFisicaId = atividadeId3
                    });

                    db.Insert(new RegistroDiario
                    {
                        Data = DateTime.Now.AddDays(-4),
                        HumorId = humorId,
                        SonoId = sonoId,
                        AlimentacaoId = alimentacaoId4,
                        AtividadeFisicaId = atividadeId4
                    });

                    db.Insert(new RegistroDiario
                    {
                        Data = DateTime.Now.AddDays(-5),
                        HumorId = humorId,
                        SonoId = sonoId,
                        AlimentacaoId = alimentacaoId5,
                        AtividadeFisicaId = atividadeId5
                    });

                    db.Insert(new RegistroDiario
                    {
                        Data = DateTime.Now.AddDays(-6),
                        HumorId = humorId,
                        SonoId = sonoId,
                        AlimentacaoId = alimentacaoId6,
                        AtividadeFisicaId = atividadeId6
                    });

                    db.Insert(new RegistroDiario
                    {
                        Data = DateTime.Now.AddDays(-7),
                        HumorId = humorId,
                        SonoId = sonoId,
                        AlimentacaoId = alimentacaoId7,
                        AtividadeFisicaId = atividadeId7
                    });

                    db.Insert(new RegistroDiario
                    {
                        Data = DateTime.Now.AddDays(-8),
                        HumorId = humorId,
                        SonoId = sonoId,
                        AlimentacaoId = alimentacaoId1,
                        AtividadeFisicaId = atividadeId2
                    });

                    db.Insert(new RegistroDiario
                    {
                        Data = DateTime.Now.AddDays(-9),
                        HumorId = humorId,
                        SonoId = sonoId,
                        AlimentacaoId = alimentacaoId2,
                        AtividadeFisicaId = atividadeId3
                    });

                    db.Insert(new RegistroDiario
                    {
                        Data = DateTime.Now.AddDays(-10),
                        HumorId = humorId,
                        SonoId = sonoId,
                        AlimentacaoId = alimentacaoId3,
                        AtividadeFisicaId = atividadeId1
                    });

                    Console.WriteLine("Registros diários fictícios inseridos com sucesso.");
                }
                else
                {
                    Console.WriteLine("Já existem registros diários no banco de dados. Nenhum dado fictício foi inserido.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir registros diários fictícios: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}