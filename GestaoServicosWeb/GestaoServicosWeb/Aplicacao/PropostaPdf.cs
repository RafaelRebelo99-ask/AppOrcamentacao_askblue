using System.Globalization;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;

namespace GestaoServicosWeb.Aplicacao
{
    // O que aparece no documento. Já vem tudo lido da base de dados pelo OrcamentoService.
    public sealed record DadosPropostaPdf(
        string Numero,
        string Estado,
        bool Rascunho,
        bool Expirada,
        string EmpresaNome,
        string EmpresaNif,
        string EmpresaEmail,
        string? EmpresaTelefone,
        string ClienteNome,
        string ClienteEmail,
        string Oportunidade,
        string Categoria,
        string Descricao,
        decimal ValorSemIVA,
        decimal TaxaIVA,
        decimal ValorIVA,
        decimal ValorTotal,
        DateTime Data,
        DateTime DataInicio,
        DateTime DataFim,
        int DuracaoDias,
        DateTime DataValidade
    );

    // ===== Gera o PDF de uma proposta =====
    // Só trata do aspeto do documento: não lê a base de dados nem verifica permissões.
    public static class PropostaPdf
    {
        private static readonly CultureInfo Pt = CultureInfo.GetCultureInfo("pt-PT");
        private static readonly Color Azul = new(2, 10, 237);     // --primary-blue do askblue.css
        private static readonly Color Vermelho = new(150, 0, 0);
        private static readonly Color Cinzento = new(110, 110, 110);
        private static readonly Color CinzentoClaro = new(220, 220, 220);

        static PropostaPdf()
        {
            // Usa o Arial instalado no Windows. Num servidor Linux é preciso configurar
            // um FontResolver com a fonte incluída no projeto.
            if (OperatingSystem.IsWindows())
                GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        }

        public static byte[] Gerar(DadosPropostaPdf d)
        {
            var documento = new Document();
            documento.Info.Title = $"Proposta {d.Numero}";
            documento.Info.Author = d.EmpresaNome;

            var normal = documento.Styles[StyleNames.Normal]!;
            normal.Font.Name = "Arial";
            normal.Font.Size = 10;

            var pagina = documento.AddSection();
            pagina.PageSetup = documento.DefaultPageSetup.Clone();
            pagina.PageSetup.PageFormat = PageFormat.A4;
            pagina.PageSetup.LeftMargin = "2cm";
            pagina.PageSetup.RightMargin = "2cm";
            pagina.PageSetup.TopMargin = "2cm";
            pagina.PageSetup.BottomMargin = "2.5cm";

            Cabecalho(pagina, d);
            Titulo(pagina, d);
            Partes(pagina, d);
            Servico(pagina, d);
            Valores(pagina, d);
            Prazos(pagina, d);
            Rodape(pagina, d);

            var renderer = new PdfDocumentRenderer { Document = documento };
            renderer.RenderDocument();

            using var memoria = new MemoryStream();
            renderer.PdfDocument.Save(memoria, false);
            return memoria.ToArray();
        }

        // Logo à esquerda, dados da AskBlue à direita
        private static void Cabecalho(Section pagina, DadosPropostaPdf d)
        {
            var tabela = pagina.AddTable();
            tabela.AddColumn("8.5cm");
            tabela.AddColumn("8.5cm");

            var linha = tabela.AddRow();

            var logo = linha.Cells[0].AddParagraph().AddImage(LogoEmBase64());
            logo.Width = "4.5cm";
            logo.LockAspectRatio = true;

            var empresa = linha.Cells[1].AddParagraph();
            empresa.Format.Alignment = ParagraphAlignment.Right;
            empresa.Format.Font.Color = Cinzento;
            empresa.AddFormattedText(d.EmpresaNome, TextFormat.Bold);
            empresa.AddLineBreak();
            empresa.AddText($"NIF: {d.EmpresaNif}");
            empresa.AddLineBreak();
            empresa.AddText(d.EmpresaEmail);

            if (!string.IsNullOrWhiteSpace(d.EmpresaTelefone))
            {
                empresa.AddLineBreak();
                empresa.AddText(d.EmpresaTelefone);
            }
        }

        private static void Titulo(Section pagina, DadosPropostaPdf d)
        {
            var titulo = pagina.AddParagraph($"Proposta {d.Numero}");
            titulo.Format.SpaceBefore = "1cm";
            titulo.Format.Font.Size = 20;
            titulo.Format.Font.Bold = true;
            titulo.Format.Font.Color = Azul;

            var info = pagina.AddParagraph($"Data: {Data(d.Data)}   |   Estado: {d.Estado}");
            info.Format.SpaceBefore = "0.1cm";
            info.Format.Font.Color = Cinzento;

            if (d.Rascunho)
                Destaque(pagina, "RASCUNHO: documento interno, ainda não enviado ao cliente.");
            else if (d.Expirada)
                Destaque(pagina, "Esta proposta já expirou.");
        }

        private static void Partes(Section pagina, DadosPropostaPdf d)
        {
            Subtitulo(pagina, "Cliente");
            pagina.AddParagraph(d.ClienteNome).Format.Font.Bold = true;
            pagina.AddParagraph(d.ClienteEmail);
        }

        private static void Servico(Section pagina, DadosPropostaPdf d)
        {
            Subtitulo(pagina, "Serviço");
            pagina.AddParagraph(PartirPalavrasLongas(d.Oportunidade)).Format.Font.Bold = true;
            pagina.AddParagraph(d.Categoria).Format.Font.Color = Cinzento;

            // Mantém as mudanças de linha escritas no formulário
            foreach (var linha in d.Descricao.ReplaceLineEndings("\n").Split('\n'))
            {
                var paragrafo = pagina.AddParagraph();
                paragrafo.Format.SpaceBefore = "0.2cm";
                paragrafo.AddText(PartirPalavrasLongas(linha));
            }
        }

        // O MigraDoc só muda de linha nos espaços: um link muito comprido saía cortado na margem
        private static string PartirPalavrasLongas(string texto)
        {
            const int maximo = 60;

            return string.Join(' ', texto.Split(' ').Select(palavra =>
                palavra.Length <= maximo
                    ? palavra
                    : string.Join(' ', palavra.Chunk(maximo).Select(parte => new string(parte)))));
        }

        private static void Valores(Section pagina, DadosPropostaPdf d)
        {
            Subtitulo(pagina, "Valores");

            var tabela = pagina.AddTable();
            tabela.Borders.Bottom.Color = CinzentoClaro;
            tabela.Borders.Bottom.Width = 0.5;
            tabela.AddColumn("12cm");
            tabela.AddColumn("5cm").Format.Alignment = ParagraphAlignment.Right;

            LinhaValor(tabela, "Valor sem IVA", d.ValorSemIVA);
            LinhaValor(tabela, $"IVA ({d.TaxaIVA.ToString("0.##", Pt)}%)", d.ValorIVA);

            var total = LinhaValor(tabela, "Total (incl. IVA)", d.ValorTotal);
            total.Format.Font.Bold = true;
            total.Format.Font.Size = 12;

            tabela.Rows[0].KeepWith = 2; // as três linhas ficam na mesma página
        }

        private static Row LinhaValor(Table tabela, string texto, decimal valor)
        {
            var linha = tabela.AddRow();
            linha.TopPadding = "0.12cm";
            linha.BottomPadding = "0.12cm";
            linha.Cells[0].AddParagraph(texto);
            linha.Cells[1].AddParagraph(valor.ToString("C2", Pt));
            return linha;
        }

        private static void Prazos(Section pagina, DadosPropostaPdf d)
        {
            Subtitulo(pagina, "Prazos");

            var dias = d.DuracaoDias == 1 ? "1 dia" : $"{d.DuracaoDias} dias";
            pagina.AddParagraph($"Execução: de {Data(d.DataInicio)} a {Data(d.DataFim)} ({dias})");

            var validade = pagina.AddParagraph($"Proposta válida até {Data(d.DataValidade)}");
            validade.Format.SpaceBefore = "0.1cm";
            validade.Format.Font.Bold = true;
        }

        private static void Rodape(Section pagina, DadosPropostaPdf d)
        {
            var rodape = pagina.Footers.Primary.AddParagraph();
            rodape.Format.Alignment = ParagraphAlignment.Center;
            rodape.Format.Font.Size = 8;
            rodape.Format.Font.Color = Cinzento;
            rodape.AddText($"{d.EmpresaNome} - Proposta {d.Numero} - Página ");
            rodape.AddPageField();
            rodape.AddText(" de ");
            rodape.AddNumPagesField();
        }

        private static void Subtitulo(Section pagina, string texto)
        {
            var p = pagina.AddParagraph(texto);
            p.Format.SpaceBefore = "0.8cm";
            p.Format.SpaceAfter = "0.2cm";
            p.Format.KeepWithNext = true;
            p.Format.Font.Size = 12;
            p.Format.Font.Bold = true;
            p.Format.Font.Color = Azul;
        }

        private static void Destaque(Section pagina, string texto)
        {
            var p = pagina.AddParagraph(texto);
            p.Format.SpaceBefore = "0.3cm";
            p.Format.Font.Bold = true;
            p.Format.Font.Color = Vermelho;
        }

        private static string Data(DateTime data) => data.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        // O logo vai dentro da própria aplicação (EmbeddedResource no .csproj)
        private static string LogoEmBase64()
        {
            using var stream = typeof(PropostaPdf).Assembly
                .GetManifestResourceStream("askblue-logo.png")
                ?? throw new FileNotFoundException("Falta o logo askblue-logo.png nos recursos da aplicação.");

            using var memoria = new MemoryStream();
            stream.CopyTo(memoria);
            return "base64:" + Convert.ToBase64String(memoria.ToArray());
        }
    }
}
