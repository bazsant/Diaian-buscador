using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diaian
{
    class Program
    {
        public static Dictionary<string, int> PalavrasChave { get; set; }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Bem vindo ao Programa Diaian\n\nPara buscar palavras nos diretórios, \ninforme as palavras separadas por \";\" (ex: select;insert;update;delete )\ne aperte Enter\n\n");

                string comando = Console.ReadLine();

                PalavrasChave = new Dictionary<string, int>();

                foreach (var palavra in comando.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    PalavrasChave.Add(palavra, 0);
                }

                Console.WriteLine("Obrigado! Agora aguarde até terminarmos a busca\n\n");

                string diretorioRaiz = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                using (StreamWriter arquivo = new StreamWriter(DateTime.Now.ToFileTimeUtc() + ".txt"))
                {

                    DirectoryInfo diretorioNutella = new DirectoryInfo(diretorioRaiz);
                    
                    VerificarArquivos(diretorioNutella.GetFiles(), arquivo);
                    VerificarDiretorios(diretorioNutella.GetDirectories(), arquivo);

                    foreach (var palavra in PalavrasChave)
                    {
                        arquivo.WriteLine(string.Format("Quantidade total de {0} - {1}", palavra.Key, palavra.Value));
                    }
                }

                Console.WriteLine("Ufa! Terminamos. Foi gerado um arquivo com as informações\nAgora aperte uma tecla para fechar...");
                Console.ReadKey();

            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void VerificarDiretorios(DirectoryInfo[] diretorios, StreamWriter arquivo)
        {
            foreach (var diretorio in diretorios)
            {
                VerificarArquivos(diretorio.GetFiles(), arquivo);
                VerificarDiretorios(diretorio.GetDirectories(), arquivo);
            }
        }

        private static void VerificarArquivos(FileInfo[] arquivos, StreamWriter arquivo)
        {
            foreach (var file in arquivos)
            {
                AbrirArquivo(file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite), file.FullName, arquivo);
            }
        }

        private static void AbrirArquivo(FileStream fileStream, string caminho, StreamWriter arquivo)
        {
            List<string> chaves = new List<string>(PalavrasChave.Keys);
            string linha = string.Empty;

            StringBuilder gravarArquivo = new StringBuilder();
            gravarArquivo.AppendLine();

            bool encontrou = false;

            using (StreamReader reader = new StreamReader(fileStream))
            {
                int numeroLinha = 1;
                do
                {
                    linha = reader.ReadLine();

                    if (!string.IsNullOrEmpty(linha) &&
                        !linha.StartsWith("//") &&
                        PalavrasChave.Any(x => linha.ToLowerInvariant().Contains(x.Key)))
                    {
                        foreach (var chave in chaves)
                        {
                            PalavrasChave[chave] += Regex.Matches(linha, chave).Count;
                        }

                        gravarArquivo.AppendLine(string.Format("{0} - {1}", numeroLinha, linha));

                        encontrou = true;
                    }

                    numeroLinha++;

                } while (!reader.EndOfStream);

            }

            if (encontrou)
            {
                arquivo.WriteLine(string.Format("Arquivo: {0}{1}", caminho, gravarArquivo.ToString()));
            }

        }

    }
}
