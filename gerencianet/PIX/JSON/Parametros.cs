using System;

namespace gerencianet.PIX.JSON
{
    public class Parametros
    {
        public class Paginacao
        {
            public int paginaAtual;
            public int itensPorPagina;
            public int quantidadeDePaginas;
            public int quantidadeTotalDeItens;
        }

        public DateTimeOffset inicio;
        public DateTimeOffset fim;
        public Paginacao paginacao;
        public string cpf;
        public string cnpj;
        public Status status;
    }
}