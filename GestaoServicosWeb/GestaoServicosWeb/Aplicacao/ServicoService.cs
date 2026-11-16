using GestaoServicosWeb.Contratos;

namespace GestaoServicosWeb.Aplicacao
{
    public class ServicoService
    {
        // Armazenamento temporário em memória: não é base de dados definitiva.
        private readonly List<Servico> servicos = new();

        // O lock evita que múltiplos pedidos em simultâneo alterem a lista ao mesmo tempo
        private readonly object bloqueio = new();

        public List<Servico> Listar(EstadoServico? estado = null, Guid? idCategoria = null)
        {
            lock (bloqueio)
            {
                var query = servicos.AsEnumerable();

                if (estado.HasValue)
                    query = query.Where(s => s.Estado == estado.Value);

                if (idCategoria.HasValue && idCategoria.Value != Guid.Empty)
                    query = query.Where(s => s.IdCategoria == idCategoria.Value);

                return query.ToList();
            }
        }

        public Servico? Buscar(Guid id)
        {
            lock (bloqueio)
            {
                return servicos.FirstOrDefault(s => s.Id == id);
            }
        }

        public List<Servico> ListarPorCliente(Guid clienteId)
        {
            lock (bloqueio)
            {
                return servicos.Where(s => s.IdCliente == clienteId).ToList();
            }
        }

        public Servico Criar(CriarServicoDto dto)
        {
            // O construtor do Servico valida datas no futuro, strings vazias e Guids válidos
            var servico = new Servico(
                dto.IdCliente,
                dto.IdCategoria,
                dto.Titulo,
                dto.Descricao,
                dto.PrazoPropostas,
                dto.DataDesejada
            );

            lock (bloqueio)
            {
                servicos.Add(servico);
                return servico;
            }
        }

        // Devolve false se o serviço não existir
        public bool Cancelar(Guid id)
        {
            lock (bloqueio)
            {
                var servico = servicos.FirstOrDefault(s => s.Id == id);
                if (servico == null)
                    return false;

                servico.Cancelar();
                return true;
            }
        }
    }
}
