using gerencianet.PIX;
using gerencianet.PIX.JSON;
using gerencianet.PIX.schemes;
using System;

namespace Debug
{
    class Program
    {
        static PixAPI pix;

        static void Main(string[] args)
        {
            pix = new(
                gerencianet.PIX.EnvironmentType.PRODUCAO,
                "Client_Id_123456789",
                "Client_Secret_123456789",
                "Debug.producaoCertificationFile.p12"
                );

            //cria uma nova cobrança
            CobrancaRequest cobranca = new CobrancaRequest();
            cobranca.calendario = new CobrancaRequest.Calendario();
            cobranca.calendario.expiracao = 3600;

            cobranca.valor = new CobrancaRequest.Valor();
            cobranca.valor.original = "0.01";

            cobranca.chave = "123456789-123456789-123456789-123456789-123456789"; //sua chave EVP

            PixResult result = pix.CriarCobrancaImediata(cobranca);

            Console.WriteLine(((CobrancaResponse)result.data).txid);
            Console.WriteLine(((CobrancaResponse)result.data).location);

            //consultar cobrança by TxId
            PixResult resultConsultarCobranca = pix.ConsultarCobranca("123456789");

            if(resultConsultarCobranca.error == null)
            {
                if (resultConsultarCobranca.data != null)
                {
                    CobrancaResponse cobrancaResponse = (CobrancaResponse) resultConsultarCobranca.data;
                    Console.WriteLine(cobrancaResponse.calendario.criacao);
                    Console.WriteLine(cobrancaResponse.txid);
                    Console.WriteLine(cobrancaResponse.location);
                    Console.WriteLine(cobrancaResponse.status);
                    Console.WriteLine(cobrancaResponse.chave);
                    Console.WriteLine(cobrancaResponse.valor);
                }
                else
                {
                    Console.WriteLine("cobrança não existe ou foi deletada");
                }
            }
            else
            {
                Console.WriteLine(resultConsultarCobranca.error);
            }

            //consultar varias cobranças
            Parametros parametros = new();
            parametros.inicio = DateTimeOffset.Now.AddMonths(-6);
            parametros.fim = DateTimeOffset.Now.AddMonths(1);
            // outros parametros possiveis
            //parametros.status = Status.CONCLUIDA;
            //parametros.cpf = "123456789";
            //parametros.paginacao = new();
            //parametros.paginacao.paginaAtual = 2;
            //parametros.paginacao.itensPorPagina = 2;

            PixResult resultConsultarCobrancas = pix.ConsultarCobrancas(parametros);

            if(resultConsultarCobrancas.error == null)
            {
                CobrancasResponse cobrancasResponse = (CobrancasResponse) resultConsultarCobrancas.data;

                Console.WriteLine(cobrancasResponse.cobs.Count + " cobranças encontradas.");
            }
            else
            {
                Console.WriteLine(resultConsultarCobrancas.error);
            }
                      
            //listar chaves EVP
            PixResult resultListarChavesEVP = pix.ListarChavesEVP();
            if (resultListarChavesEVP.error == null)
            {
                ListarChavesEVPResponse listarChavesEVPResponse = (ListarChavesEVPResponse) resultListarChavesEVP.data;

                Console.WriteLine(listarChavesEVPResponse.chaves.Count + " chaves encontradas");
            }
            else
            {
                Console.WriteLine(resultListarChavesEVP.error);
            }

            //recuperar location
            PixResult resultRecuperarLocation = pix.RecuperarLocation("10");
            if (resultRecuperarLocation.error == null)
            {
                Location location = (Location)resultRecuperarLocation.data;

                Console.WriteLine(location.id);
                Console.WriteLine(location.txid);
                Console.WriteLine(location.criacao);
                Console.WriteLine(location.location);
            }
            else
            {
                Console.WriteLine(resultRecuperarLocation.error);
            }

            //gerar QRCode e Pix Copia e Cola
            PixResult resultGerarQRCode = pix.GerarQRCode("264");
            if (resultGerarQRCode.error == null)
            {
                QRCode qrCode = (QRCode)resultGerarQRCode.data;

                Console.WriteLine(qrCode.imagemQrcode);
                Console.WriteLine(qrCode.qrcode);
            }
            else
            {
                Console.WriteLine(resultGerarQRCode.error);
            }

            Console.ReadKey();
        }

        //cria sua primeira chave EVP
        //WARNING: só precisa criar essa chave EVP uma vez
        public static void CriarChaveEVP()
        {
            PixResult resultCriarChaveEVP = pix.CriarChaveEVP();
            if (resultCriarChaveEVP.error == null)
            {
                CriarChaveEVPResponse criarChaveEVPResponse = (CriarChaveEVPResponse)resultCriarChaveEVP.data;

                Console.WriteLine(criarChaveEVPResponse.chave); //salve esta chave para futuras cobranças
            }
            else
            {
                Console.WriteLine(resultCriarChaveEVP.error);
            }
        }
    }
}