using gerencianet.PIX.certificate;
using gerencianet.PIX.JSON;
using gerencianet.PIX.schemes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HttpClientHelper;
using System.Net.Http;

namespace gerencianet.PIX
{
    public class PixAPI
    {
        private readonly string OAuth2;
        private X509Certificate2 certificate;
        private string accessToken = "0";
        private PixURL pixURL;

        /// <summary>
        /// Initialize object
        /// </summary>
        /// <param name="clientId">clientId from gerencianet</param>
        /// <param name="clientSecret">clientSecret from gerencianet</param>
        /// <param name="certificatePath">Path format: "YourProject.NameOfFolderIfExist.yourCertificateFile.p12"</param>
        public PixAPI(EnvironmentType environmentType, string clientId, string clientSecret, string certificatePath)
        {
            OAuth2 = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret));
            this.certificate = Certificate.Import(certificatePath);
            this.pixURL = new(environmentType);
        }

        private PixResult InitToken()
        {
            PixResult result = Token();

            if (result.error == null)
            {
                accessToken = ((Token)result.data).accessToken;                
            }

            return result;
        }

        private PixResult Token()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", OAuth2);

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("grant_type", "client_credentials");
            
            HttpClientHelperResult result = HttpClientHelperNS.Response(
                pixURL.Token(), HttpMethod.Post,
                headers, parameters, ContentType.JSON,
                certificate: certificate);

            if (result.error == null)
            {
                if (result.data != null)
                {
                    switch (result.httpStatusCode)
                    {
                        case HttpStatusCode.OK:
                            return new PixResult(data: JsonConvert.DeserializeObject<Token>(result.data));
                        default:
                            return new PixResult(error: new Exception(string.Format(
                                "PixAPI.cs Token() - httpStatusCode({0}) data({1})",
                                result.httpStatusCode, result.data)));
                    }
                }
                else
                {
                    return new PixResult(error: new Exception(string.Format(
                        "PixAPI.cs Token() - httpStatusCode({0}) data(null)",
                        result.httpStatusCode)));
                }
            }
            else
            {
                return new PixResult(error: result.error);
            }
        }

        public PixResult CriarCobrancaImediata(CobrancaRequest cobranca)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + accessToken);

            HttpClientHelperResult result = HttpClientHelperNS.Response(
            pixURL.CriarCobrancaImediata(), HttpMethod.Post, headers,
            contentType: ContentType.JSON,
            stringContent: JsonConvert.SerializeObject(cobranca),
            certificate: certificate);

            if (result.error == null)
            {
                if (result.data != null)
                {
                    switch (result.httpStatusCode)
                    {
                        case HttpStatusCode.Created:
                            return new PixResult(data: JsonConvert.DeserializeObject<CobrancaResponse>(result.data));
                        case HttpStatusCode.Unauthorized:
                            PixResult initToken = InitToken();
                            return initToken.error == null ? CriarCobrancaImediata(cobranca) : initToken;
                        default:
                            return new PixResult(error: new Exception(string.Format(
                                "PixAPI.cs Token() - httpStatusCode({0}) data({1})",
                                result.httpStatusCode, result.data)));
                    }
                }
                else
                {
                    return new PixResult(error: new Exception(string.Format(
                        "PixAPI.cs Token() - httpStatusCode({0}) data(null)",
                        result.httpStatusCode)));
                }
            }
            else
            {
                return new PixResult(error: result.error);
            }
        }

        public PixResult ConsultarCobranca(string txid)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + accessToken);

            HttpClientHelperResult result = HttpClientHelperNS.Response(
                pixURL.ConsultarCobranca(txid), HttpMethod.Get,
                headers, certificate: certificate);

            if (result.error == null)
            {
                if (result.data != null)
                {
                    switch (result.httpStatusCode)
                    {
                        case HttpStatusCode.OK:
                            return new PixResult(data: JsonConvert.DeserializeObject<CobrancaResponse>(result.data));
                        case HttpStatusCode.BadRequest:
                            return new PixResult(data: null);
                        case HttpStatusCode.Unauthorized:
                            PixResult initToken = InitToken();
                            return initToken.error == null ? ConsultarCobranca(txid) : initToken;
                        default:
                            return new PixResult(error: new Exception(string.Format(
                                "PixAPI.cs Token() - httpStatusCode({0}) data({1})",
                                result.httpStatusCode, result.data)));
                    }
                }
                else
                {
                    return new PixResult(error: new Exception(string.Format(
                        "PixAPI.cs Token() - httpStatusCode({0}) data(null)",
                        result.httpStatusCode)));
                }
            }
            else
            {
                return new PixResult(new PixResult(error: result.error));
            }
        }

        public PixResult ConsultarCobrancas(Parametros parametros)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + accessToken);

            HttpClientHelperResult result = HttpClientHelperNS.Response(
                pixURL.ConsultarCobrancas(parametros), HttpMethod.Get,
                headers, certificate: certificate);

            if (result.error == null)
            {
                if (result.data != null)
                {
                    switch (result.httpStatusCode)
                    {
                        case HttpStatusCode.OK:
                            return new PixResult(data: JsonConvert.DeserializeObject<CobrancasResponse>(result.data));
                        case HttpStatusCode.Unauthorized:
                            PixResult initToken = InitToken();
                            return initToken.error == null ? ConsultarCobrancas(parametros) : initToken;
                        default:
                            return new PixResult(error: new Exception(string.Format(
                                "PixAPI.cs Token() - httpStatusCode({0}) data({1})",
                                result.httpStatusCode, result.data)));
                    }
                }
                else
                {
                    return new PixResult(error: new Exception(string.Format(
                        "PixAPI.cs Token() - httpStatusCode({0}) data(null)",
                        result.httpStatusCode)));
                }
            }
            else
            {
                return new PixResult(new PixResult(error: result.error));
            }
        }

        /// <summary>
        /// Consultar locations cadastradas.
        /// </summary>
        /// <param name="id">location id.</param>
        /// <returns>Location object.</returns>
        public PixResult RecuperarLocation(string id)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + accessToken);

            HttpClientHelperResult result = HttpClientHelperNS.Response(
                pixURL.RecuperarLocation(id), HttpMethod.Get,
                headers, certificate: certificate);

            if (result.error == null)
            {
                if (result.data != null)
                {
                    switch (result.httpStatusCode)
                    {
                        case HttpStatusCode.OK:
                            return new PixResult(data: JsonConvert.DeserializeObject<Location>(result.data));
                        case HttpStatusCode.BadRequest:
                            return new PixResult(data: null);
                        case HttpStatusCode.Unauthorized:
                            PixResult initToken = InitToken();
                            return initToken.error == null ? RecuperarLocation(id) : initToken;
                        default:
                            return new PixResult(error: new Exception(string.Format(
                                "PixAPI.cs Token() - httpStatusCode({0}) data({1})",
                                result.httpStatusCode, result.data)));
                    }
                }
                else
                {
                    return new PixResult(error: new Exception(string.Format(
                        "PixAPI.cs Token() - httpStatusCode({0}) data(null)",
                        result.httpStatusCode)));
                }
            }
            else
            {
                return new PixResult(new PixResult(error: result.error));
            }
        }

        /// <summary>
        /// Gerar QRCode de um location.
        /// </summary>
        /// <param name="id">location id.</param>
        /// <returns>QRCode object.</returns>
        public PixResult GerarQRCode(string id)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + accessToken);

            HttpClientHelperResult result = HttpClientHelperNS.Response(
                pixURL.GerarQRCode(id), HttpMethod.Get,
                headers, certificate: certificate);

            if (result.error == null)
            {
                if (result.data != null)
                {
                    switch (result.httpStatusCode)
                    {
                        case HttpStatusCode.OK:
                            return new PixResult(data: JsonConvert.DeserializeObject<QRCode>(result.data));
                        case HttpStatusCode.BadRequest:
                            return new PixResult(data: null);
                        case HttpStatusCode.Unauthorized:
                            PixResult initToken = InitToken();
                            return initToken.error == null ? GerarQRCode(id) : initToken;
                        default:
                            return new PixResult(error: new Exception(string.Format(
                                "PixAPI.cs Token() - httpStatusCode({0}) data({1})",
                                result.httpStatusCode, result.data)));
                    }
                }
                else
                {
                    return new PixResult(error: new Exception(string.Format(
                        "PixAPI.cs Token() - httpStatusCode({0}) data(null)",
                        result.httpStatusCode)));
                }
            }
            else
            {
                return new PixResult(new PixResult(error: result.error));
            }
        }

        public PixResult ListarChavesEVP()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + accessToken);

            HttpClientHelperResult result = HttpClientHelperNS.Response(
                pixURL.ListarChavesEVP(), HttpMethod.Get,
                headers, certificate: certificate);

            if (result.error == null)
            {
                if (result.data != null)
                {
                    switch (result.httpStatusCode)
                    {
                        case HttpStatusCode.OK:
                            return new PixResult(data: JsonConvert.DeserializeObject<ListarChavesEVPResponse>(result.data));
                        case HttpStatusCode.Unauthorized:                            
                            PixResult initToken = InitToken();
                            return initToken.error == null ? ListarChavesEVP() : initToken;
                        default:
                            return new PixResult(error: new Exception(string.Format(
                                "PixAPI.cs Token() - httpStatusCode({0}) data({1})",
                                result.httpStatusCode, result.data)));
                    }
                }
                else
                {
                    return new PixResult(error: new Exception(string.Format(
                        "PixAPI.cs Token() - httpStatusCode({0}) data(null)",
                        result.httpStatusCode)));
                }
            }
            else
            {
                return new PixResult(new PixResult(error: result.error));
            }
        }

        public PixResult CriarChaveEVP()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + accessToken);

            HttpClientHelperResult result = HttpClientHelperNS.Response(
                pixURL.CriarChaveEVP(), HttpMethod.Post,
                headers, certificate: certificate);

            if (result.error == null)
            {
                if (result.data != null)
                {
                    switch (result.httpStatusCode)
                    {
                        case HttpStatusCode.Created:
                            return new PixResult(data: JsonConvert.DeserializeObject<CriarChaveEVPResponse>(result.data));
                        case HttpStatusCode.BadRequest:
                            return new PixResult(data: null);
                        case HttpStatusCode.Unauthorized:
                            PixResult initToken = InitToken();
                            return initToken.error == null ? CriarChaveEVP() : initToken;
                        default:
                            return new PixResult(error: new Exception(string.Format(
                                "PixAPI.cs Token() - httpStatusCode({0}) data({1})",
                                result.httpStatusCode, result.data)));
                    }
                }
                else
                {
                    return new PixResult(error: new Exception(string.Format(
                        "PixAPI.cs Token() - httpStatusCode({0}) data(null)",
                        result.httpStatusCode)));
                }
            }
            else
            {
                return new PixResult(new PixResult(error: result.error));
            }
        }
    }
}