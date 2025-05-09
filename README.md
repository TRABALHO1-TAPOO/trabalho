# Documentação do Banco de Dados - Diário de Saúde

Este documento é um guia completo do banco de dados utilizado na aplicação Diário de Saúde. Foi criado para facilitar o entendimento mesmo para quem não tem experiência prévia com bancos de dados ou com este sistema.

## O que é o banco de dados neste projeto?

Nosso aplicativo Diário de Saúde utiliza um banco de dados SQLite para armazenar todas as informações do usuário. SQLite é um banco de dados leve e simples que funciona como um arquivo único no computador do usuário, não precisando de um servidor separado.

A localização do arquivo de banco de dados é:
`%LocalAppData%\DiarioSaude\diariosaude.db`

Por exemplo, em um computador Windows, seria algo como:
`C:\Users\SeuUsuario\AppData\Local\DiarioSaude\diariosaude.db`

## Organização dos Dados (Tabelas)

O banco de dados está organizado em seis tabelas principais, cada uma com uma função específica:

### 1. RegistroDiario
Esta é a tabela principal que armazena as entradas diárias do usuário. Cada registro contém referências para humor, sono, alimentação e atividade física.

```csharp
public class RegistroDiario
{
    [PrimaryKey, Identity]  // Chave primária que identifica unicamente cada registro
    public int Id { get; set; }

    [Column, NotNull]  // Data em que o registro foi feito
    public DateTime Data { get; set; }

    [Column, NotNull]  // Referência ao humor selecionado (conecta com a tabela Humor)
    public int HumorId { get; set; }

    [Column, NotNull]  // Referência à qualidade do sono (conecta com a tabela QualidadeSono)
    public int SonoId { get; set; }

    [Column, NotNull]  // Referência à alimentação (conecta com a tabela Alimentacao)
    public int AlimentacaoId { get; set; }

    [Column, NotNull]  // Referência à atividade física (conecta com a tabela AtividadeFisica)
    public int AtividadeFisicaId { get; set; }
}
```

### 2. Humor
Armazena as opções de humor que o usuário pode selecionar.

```csharp
public class Humor
{
    [PrimaryKey, Identity]
    public int Id { get; set; }

    [Column, NotNull]
    public string Descricao { get; set; }  // Por exemplo: "Feliz", "Triste", "Ansioso", "Neutro"
}
```

### 3. QualidadeSono
Armazena as opções de qualidade de sono.

```csharp
public class QualidadeSono
{
    [PrimaryKey, Identity]
    public int Id { get; set; }

    [Column, NotNull]
    public string Descricao { get; set; }  // Por exemplo: "Boa", "Média", "Ruim"
}
```

### 4. Alimentacao
Armazena informações sobre as refeições do usuário.

```csharp
public class Alimentacao
{
    [PrimaryKey, Identity]
    public int Id { get; set; }

    [Column, NotNull]
    public string Descricao { get; set; }  // Descrição da alimentação
}
```

### 5. AtividadeFisica
Armazena informações sobre os exercícios realizados.

```csharp
public class AtividadeFisica
{
    [PrimaryKey, Identity]
    public int Id { get; set; }

    [Column, NotNull]
    public string Descricao { get; set; }  // Descrição da atividade física
}
```

### 6. Configuracao
Armazena configurações do aplicativo.

```csharp
public class Configuracao
{
    [PrimaryKey, Identity]
    public int Id { get; set; }

    [Column, NotNull]
    public bool TemaEscuro { get; set; }  // Preferência de tema do aplicativo
}
```

## Como o banco de dados é criado?

Quando o aplicativo é iniciado pela primeira vez, ele:

1. Verifica se o diretório para armazenar o banco de dados existe
2. Cria o diretório se necessário
3. Define o caminho completo para o arquivo do banco de dados
4. Cria o arquivo do banco de dados se não existir
5. Cria todas as tabelas necessárias
6. Insere dados padrão nas tabelas de referência (como opções de humor e qualidade de sono)

O código responsável por isso está na classe `App.cs` e na classe `DiarioSaudeDb.cs`:

```csharp
// Em App.cs - Inicialização do aplicativo
public override void Initialize()
{
    // ... código de inicialização ...
    
    // Define o caminho do banco de dados
    string appDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DiarioSaude");
        
    // Cria o diretório se não existir
    if (!Directory.Exists(appDataPath))
        Directory.CreateDirectory(appDataPath);
        
    DbPath = Path.Combine(appDataPath, "diariosaude.db");
    
    // String de conexão para o SQLite
    string connectionString = $"Data Source={DbPath}";
    
    // Chama o método que cria o banco de dados e as tabelas
    DiarioSaudeDb.CreateDatabase(connectionString);
}
```

O método `CreateDatabase` na classe `DiarioSaudeDb` cria as tabelas e insere dados iniciais:

```csharp
public static void CreateDatabase(string connectionString)
{
    using var db = new DiarioSaudeDb(connectionString);
    
    // Cria as tabelas se não existirem
    db.CreateTable<RegistroDiario>();
    db.CreateTable<Humor>();
    db.CreateTable<QualidadeSono>();
    db.CreateTable<Alimentacao>();
    db.CreateTable<AtividadeFisica>();
    db.CreateTable<Configuracao>();

    // Verifica se a tabela Humor está vazia e insere valores padrão
    if (!db.Humores.AsQueryable().Any())
    {
        db.Insert(new Humor { Descricao = "Feliz" });
        db.Insert(new Humor { Descricao = "Triste" });
        db.Insert(new Humor { Descricao = "Ansioso" });
        db.Insert(new Humor { Descricao = "Neutro" });
    }

    // Insere valores padrão para QualidadeSono
    if (!db.QualidadesSono.AsQueryable().Any())
    {
        db.Insert(new QualidadeSono { Descricao = "Boa" });
        db.Insert(new QualidadeSono { Descricao = "Média" });
        db.Insert(new QualidadeSono { Descricao = "Ruim" });
    }
}
```

## Como acessar e modificar os dados? (Repositório)

Para facilitar o acesso ao banco de dados, o projeto utiliza um padrão chamado "Repository" (Repositório). Isso significa que existe uma classe especial (`DiarioSaudeRepository`) que contém todos os métodos necessários para trabalhar com o banco de dados.

### Como funciona o Repositório

A classe `DiarioSaudeRepository` é como um intermediário entre seu código e o banco de dados. Em vez de escrever comandos SQL diretos, você usa métodos simples desta classe.

### Ciclo de vida do Repositório

1. **Criação**: Você cria uma instância do repositório com a string de conexão ao banco de dados
2. **Uso**: Você chama métodos para inserir, atualizar, ler ou excluir dados
3. **Descarte**: Você descarta o repositório usando `Dispose()` ou o padrão `using`

```csharp
// Exemplo de uso completo do repositório
string connectionString = $"Data Source={App.DbPath}";

// O 'using' garante que o repositório será fechado corretamente
using (var repository = new DiarioSaudeRepository(connectionString))
{
    // Agora você pode usar o repositório para acessar os dados
    var humores = await repository.ObterHumoresAsync();
    
    // Quando o bloco 'using' terminar, o repositório é automaticamente liberado
}
```

## Guia Passo a Passo de Operações Comuns

### 1. Criando um novo registro diário

Para registrar um novo dia no diário de saúde:

```csharp
// 1. Crie o repositório
using var repository = new DiarioSaudeRepository($"Data Source={App.DbPath}");

// 2. Crie uma nova entrada de alimentação
var alimentacao = new Alimentacao { Descricao = "Café da manhã saudável com frutas" };
int alimentacaoId = await repository.AdicionarAlimentacaoAsync(alimentacao);

// 3. Crie uma nova entrada de atividade física
var atividade = new AtividadeFisica { Descricao = "Caminhada de 30 minutos" };
int atividadeId = await repository.AdicionarAtividadeFisicaAsync(atividade);

// 4. Crie o registro diário completo
// (HumorId e SonoId são IDs existentes das tabelas Humor e QualidadeSono)
var registro = new RegistroDiario
{
        Data = DateTime.Now,  // Data atual
        HumorId = 1,          // ID 1 = "Feliz" (se seguir os dados padrão)
        SonoId = 1,           // ID 1 = "Boa" (se seguir os dados padrão)
        AlimentacaoId = alimentacaoId,
        AtividadeFisicaId = atividadeId
};

// 5. Salve o registro no banco de dados
int registroId = await repository.AdicionarRegistroDiarioAsync(registro);

// Pronto! O ID retornado é o identificador único do novo registro
Console.WriteLine($"Registro criado com ID: {registroId}");
```

### 2. Consultando registros por período

Para ver todos os registros da última semana:

```csharp
using var repository = new DiarioSaudeRepository($"Data Source={App.DbPath}");

// Define o período de 7 dias atrás até hoje
DateTime inicio = DateTime.Now.AddDays(-7);
DateTime fim = DateTime.Now;

// Obtém todos os registros no período
var registrosSemana = await repository.ObterRegistrosPorPeriodoAsync(inicio, fim);

// Exibe os resultados
foreach (var registro in registrosSemana)
{
    Console.WriteLine($"Registro do dia {registro.Data.ToShortDateString()}, Humor ID: {registro.HumorId}");
}
```

### 3. Atualizando um registro existente

Para modificar um registro que já existe:

```csharp
using var repository = new DiarioSaudeRepository($"Data Source={App.DbPath}");

// Obtém o registro pelo ID
int registroId = 1; // ID do registro que você quer atualizar
var registro = await repository.ObterRegistroDiarioAsync(registroId);

if (registro != null)
{
    // Modifica os valores
    registro.HumorId = 2; // Mudou para ID 2 = "Triste" (se seguir os dados padrão)
    
    // Salva as alterações
    await repository.AtualizarRegistroDiarioAsync(registro);
    Console.WriteLine("Registro atualizado com sucesso!");
}
else
{
    Console.WriteLine("Registro não encontrado!");
}
```

### 4. Excluindo um registro

Para remover um registro do banco de dados:

```csharp
using var repository = new DiarioSaudeRepository($"Data Source={App.DbPath}");

// ID do registro que você quer excluir
int registroId = 1;

// Exclui o registro
await repository.DeletarRegistroDiarioAsync(registroId);
Console.WriteLine("Registro excluído com sucesso!");
```

## Consultando dados de referência

Para obter as listas de opções disponíveis:

```csharp
using var repository = new DiarioSaudeRepository($"Data Source={App.DbPath}");

// Lista de humores disponíveis
var humores = await repository.ObterHumoresAsync();
foreach (var humor in humores)
{
    Console.WriteLine($"ID: {humor.Id}, Descrição: {humor.Descricao}");
}

// Lista de qualidades de sono
var qualidadesSono = await repository.ObterQualidadesSonoAsync();
foreach (var sono in qualidadesSono)
{
    Console.WriteLine($"ID: {sono.Id}, Descrição: {sono.Descricao}");
}
```

## Como o banco de dados funciona internamente

Internamente, o sistema usa uma biblioteca chamada LINQ to DB para mapear as classes C# para tabelas do SQLite. Isso significa que você não precisa escrever comandos SQL diretamente - a biblioteca converte suas operações em C# para operações de banco de dados.

A classe `DiarioSaudeDb` define a estrutura do banco de dados:

```csharp
public class DiarioSaudeDb : DataConnection
{
    // Construtor que recebe a string de conexão
    public DiarioSaudeDb(string connectionString) 
        : base(ProviderName.SQLite, connectionString)
    {
    }

    // Propriedades que representam as tabelas no banco de dados
    public ITable<RegistroDiario> RegistrosDiarios => this.GetTable<RegistroDiario>();
    public ITable<Humor> Humores => this.GetTable<Humor>();
    public ITable<QualidadeSono> QualidadesSono => this.GetTable<QualidadeSono>();
    public ITable<Alimentacao> Alimentacoes => this.GetTable<Alimentacao>();
    public ITable<AtividadeFisica> AtividadesFisicas => this.GetTable<AtividadeFisica>();
    public ITable<Configuracao> Configuracoes => this.GetTable<Configuracao>();
}
```

Quando você chama métodos do repositório, por baixo dos panos o sistema está:

1. Abrindo uma conexão com o arquivo SQLite
2. Convertendo sua solicitação em comandos SQL
3. Executando os comandos no banco de dados
4. Convertendo os resultados de volta para objetos C#
5. Retornando esses objetos para você

## Dicas para Solução de Problemas

Se encontrar problemas com o banco de dados:

1. **Verifique se o arquivo existe** no local esperado (`%LocalAppData%\DiarioSaude\diariosaude.db`)
2. **Verifique permissões de acesso** ao diretório e ao arquivo
3. **Tente reiniciar o aplicativo**, que pode recriar o banco de dados se necessário
4. **Verifique se há erros no console** que possam indicar problemas com o banco de dados

## Resumo dos Métodos Disponíveis no Repositório

| Método | Descrição | Exemplo de Uso |
|--------|-----------|----------------|
| `AdicionarRegistroDiarioAsync` | Adiciona um novo registro diário | `await repository.AdicionarRegistroDiarioAsync(registro)` |
| `ObterRegistroDiarioAsync` | Obtém um registro pelo ID | `var registro = await repository.ObterRegistroDiarioAsync(1)` |
| `ObterRegistrosPorPeriodoAsync` | Obtém registros em um período | `var registros = await repository.ObterRegistrosPorPeriodoAsync(inicio, fim)` |
| `AtualizarRegistroDiarioAsync` | Atualiza um registro existente | `await repository.AtualizarRegistroDiarioAsync(registro)` |
| `DeletarRegistroDiarioAsync` | Exclui um registro pelo ID | `await repository.DeletarRegistroDiarioAsync(1)` |
| `ObterHumoresAsync` | Obtém a lista de humores | `var humores = await repository.ObterHumoresAsync()` |
| `ObterQualidadesSonoAsync` | Obtém a lista de qualidades de sono | `var sono = await repository.ObterQualidadesSonoAsync()` |
| `AdicionarAlimentacaoAsync` | Adiciona um registro de alimentação | `await repository.AdicionarAlimentacaoAsync(alimentacao)` |
| `ObterAlimentacaoAsync` | Obtém um registro de alimentação pelo ID | `var alim = await repository.ObterAlimentacaoAsync(1)` |
| `AdicionarAtividadeFisicaAsync` | Adiciona um registro de atividade física | `await repository.AdicionarAtividadeFisicaAsync(atividade)` |
| `ObterAtividadeFisicaAsync` | Obtém um registro de atividade física pelo ID | `var atv = await repository.ObterAtividadeFisicaAsync(1)` |
| `ObterConfiguracaoAsync` | Obtém as configurações do aplicativo | `var config = await repository.ObterConfiguracaoAsync()` |
| `SalvarConfiguracaoAsync` | Salva as configurações do aplicativo | `await repository.SalvarConfiguracaoAsync(config)` |
