using System;
using PTLRuntime.NETScript;
using System.Drawing;
using PTLRuntime.NETScript.Indicators;
using System.Linq;
using System.Collections.Generic;

namespace Ciclo_TFMF
{
    // Define a classe Ciclo_TFMF que herda de NETIndicator
    public class Ciclo_TFMF : NETIndicator
    {
        // Construtor da classe
        public Ciclo_TFMF()
            : base()
        {
            #region Initialization

            // Inicialização dos metadados do indicador
            base.Author = "Wlymarvin Oliveira";
            base.Comments = "";
            base.Company = "";
            base.Copyrights = "";
            base.DateOfCreation = "20/06/2018";
            base.ExpirationDate = 0;
            base.Version = "1.0e";
            base.Password = "";
            base.ProjectName = "Ciclo_TFMF";

            #endregion

            // Configuração das linhas do indicador
            SetIndicatorLine("Linha De Compra", Color.Gold, 3, LineStyle.SimpleChart);
            SetIndicatorLine("Linha de Venda", Color.Red, 3, LineStyle.SimpleChart);
            SetIndicatorLine("Linha de Media Para o Preco", Color.Snow, 1, LineStyle.SimpleChart);
            SetIndicatorLine("Tendencia_Compra", Color.OliveDrab, 4, LineStyle.SimpleChart);
            SetIndicatorLine("Tendencia_Venda", Color.Thistle, 4, LineStyle.SimpleChart);
            SetIndicatorLine("Tendencia_Compra2", Color.Brown, 4, LineStyle.SimpleChart);
            SetIndicatorLine("Tendencia_Venda2", Color.Brown, 4, LineStyle.SimpleChart);
            SeparateWindow = false;
        }

        // Região de parâmetros de entrada
        #region Parameter

        // Período de Candles Cálculados
        [InputParameter("Período de Candles Cálculados", 0, 1, 9999)]
        public int Periodo_Da_Media = 20;

        // Valor da Sensibilidade de Cálculo
        [InputParameter("Valor da Sensibilidade de Cálculo", 2, 0, 999, 1, 0.1)]
        public double Fator_De_Sensibilidade = 1;

        // Período de Candles de Fixação do Preço
        [InputParameter("Período de Candles de Fixação do Preço", 0, 1, 9999)]
        public int Periodo_De_Candles = 1;

        // Período de Candles Para Definir Rompimento
        [InputParameter("Período de Candles Para Definir Rompimento", 0, 1, 9999)]
        public int Periodo_De_Candles_Rompimento = 1;

        // Tipo do Preço Para Rompimento de Candles
        [InputParameter("Tipo do Preço Para Rompimento de Candles", 1, new object[] {
            "Fechamento", PriceType.Close,
            "Abertura", PriceType.Open,
            "Máximo", PriceType.High,
            "Mínimo", PriceType.Low,
            "Típico", PriceType.Typical,
            "Médio", PriceType.Medium,
            "Peso", PriceType.Weighted})]
        public PriceType TipoPrecoRompimentoDeCandles = PriceType.Close;

        // Tipo do Preco Para Cálculo de Candles Linha Compra
        [InputParameter("Tipo do Preco Para Cálculo de Candles Linha Compra", 1, new object[] {
            "Fechamento", PriceType.Close,
            "Abertura", PriceType.Open,
            "Máximo", PriceType.High,
            "Mínimo", PriceType.Low,
            "Típico", PriceType.Typical,
            "Médio", PriceType.Medium,
            "Peso", PriceType.Weighted})]
        public PriceType TipoPrecoCandlesLinhaCompra = PriceType.Close;

        // Tipo do Preco Para Cálculo de Candles Linha Venda
        [InputParameter("Tipo do Preco Para Cálculo de Candles Linha Venda", 1, new object[] {
            "Fechamento", PriceType.Close,
            "Abertura", PriceType.Open,
            "Máximo", PriceType.High,
            "Mínimo", PriceType.Low,
            "Típico", PriceType.Typical,
            "Médio", PriceType.Medium,
            "Peso", PriceType.Weighted})]
        public PriceType TipoPrecoCandlesLinhaVenda = PriceType.Close;

        // Tipo do Preco Para Cálculo de Candles Linha Media Preco
        [InputParameter("Tipo do Preco Para Cálculo de Candles Linha Media Preco", 1, new object[] {
            "Fechamento", PriceType.Close,
            "Abertura", PriceType.Open,
            "Máximo", PriceType.High,
            "Mínimo", PriceType.Low,
            "Típico", PriceType.Typical,
            "Médio", PriceType.Medium,
            "Peso", PriceType.Weighted})]
        public PriceType TipoPrecoCandlesLinhaMediaPreco = PriceType.Close;

        // Tipo de Média Usada
        [InputParameter("Tipo de Média Usada", 1, new object[] {
            "Simples", MAMode.SMA,
            "Ponderada Linearmente", MAMode.LWMA})]
        public MAMode TipoDaMedia = MAMode.SMA;

        // Suavizar o Indicador em Relação ao Mercado
        [InputParameter("Suavisar o Indicador em Relação ao Mercado", 1, new object[] {
            "Sim", Suavisar.Sim,
            "Não", Suavisar.Nao,
            "Parcialmente", Suavisar.Parcial})]
        public Suavisar Suavisado = Suavisar.Sim;

        // Enum para suavizar o indicador
        public enum Suavisar
        {
            Sim = 1,
            Nao = 0,
            Parcial = 2
        }
        #endregion

        // Método de inicialização do indicador
        public override void Init()
        {
            // Define o nome do indicador
            IndicatorShortName(string.Format("Ciclo_TFMF({0})", CurrentData, Periodo_Da_Media, Fator_De_Sensibilidade, TipoDaMedia, TipoPrecoCandlesLinhaCompra, TipoPrecoCandlesLinhaVenda, TipoPrecoCandlesLinhaMediaPreco, Suavisado));
        }

        // Método chamado a cada nova cotação
        public override void OnQuote()
        {
            // Incrementa o contador de candles
            candle++;

            double data = DateTime.Today.DayOfYear;
            double data1 = DateTime.Today.Year;
            int limite = 365;

            // Verifica se o ano é maior ou igual a 2019 e se a data é maior que o limite
            if (data1 >= 2019 && data > limite)
            {
                return;
            }
            else
            {
                // Verifica se o número de dados é menor que o período da média mais 1
                if (CurrentData.Count < Periodo_Da_Media + 1)
                    return;

                // Chama o método de cálculo do indicador
                Calcula_FMF();
            }

        }

        // Método chamado a cada novo bar
        public override void NextBar()
        {
            // Incrementa o contador de candles
            candle++;

            // Verifica se o número de dados é menor que o período da média mais 1
            if (CurrentData.Count < Periodo_Da_Media + 1)
                return;
        }

        // Inicialização de variáveis
        int candle = 1;
        int nbase = 100000;
        double vendido = 0;
        double comprado = 0;
        bool LC = true;
        bool LV = true;

        // Método para calcular o indicador
        public void Calcula_FMF()
        {
            // Verifica se o contador de candles é maior que 1
            if (candle > 1)
            {
                double pega_ATR_periodo_selecionado = 0;
                switch (TipoDaMedia)
                {
                    case MAMode.SMA:
                        pega_ATR_periodo_selecionado = GetSMA();
                        break;

                    case MAMode.LWMA:
                        pega_ATR_periodo_selecionado = GetLWMA();
                        break;
                }

                double Tr = GetTrueRange(1);
                double multiplica_TR = (pega_ATR_periodo_selecionado - (((Fator_De_Sensibilidade * Tr) + Tr) / Periodo_Da_Media));
                double preco_fechamento_candle_anterior = CurrentData.GetPrice(TipoPrecoRompimentoDeCandles, 1);
                double linha_media_preco = CurrentData.GetPrice(TipoPrecoCandlesLinhaMediaPreco, Periodo_De_Candles);

                // Verifica se o preço de fechamento do candle anterior é menor que a base
                if (CurrentData.GetPrice(PriceType.Close, 1) < nbase)
                {
                    comprado = CurrentData.GetPrice(PriceType.Low, 1) - multiplica_TR;
                    vendido = CurrentData.GetPrice(PriceType.High, 1) + multiplica_TR;
                    nbase = 0;
                }

                // Verifica se o preço de fechamento do candle anterior é menor que o preço comprado
                if (preco_fechamento_candle_anterior < comprado)
                {
                    // Verifica se a suavização está ativada
                    if (Suavisado == Ciclo_TFMF.Suavisar.Sim)
                    {
                        comprado = CurrentData.GetPrice(TipoPrecoCandlesLinhaCompra, Periodo_De_Candles) - (multiplica_TR * (Fator_De_Sensibilidade));
                    }
                    else if (Suavisado == Ciclo_TFMF.Suavisar.Parcial)
                    {
                        comprado = CurrentData.GetPrice(TipoPrecoCandlesLinhaCompra, Periodo_De_Candles) - multiplica_TR;
                    }
                    else
                    {
                        comprado = CurrentData.GetPrice(TipoPrecoCandlesLinhaCompra, Periodo_De_Candles);
                    }

                    LC = false;
                    LV = true;

                    vendido = CurrentData.GetPrice(TipoPrecoCandlesLinhaVenda, Periodo_De_Candles) + (multiplica_TR * (Fator_De_Sensibilidade));
                }

                // Verifica se o preço de fechamento do candle anterior é maior que o preço vendido
                if (preco_fechamento_candle_anterior > vendido)
                {
                    // Verifica se a suavização está ativada
                    if (Suavisado == Ciclo_TFMF.Suavisar.Sim)
                    {
                        vendido = CurrentData.GetPrice(TipoPrecoCandlesLinhaVenda, Periodo_De_Candles) + (multiplica_TR * (Fator_De_Sensibilidade));
                    }
                    else if (Suavisado == Ciclo_TFMF.Suavisar.Parcial)
                    {
                        vendido = CurrentData.GetPrice(TipoPrecoCandlesLinhaVenda, Periodo_De_Candles) + multiplica_TR;
                    }
                    else
                    {
                        vendido = CurrentData.GetPrice(TipoPrecoCandlesLinhaVenda, Periodo_De_Candles);
                    }

                    LV = false;
                    LC = true;

                    comprado = CurrentData.GetPrice(TipoPrecoCandlesLinhaCompra, Periodo_De_Candles) - (multiplica_TR * (Fator_De_Sensibilidade));
                }

                // Define os valores do indicador
                if (LV == true)
                {
                    SetValue(1, 0, vendido);

                    if (CurrentData.GetPrice(PriceType.High, 0) > vendido)
                    {
                        SetMarker(1, 0, Color.Gray);
                    }
                }
                if (LC == true)
                {
                    SetValue(0, 0, comprado);
                    if (CurrentData.GetPrice(PriceType.Low, 0) < comprado)
                    {
                        SetMarker(0, 0, Color.Blue);
                    }
                }

                SetValue(2, 0, linha_media_preco);
            }
        }

        // Método para calcular o True Range
        private double GetTrueRange(int index)
        {
            double hi = CurrentData.GetPrice(PriceType.High, index);
            double lo = CurrentData.GetPrice(PriceType.Low, index);
            double prevClose = (CurrentData.Count > index + 1) ? CurrentData.GetPrice(PriceType.Close, index + 1) : CurrentData.GetPrice(PriceType.Close, index);

            return Math.Max(hi - lo, Math.Max(Math.Abs(prevClose - hi), Math.Abs(prevClose - lo)));
        }

        // Método para calcular a Média Móvel Simples
        public double GetSMA()
        {
            double summa = 0.0; // Soma dos preços
            // Loop de cálculo.
            for (int i = 0; i < Periodo_Da_Media; i++)
                summa += GetTrueRange(i);

            // Retorna o valor atual da SMA
            return summa / Periodo_Da_Media;
        }

        // Método para calcular a Média Móvel Exponencial
        public double GetEMA()
        {
            // Cálculo de um coeficiente
            double k = 2.0 / (Periodo_Da_Media + 1);
            double summa = (GetValue(0, 1) == Int32.MaxValue) ? GetTrueRange(0)
                                                              : GetValue(0, 1);
            summa += k * (GetTrueRange(0) - summa);

            // Retorna o valor
            return summa;
        }

        // Método para calcular a Média Móvel Modificada
        public double GetMMA()
        {
            double k = 1.0 / Periodo_Da_Media;// coeficiente

            //// Loop de cálculo.
            double oldMma = (GetValue(0, 1) == Int32.MaxValue) ? GetTrueRange(0)
                                                               : GetValue(0, 1);
            double mma = GetTrueRange(0) * k + oldMma * (1.0 - k);

            // Retorna mma
            return mma;
        }

        // Método para calcular a Média Móvel Ponderada Linearmente
        public double GetLWMA()
        {
            double numerator = 0.0;                  // Numerador da taxa
            double denominator = 0.0;                // Denominador da taxa
            int period = Periodo_Da_Media;           // contador de período
            double k = 1.0 / Periodo_Da_Media;       // coeficiente

            // Loop de cálculo.
            for (int i = 0; i < Periodo_Da_Media; i++)
            {
                numerator += period * GetTrueRange(i);
                denominator += period;

                period--;
            }

            // Retorna o valor atual
            if (denominator != 0)
                return numerator / denominator;
            else
                return 0;
        }
    }
}
