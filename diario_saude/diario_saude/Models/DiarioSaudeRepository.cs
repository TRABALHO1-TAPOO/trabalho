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

        public async Task<int> AdicionarRegistroDiarioAsync(RegistroDiario registro)
        {
            return await _db.InsertWithInt32IdentityAsync(registro);
        }

        public async Task<RegistroDiario?> ObterRegistroDiarioAsync(int id)
        {
            return await _db.RegistrosDiarios.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AtualizarRegistroDiarioAsync(RegistroDiario registro)
        {
            await _db.UpdateAsync(registro);
        }

        public async Task DeletarRegistroDiarioAsync(int id)
        {
            await _db.RegistrosDiarios.DeleteAsync(r => r.Id == id);
        }

        public async Task<List<Humor>> ObterHumoresAsync()
        {
            return await _db.Humores.ToListAsync();
        }

        public async Task<List<QualidadeSono>> ObterQualidadesSonoAsync()
        {
            return await _db.QualidadesSono.ToListAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}