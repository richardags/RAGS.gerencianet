using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace gerencianet.PIX.certificate
{
    internal class Certificate
    {
        public static string ValidateCertificatePath(string certificatePath)
        {
            Regex regex = new("^([a-zA-Z0-9-]+)(?:\\.[a-zA-Z0-9-]+)+\\.p12$");
            Match match = regex.Match(certificatePath);

            return match.Success ? match.Groups[1].Value : null;
        }

        public static X509Certificate2 Import(string certificatePath)
        {
            //var allRessources = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            string projectName = ValidateCertificatePath(certificatePath);

            if(projectName == null)
            {
                throw new Exception("[gerencianet.PIX.certificate] [Certificate.cs] Import - error: certificatePath invalid path format.");
            }

            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach(Assembly assembly in assemblies)
            {
                if(!assembly.FullName.StartsWith(projectName + ", "))
                {
                    continue;
                }
                else if(!assembly.GetManifestResourceNames().Contains(certificatePath))
                {
                    continue;
                }

                using (Stream cs = assembly.GetManifestResourceStream(certificatePath))
                {
                    Byte[] raw = new Byte[cs.Length];

                    for (Int32 i = 0; i < cs.Length; ++i)
                    {
                        raw[i] = (Byte)cs.ReadByte();
                    }

                    return new X509Certificate2(raw);
                }
            }

            throw new Exception("[gerencianet.PIX.certificate] [Certificate.cs] Import - error: certificatePath not found. Are you missing to change .p12 file to 'Build Action->Embedded resource'?");
        }
    }
}