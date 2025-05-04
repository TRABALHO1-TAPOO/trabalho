using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<RegistroDiario>> ObterRegistrosDiariosAsync()
        {
            return await Task.Run(() => _db.RegistrosDiarios.ToList());
        }

        public async Task<List<Humor>> ObterHumoresAsync()
        {
            return await Task.Run(() => _db.Humores.ToList());
        }

        public async Task<List<QualidadeSono>> ObterQualidadesSonoAsync()
        {
            return await Task.Run(() => _db.QualidadesSono.ToList());
        }

        public async Task<List<Alimentacao>> ObterAlimentacoesAsync()
        {
            return await Task.Run(() => _db.Alimentacoes.ToList());
        }

        public async Task<List<AtividadeFisica>> ObterAtividadesFisicasAsync()
        {
            return await Task.Run(() => _db.AtividadesFisicas.ToList());
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}