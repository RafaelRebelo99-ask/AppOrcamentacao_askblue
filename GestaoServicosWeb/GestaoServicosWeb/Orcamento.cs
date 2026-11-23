using System;

namespace GestaoServicosWeb
{
    public enum EstadoOrcamento
    {
        Rascunho,
        Enviado,
        Aceite,
        Recusado
    }

    // representa uma proposta enviada por um trabalhador 

    public class Orcamento
    {
        public Guid Id { get; private set; }
        public Guid IdOportunidade { get; private set; }
        public Oportunidade? Oportunidade { get; private set; }

        public Guid IdCriador { get; private set; }
        public Utilizador? Criador { get; private set; }

        public decimal ValorSemIVA { get; private set; }

        public decimal TaxaIVA { get; private set; }

        public DateTime DataCriacao { get; private set; }
        public DateTime? DataEnvio { get; private set; }
        public DateTime? DataDecisao { get; private set; }

        public DateTime DataValidade { get; private set; }
        public DateTime DataInicio { get; private set; }

        public int DuracaoDias { get; private set; }

        public DateTime DataFim => DataInicio.AddDays(DuracaoDias);

        public string Descricao { get; private set; } = string.Empty;

        public decimal ValorTotal =>
            Math.Round(
                ValorSemIVA * (1 + TaxaIVA / 100m),
                2,
                MidpointRounding.AwayFromZero);

        public decimal ValorIVA => ValorTotal - ValorSemIVA;

        public EstadoOrcamento Estado { get; private set; }

        public bool Expirado =>
            Estado == EstadoOrcamento.Enviado &&
            DateTime.UtcNow.Date > DataValidade.Date;

        private Orcamento() { }

        public Orcamento(
            Guid idOportunidade,
            Guid idCriador,
            string descricao,
            decimal valorSemIVA,
            decimal taxaIVA,
            DateTime dataValidade,
            DateTime dataInicio,
            int duracaoDias)
        {
            ValidarIds(idOportunidade, idCriador);
            ValidarDetalhes(descricao, valorSemIVA, taxaIVA, duracaoDias);
            ValidarDatas(dataValidade, dataInicio);

            Id = Guid.NewGuid();

            IdOportunidade = idOportunidade;
            IdCriador = idCriador;

            Descricao = descricao.Trim();
            ValorSemIVA = valorSemIVA;
            TaxaIVA = taxaIVA;
            DataValidade = dataValidade;
            DataInicio = dataInicio;
            DuracaoDias = duracaoDias;

            DataCriacao = DateTime.UtcNow;
            Estado = EstadoOrcamento.Rascunho;
        }

        public void Atualizar(
            string descricao,
            decimal valorSemIVA,
            decimal taxaIVA,
            DateTime dataValidade,
            DateTime dataInicio,
            int duracaoDias)
        {
            ExigirEstado(EstadoOrcamento.Rascunho);

            ValidarDetalhes(descricao, valorSemIVA, taxaIVA, duracaoDias);
            ValidarDatas(dataValidade, dataInicio);

            Descricao = descricao.Trim();
            ValorSemIVA = valorSemIVA;
            TaxaIVA = taxaIVA;
            DataValidade = dataValidade;
            DataInicio = dataInicio;
            DuracaoDias = duracaoDias;
        }

        public void Enviar()
        {
            ExigirEstado(EstadoOrcamento.Rascunho);

            if (DataValidade.Date <= DateTime.UtcNow.Date)
                throw new InvalidOperationException(
                    "A validade tem de ser uma data futura.");

            if (DataInicio.Date < DateTime.UtcNow.Date)
                throw new InvalidOperationException(
                    "A data de início não pode estar no passado.");

            DataEnvio = DateTime.UtcNow;
            Estado = EstadoOrcamento.Enviado;
        }

        public void Aceitar()
        {
            ExigirEstado(EstadoOrcamento.Enviado);
            GarantirNaoExpirado();

            DataDecisao = DateTime.UtcNow;
            Estado = EstadoOrcamento.Aceite;
        }

        public void Recusar()
        {
            ExigirEstado(EstadoOrcamento.Enviado);
            GarantirNaoExpirado();

            DataDecisao = DateTime.UtcNow;
            Estado = EstadoOrcamento.Recusado;
        }

        public void RecusarPorAceitacaoDeOutraProposta()
        {
            ExigirEstado(EstadoOrcamento.Enviado);

            DataDecisao = DateTime.UtcNow;
            Estado = EstadoOrcamento.Recusado;
        }

        private void GarantirNaoExpirado()
        {
            if (Expirado)
                throw new InvalidOperationException(
                    "Este orçamento já expirou.");
        }

        private void ExigirEstado(EstadoOrcamento estadoPermitido)
        {
            if (Estado != estadoPermitido)
                throw new InvalidOperationException(
                    $"Não é possível realizar esta operação quando o orçamento está no estado {Estado}.");
        }

        private static void ValidarIds(
            Guid idOportunidade,
            Guid idCriador)
        {
            if (idOportunidade == Guid.Empty)
                throw new ArgumentException("A oportunidade é obrigatória.");

            if (idCriador == Guid.Empty)
                throw new ArgumentException("O criador é obrigatório.");
        }

        private static void ValidarDetalhes(
            string descricao,
            decimal valorSemIVA,
            decimal taxaIVA,
            int duracaoDias)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException(
                    "A descrição do orçamento é obrigatória.");

            if (valorSemIVA <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(valorSemIVA),
                    "O valor tem de ser positivo.");

            if (taxaIVA < 0 || taxaIVA > 100)
                throw new ArgumentOutOfRangeException(
                    nameof(taxaIVA),
                    "Taxa de IVA inválida.");

            if (duracaoDias <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(duracaoDias),
                    "A duração tem de ser positiva.");
        }

        private static void ValidarDatas(
            DateTime dataValidade,
            DateTime dataInicio)
        {
            if (dataValidade == default)
                throw new ArgumentException(
                    "A data de validade é obrigatória.");

            if (dataInicio == default)
                throw new ArgumentException(
                    "A data de início é obrigatória.");
        }
    }
}
