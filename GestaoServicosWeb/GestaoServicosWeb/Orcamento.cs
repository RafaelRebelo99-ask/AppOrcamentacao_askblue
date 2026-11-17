using System;
using System.Linq;

namespace GestaoServicosWeb
{
    public enum EstadoOrcamento
    {
        Rascunho,
        Enviado,
        Aceite,
        Recusado,
        Expirado
    }

    /*
     * Rascunho (pode ser editado / eliminado) > Enviado (Já não pode ser editado) > Aceite / Recusado (pelo cliente)
     */

    public class Orcamento
    {
        private Guid _idPrestador;
        private Guid _idServico;
        private decimal _valorSemIVA;
        private decimal _taxaIVA;
        private int _duracaoDias;
        private EstadoOrcamento _estado;

        public Guid Id { get; private set; }

        // FK
        public Guid IdPrestador
        {
            get => _idPrestador;
            private set
            {
                ValidarIdPrestador(value);
                _idPrestador = value;
            }
        }
        public Utilizador Prestador { get; private set; }

        // FK
        public Guid IdServico
        {
            get => _idServico;
            private set
            {
                ValidarIdServico(value);
                _idServico = value;
            }
        }
        public Servico Servico { get; private set; }

        public decimal ValorSemIVA
        {
            get => _valorSemIVA;
            private set
            {
                ValidarValor(value);
                _valorSemIVA = value;
            }
        }

        public decimal TaxaIVA
        {
            get => _taxaIVA;
            private set
            {
                ValidarTaxa(value);
                _taxaIVA = value;
            }
        }

        public DateTime DataValidade { get; private set; }

        public int DuracaoDias
        {
            get => _duracaoDias;
            private set
            {
                ValidarDuracao(value);
                _duracaoDias = value;
            }
        }
        public decimal ValorTotal =>
            Math.Round(ValorSemIVA * (1 + TaxaIVA / 100m), 2);

        public EstadoOrcamento Estado
        {
            get => _estado;
            private set
            {
                if (!Enum.IsDefined(value))
                    throw new ArgumentException("Estado inválido.", nameof(value));
                _estado = value;
            }
        }

        // válido até ao fim do dia da DataValidade
        public bool Expirado =>
            Estado == EstadoOrcamento.Enviado
            && DateTime.UtcNow.Date > DataValidade.Date;

        public Orcamento(Guid idPrestador, Guid idServico, decimal valorSemIVA, decimal taxaIVA,
            DateTime dataValidade, int duracaoDias)
        {
            ValidarValidade(dataValidade);

            Id = Guid.NewGuid();
            IdPrestador = idPrestador;
            IdServico = idServico;
            ValorSemIVA = valorSemIVA;
            TaxaIVA = taxaIVA;
            DataValidade = dataValidade;
            DuracaoDias = duracaoDias;

            Estado = EstadoOrcamento.Rascunho;
        }

        public void Atualizar(decimal valorSemIVA, decimal taxaIVA, DateTime dataValidade, int duracaoDias)
        {
            if (Estado is not (EstadoOrcamento.Rascunho))
                throw new InvalidOperationException($"Um orçamento {Estado} não pode ser editado.");

            ValidarValor(valorSemIVA);
            ValidarTaxa(taxaIVA);
            ValidarValidade(dataValidade);
            ValidarDuracao(duracaoDias);

            ValorSemIVA = valorSemIVA;
            TaxaIVA = taxaIVA;
            DataValidade = dataValidade;
            DuracaoDias = duracaoDias;
        }

        public void Enviar()
        {
            ExigirEstado(EstadoOrcamento.Rascunho);
            Estado = EstadoOrcamento.Enviado;
        }

        public void Aceitar()
        {
            ExigirEstado(EstadoOrcamento.Enviado);
            Estado = EstadoOrcamento.Aceite;
        }

        public void Recusar()
        {
            ExigirEstado(EstadoOrcamento.Enviado);
            Estado = EstadoOrcamento.Recusado;
        }

        private void ExigirEstado(params EstadoOrcamento[] permitidos)
        {
            if (!permitidos.Contains(Estado))
                throw new InvalidOperationException($"Transição inválida a partir de {Estado}.");
        }

        private static void ValidarIdPrestador(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Prestador inválido.", nameof(id));
        }

        private static void ValidarIdServico(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Serviço inválido.", nameof(id));
        }

        private static void ValidarValor(decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentOutOfRangeException(nameof(valor), "O valor tem de ser positivo.");
        }

        private static void ValidarTaxa(decimal taxa)
        {
            if (taxa < 0 || taxa > 100)
                throw new ArgumentOutOfRangeException(nameof(taxa), "Taxa de IVA inválida.");
        }

        private static void ValidarValidade(DateTime data)
        {
            if (data.Date <= DateTime.UtcNow.Date)
                throw new ArgumentOutOfRangeException(nameof(data), "A validade tem de ser no futuro.");
        }

        private static void ValidarDuracao(int dias)
        {
            if (dias <= 0)
                throw new ArgumentOutOfRangeException(nameof(dias), "A duração tem de ser positiva.");
        }
    }
}