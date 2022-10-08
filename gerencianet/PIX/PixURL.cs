using gerencianet.PIX.JSON;
using HttpClientHelper;
using System.Collections.Generic;

namespace gerencianet.PIX
{
    internal class PixURL
    {
        private static readonly string URL_BASE_HOMOLOGACAO = "https://api-pix-h.gerencianet.com.br";
        private static readonly string URL_BASE_PRODUCAO = "https://api-pix.gerencianet.com.br";

        private EnvironmentType environmentType;

        public PixURL(EnvironmentType environmentType)
        {
            this.environmentType = environmentType;
        }

        public string URL_BASE
        {
            get
            {
                return environmentType == EnvironmentType.HOMOLOGACAO ? URL_BASE_HOMOLOGACAO : URL_BASE_PRODUCAO;
            }
        }

        public string Token()
        {
            return URL_BASE + "/oauth/token";
        }

        public string CriarCobrancaImediata()
        {
            return URL_BASE + "/v2/cob";
        }
        public string ConsultarCobranca(string txtid)
        {
            return URL_BASE + "/v2/cob/" + txtid;
        }

        public string ConsultarCobrancas(Parametros parametros)
        {
            string format = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";

            Dictionary<string, object> parameters = new();
            parameters.Add("inicio", parametros.inicio.ToString(format));
            parameters.Add("fim", parametros.fim.ToString(format));

            if (parametros.cpf != null)
            {
                parameters.Add("cpf", parametros.cpf);
            }
            else if (parametros.cnpj != null)
            {
                parameters.Add("cnpj", parametros.cnpj);
            }

            if (parametros.status != Status.NONE)
            {
                parameters.Add("status", parametros.status);
            }

            if (parametros.paginacao != null)
            {
                if (parametros.paginacao.paginaAtual != 0)
                {
                    parameters.Add("paginacao.paginaAtual", parametros.paginacao.paginaAtual);
                }

                if (parametros.paginacao.itensPorPagina != 0)
                {
                    parameters.Add("paginacao.itensPorPagina", parametros.paginacao.itensPorPagina);
                }
            }

            return URL_BASE + "/v2/cob?" + HttpClientHelperUtils.GetURLEncoded(parameters);
        }

        public string RecuperarLocation(string id)
        {
            return URL_BASE + "/v2/loc/" + id;
        }

        public string GerarQRCode(string id)
        {
            return URL_BASE + "/v2/loc/" + id + "/qrcode";
        }

        public string ListarChavesEVP()
        {
            return URL_BASE + "/v2/gn/evp";
        }
        public  string CriarChaveEVP()
        {
            return URL_BASE + "/v2/gn/evp";
        }
    }
}