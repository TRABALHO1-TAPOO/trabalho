using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToDB;

namespace DiarioSaude.Models
{
    public class DiarioSaudeRepository : IDisposable
    {
        private readonly DiarioSaudeDb _db;

        public DiarioSaudeRepository(string connectionString)
        {
            _db = new DiarioSaudeDb(connectionString);
        }

        // RegistroDiario operations
        public async Task<int> AdicionarRegistroDiarioAsync(RegistroDiario registro)
        {
            return await _db.InsertWithInt32IdentityAsync(registro);
        }

        public async Task<RegistroDiario> ObterRegistroDiarioAsync(int id)
        {
            return await _db.RegistrosDiarios.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<RegistroDiario>> ObterRegistrosPorPeriodoAsync(DateTime inicio, DateTime fim)
        {
            return await _db.RegistrosDiarios
                .Where(r => r.Data >= inicio && r.Data <= fim)
                .OrderByDescending(r => r.Data)
                .ToListAsync();
        }

        public async Task AtualizarRegistroDiarioAsync(RegistroDiario registro)
        {
            await _db.UpdateAsync(registro);
        }

        public async Task DeletarRegistroDiarioAsync(int id)
        {
            await _db.RegistrosDiarios.DeleteAsync(r => r.Id == id);
        }

        // Humor operations
        public async Task<List<Humor>> ObterHumoresAsync()
        {
            return await _db.Humores.ToListAsync();
        }

        // QualidadeSono operations
        public async Task<List<QualidadeSono>> ObterQualidadesSonoAsync()
        {
            return await _db.QualidadesSono.ToListAsync();
        }

        // Alimentacao operations
        public async Task<int> AdicionarAlimentacaoAsync(Alimentacao alimentacao)
        {
            return await _db.InsertWithInt32IdentityAsync(alimentacao);
        }

        public async Task<Alimentacao> ObterAlimentacaoAsync(int id)
        {
            return await _db.Alimentacoes.FirstOrDefaultAsync(a => a.Id == id);
        }

        // AtividadeFisica operations
        public async Task<int> AdicionarAtividadeFisicaAsync(AtividadeFisica atividade)
        {
            return await _db.InsertWithInt32IdentityAsync(atividade);
        }

        public async Task<AtividadeFisica> ObterAtividadeFisicaAsync(int id)
        {
            return await _db.AtividadesFisicas.FirstOrDefaultAsync(a => a.Id == id);
        }

        // Configuracao operations
        public async Task<Configuracao> ObterConfiguracaoAsync()
        {
            return await _db.Configuracoes.FirstOrDefaultAsync();
        }

        public async Task SalvarConfiguracaoAsync(Configuracao config)
        {
            if (await _db.Configuracoes.AnyAsync())
                await _db.UpdateAsync(config);
            else
                await _db.InsertAsync(config);
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
} 