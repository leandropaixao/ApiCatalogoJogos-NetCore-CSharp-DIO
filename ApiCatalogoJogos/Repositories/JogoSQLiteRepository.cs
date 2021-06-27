using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ApiCatalogoJogos.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ApiCatalogoJogos.Repositories
{
    
    public class ApiContext : DbContext
    {
        public DbSet<Jogo> Jogo { get; set; }
        private readonly IConfiguration _config;

        public ApiContext(IConfiguration config)
        {
            _config = config;
        }

        // The following configures EF to create a Sqlite database file as `C:\blogging.db`.
        // For Mac or Linux, change this to `/tmp/blogging.db` or any other absolute path.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(_config.GetConnectionString("SqLite"));
    }
    

    public class JogoSQLiteRepository : IJogoRepository
    {
        private readonly ApiContext _apiContext;

        public JogoSQLiteRepository(IConfiguration configuration)
        {
            _apiContext = new ApiContext(configuration);
        }

        public async Task Atualizar(Jogo jogo)
        {
            var entidadeJogo = _apiContext.Jogo.Find(jogo.Id);

            entidadeJogo.Nome = jogo.Nome;
            entidadeJogo.Produtora = jogo.Produtora;
            entidadeJogo.Preco = jogo.Preco;

            _ = await _apiContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _apiContext?.Dispose();
        }

        public async Task Inserir(Jogo jogo)
        {
            _apiContext.Add(jogo);
            _ = await _apiContext.SaveChangesAsync();
        }

        public async Task<List<Jogo>> Obter(int pagina, int quantidade)
        {
            return await _apiContext.Jogo.ToListAsync();
        }

        public async Task<Jogo> Obter(Guid id)
        {
            var jogo = await _apiContext.Jogo.FindAsync(id);
            return jogo;
        }

        public async Task<List<Jogo>> Obter(string nome, string produtora)
        {
            return await _apiContext.Jogo.Where(j => j.Nome == nome && j.Produtora == produtora).ToListAsync();
        }

        public async Task Remover(Guid id)
        {
            var jogo = await _apiContext.Jogo.FindAsync(id);
            _apiContext.Remove(jogo);
            _ = await _apiContext.SaveChangesAsync();
        }
    }
}
