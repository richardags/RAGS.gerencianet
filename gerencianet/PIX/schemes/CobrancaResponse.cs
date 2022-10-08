using gerencianet.PIX.JSON;

namespace gerencianet.PIX.schemes
{
    public class CobrancaResponse
    {
        public Calendario calendario;
        public Valor valor;
        public string chave;
        public string txid;
        public int revisao;
        public Location loc;
        public string location;
        public Status status;
    }
}