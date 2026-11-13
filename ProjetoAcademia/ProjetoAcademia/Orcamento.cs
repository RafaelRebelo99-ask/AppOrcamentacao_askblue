namespace ProjetoAcademia
{
    enum EstadoOrcamento
    {
        Aceite,
        Recusado, 
        Enviado,
        Rascunho
    }

    class Orcamento
    {
        public Guid Id { get; private set; }

        // FK
        public Guid IdPrestador { get; 
            private set 
            {
                if (IdPrestador == Guid.Empty)
                    throw new ArgumentException("Prestador inválido.", nameof(IdPrestador));
            } 
        }
        public Utilizador Prestador { get; private set; }

        // FK
        public Guid IdServico { get; 
            private set 
            {
                if (IdServico == Guid.Empty)
                    throw new ArgumentException("Serviço inválido.", nameof(IdServico));
            } 
        }
        public Servico Servico { get; private set; }

        public decimal ValorSemIVA { get; 
            private set
            {
                if (ValorSemIVA <= 0)
                    throw new ArgumentOutOfRangeException(nameof(ValorSemIVA), "O valor tem de ser positivo.");
            }
        }
        public decimal TaxaIVA { get; 
            private set
            {
                if (TaxaIVA < 0 || TaxaIVA > 100)
                    throw new ArgumentOutOfRangeException(nameof(TaxaIVA), "Taxa de IVA inválida.");
            }
        }
        public DateTime DataValidade { get; 
            private set 
            {
                if (DataValidade.Date <= DateTime.Now.Date)
                    throw new ArgumentOutOfRangeException(nameof(DataValidade), "A validade tem de ser no futuro.");
            } 
        }
        public int DuracaoDias { get; 
            private set
            {
                if (DuracaoDias <= 0)
                    throw new ArgumentOutOfRangeException(nameof(DuracaoDias), "A duração tem de ser positiva.");
            }
        }

        private decimal ValorTotal
        {
            get
            {
                return Math.Round(ValorSemIVA * (1 + TaxaIVA / 100m), 2);
            }
        }

        public Orcamento(Guid idPrestador, Guid idServico, decimal valorSemIVA, decimal taxaIVA,
            DateTime dataValidade, int duracaoDias)
        {
            this.Id = Guid.NewGuid();
            this.IdPrestador = idPrestador;
            this.IdServico = idServico;
            this.ValorSemIVA = valorSemIVA;
            this.TaxaIVA = taxaIVA;
            this.DataValidade = dataValidade;
            this.DuracaoDias = duracaoDias;

        }
    }

    /*
     Request (POST) 
    - campos preenchidos pelo prestador -> ValorSemIVA, TaxaIVA, DuracaoDias, DataValidade

    Response 
    - ID, IdServico, ValorSemIVA, TaxaIVA, DuracaoDias, DataValidade, ValorTotal
     */
}
