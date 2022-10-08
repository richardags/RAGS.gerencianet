namespace gerencianet.PIX.schemes
{
    public class CobrancaRequest
    {
        public class Calendario
        {
            public int expiracao;
        }
        public class Valor
        {
            public string original;
        }

        public Calendario calendario;
        public Valor valor;
        public string chave;
    }
}