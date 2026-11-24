namespace GestaoServicosWeb
{
    public enum EstadoOportunidade
    {
        Registada,
        EmAnalise,
        EmPreparacao,
        PropostaEnviada,
        Ganha,
        Perdida,
        Cancelada
    }
    // serviço -> oportunidade

        public class Oportunidade
        {
            public Guid Id { get; private set; }

            // Cliente
            public Guid IdCliente { get; private set; }
            internal Utilizador? Cliente { get; private set; }

            // Categoria
            public Guid IdCategoria { get; private set; }
            internal Categoria? Categoria { get; private set; }

            // O trabalhador responsável por registar a oportunidade
            public Guid IdResponsavel { get; private set; }
            internal Utilizador? Responsavel { get; private set; }

            public string Titulo { get; private set; } = string.Empty;
            public string Descricao { get; private set; } = string.Empty;

            public EstadoOportunidade Estado { get; private set; }

            public DateTime DataCriacao { get; private set; }
            public DateTime? PrazoProposta { get; private set; }
            public DateTime? DataPretendida { get; private set; }

            private Oportunidade() { }

            public Oportunidade(
                Guid idCliente,
                Guid idCategoria,
                Guid idResponsavel,
                string titulo,
                string descricao,
                DateTime? prazoProposta = null,
                DateTime? dataPretendida = null)
            {
                ValidarIds(idCliente, idCategoria, idResponsavel);
                ValidarDetalhes(titulo, descricao);
                ValidarDatas(prazoProposta, dataPretendida);

                Id = Guid.NewGuid();

                IdCliente = idCliente;
                IdCategoria = idCategoria;
                IdResponsavel = idResponsavel;

                Titulo = titulo.Trim();
                Descricao = descricao.Trim();

                PrazoProposta = prazoProposta;
                DataPretendida = dataPretendida;

                DataCriacao = DateTime.UtcNow;
                Estado = EstadoOportunidade.Registada;
            }

            public void Atualizar(
                Guid idCategoria,
                Guid idResponsavel,
                string titulo,
                string descricao,
                DateTime? prazoProposta,
                DateTime? dataPretendida)
            {
                if (Estado is EstadoOportunidade.PropostaEnviada
                    or EstadoOportunidade.Ganha
                    or EstadoOportunidade.Perdida
                    or EstadoOportunidade.Cancelada)
                {
                    throw new InvalidOperationException(
                        "Não é possível editar uma oportunidade neste estado.");
                }

                if (idCategoria == Guid.Empty)
                    throw new ArgumentException("A categoria é obrigatória.");

                if (idResponsavel == Guid.Empty)
                    throw new ArgumentException("O responsável é obrigatório.");

                ValidarDetalhes(titulo, descricao);
                ValidarDatas(prazoProposta, dataPretendida);

                IdCategoria = idCategoria;
                IdResponsavel = idResponsavel;
                Titulo = titulo.Trim();
                Descricao = descricao.Trim();
                PrazoProposta = prazoProposta;
                DataPretendida = dataPretendida;
            }

            public void IniciarAnalise()
            {
                if (Estado != EstadoOportunidade.Registada)
                    throw new InvalidOperationException(
                        "Apenas oportunidades registadas podem passar para análise.");

                Estado = EstadoOportunidade.EmAnalise;
            }

            public void IniciarPreparacao()
            {
                if (Estado is not (
                    EstadoOportunidade.Registada or
                    EstadoOportunidade.EmAnalise))
                {
                    throw new InvalidOperationException(
                        "A oportunidade não pode passar para preparação neste estado.");
                }

                Estado = EstadoOportunidade.EmPreparacao;
            }

        public void RegistarPropostaEnviada()
        {
            if (Estado is not (
                EstadoOportunidade.EmPreparacao or
                EstadoOportunidade.PropostaEnviada))
            {
                throw new InvalidOperationException(
                    "A oportunidade não pode receber propostas neste estado.");
            }

            Estado = EstadoOportunidade.PropostaEnviada;
        }

        public void MarcarComoGanha()
            {
                if (Estado != EstadoOportunidade.PropostaEnviada)
                    throw new InvalidOperationException(
                        "Apenas uma oportunidade com proposta enviada pode ser ganha.");

                Estado = EstadoOportunidade.Ganha;
            }

            public void MarcarComoPerdida()
            {
                if (Estado is EstadoOportunidade.Ganha
                    or EstadoOportunidade.Perdida
                    or EstadoOportunidade.Cancelada)
                {
                    throw new InvalidOperationException(
                        "A oportunidade não pode ser marcada como perdida neste estado.");
                }

                Estado = EstadoOportunidade.Perdida;
            }

            public void Cancelar()
            {
                if (Estado is EstadoOportunidade.Ganha
                    or EstadoOportunidade.Perdida
                    or EstadoOportunidade.Cancelada)
                {
                    throw new InvalidOperationException(
                        "A oportunidade não pode ser cancelada neste estado.");
                }

                Estado = EstadoOportunidade.Cancelada;
            }

            private static void ValidarIds(
                Guid idCliente,
                Guid idCategoria,
                Guid idResponsavel)
            {
                if (idCliente == Guid.Empty)
                    throw new ArgumentException("O cliente é obrigatório.");

                if (idCategoria == Guid.Empty)
                    throw new ArgumentException("A categoria é obrigatória.");

                if (idResponsavel == Guid.Empty)
                    throw new ArgumentException("O responsável é obrigatório.");
            }

            private static void ValidarDetalhes(string titulo, string descricao)
            {
                if (string.IsNullOrWhiteSpace(titulo))
                    throw new ArgumentException("O título não pode estar vazio.");

                if (titulo.Trim().Length > 200)
                    throw new ArgumentException("O título não pode exceder 200 caracteres.");

                if (string.IsNullOrWhiteSpace(descricao))
                    throw new ArgumentException("A descrição não pode estar vazia.");
            }

            private static void ValidarDatas(
                DateTime? prazoProposta,
                DateTime? dataPretendida)
            {
                if (prazoProposta.HasValue &&
                    prazoProposta.Value <= DateTime.UtcNow)
                {
                    throw new ArgumentException(
                        "O prazo da proposta deve ser uma data futura.");
                }

                if (prazoProposta.HasValue &&
                    dataPretendida.HasValue &&
                    dataPretendida.Value < prazoProposta.Value)
                {
                    throw new ArgumentException(
                        "A data pretendida deve ser igual ou posterior ao prazo da proposta.");
                }
            }
        }
    }

