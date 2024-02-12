using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)// ao clicar em registro
        {
            string enderecoforn = this.Controls["entrada"].Text; // recebe a informação da caixa de texto
            string indicador = "", complemento = "";
            
            // organiza e separa o endereço
            string[] resultado = Classificacao(enderecoforn);
            if(resultado[1] != null)
            {
                this.Controls["entrada"].Text = null; // apaga o texto digitado caso não existam erros
            }
            
            this.Controls["rua"].Text = "Rua: " + resultado[0];
            this.Controls["numero"].Text = "Número: " + resultado[1];
        }
        private string[] Classificacao(string analise)
        {
            string rua = "", numero = "";
            
            if(analise.Contains(" "))
            {
                string analiseSV;
                if(analise.Contains(",")) // retira as virgulas pra ficar bonito 
                {
                    string[] semvirgula = analise.Split(',');
                    analiseSV = string.Join(" ", semvirgula);
                }
                else
                {
                    analiseSV = analise;
                }
                string[] separa = analiseSV.Split(' ');
                string indicador = null, complemento = null, numeroex = null;
                List<string> palavras = new List<string>();
                List<string> numeros = new List<string>();

                if (Buscaindicador(separa) != null) // identifica indicadores de numeros
                {
                    indicador = Buscaindicador(separa);
                }
                if (Buscacomplemento(separa) != null) // identifica complementos de numeros
                {
                    complemento = Buscacomplemento(separa);
                }
                if(Buscanumerosexed(separa) != null)
                {
                    numeroex = Buscanumerosexed(separa);
                }
                foreach (var termo in separa) // separa numeros e palavras
                {
                    if (Contemnumero(termo))
                    {
                        if(termo == numeroex)
                        {
                            palavras.Add(termo);
                        }
                        else
                        {
                            numeros.Add(termo);
                        }
                    }
                    else
                    {
                        if(indicador == null && complemento == null) // checa se há indicadores de numeros ou complementos
                        {
                            palavras.Add(termo);
                        }
                        else
                        {
                            if(termo != indicador) // checa se o termo analizado é um indicador, se for, é descartado
                            {
                                if(termo == complemento) // checa se o termo é um complemento, se for, é enviado à lista de numeros
                                {
                                    numeros.Add(termo);
                                }
                                else
                                {
                                    palavras.Add(termo);
                                }
                            }
                        }
                    }
                }
                if (palavras.Count > 0 && numeros.Count > 0)
                {
                    rua = string.Join(" ", palavras);
                    numero = string.Join(" ", numeros);
                    return new string[] { rua, numero }; // retorna o resultado da analise;
                }
                else
                {
                    return new string[] { "Não foi possível REGISTRAR este endereço", null };
                }
                
            }
            else
            {
                return new string[] { "Não foi possível REGISTRAR este endereço", null };
            }
        }



        static bool Contemnumero(string palavraatual)
        {
            return palavraatual.Any(char.IsDigit);
        }
        static string Buscaindicador(string[] palavraatual)
        {
            List<string> termos = new List<string> { "Número", "número", "Numero", "numero", "No", "Nº" }; // termos para identificação de qual parte é o numero
            string indicador = null;
            foreach (var palavra in palavraatual)
            {
                if (termos.Any(termo => palavra.Contains(termo)))
                {
                    indicador = palavra;
                }
            }
            return indicador;
        }
        static string Buscacomplemento(string[] palavraatual)
        {
            string complemento = null;
            foreach(var termo in palavraatual)
            {
                if (termo.Length == 1 && !Contemnumero(termo))
                {
                    if (Contemnumero(palavraatual[Array.IndexOf(palavraatual, termo) - 1])) // checa se é um complemento baseado no elemento anterior
                    {
                        complemento = termo;
                    }
                }
            }
            return complemento;
        }
        static string Buscanumerosexed(string[] palavraatual)
        {
            string resnum = null;
            List<string> numeros = new List<string>();
            List<string> separanum = new List<string>();
            foreach(var termo in palavraatual)
            {
                if(Contemnumero(termo))
                {
                    numeros.Add(termo);
                }
            }
            if(numeros.Count > 1) // em caso de mais de um numero no endereço
            {
                string indicador = Buscaindicador(palavraatual); // checa se existe um indicador de numero
                foreach (var termo in palavraatual)
                {
                    if(Contemnumero(termo) && indicador != palavraatual[Array.IndexOf(palavraatual, termo) - 1]) // checa se o termo anterior é um indicador
                    {
                        separanum.Add(termo);
                    }
                }
            }
            resnum = string.Join(" ", separanum.ToArray());
            return resnum;
        }



        /*string rua = "", numero = "";
            if(analise.Contains(" ")) // busca espaços no endereço para separar palavras
            {
                string[] separa = analise.Split(' ');
                List<string> palavras = new List<string>();
                List<string> numeros = new List<string>();

                if (Contemnumero(analise)) // busca numeros entre as palavras separadas
                {
                    foreach (var termo in separa)
                    {
                        if(Contemnumero(termo))
                        {
                            numeros.Add(termo);
                        }
                        else
                        {
                            if(termo.Length == 1 && numeros.Count == 1)
                            {
                                if(Contemnumero(separa[Array.IndexOf(separa, termo) - 1])) // checa se é um complemento baseado no elemento anterior
                                {
                                    numeros.Add(termo);
                                }
                                else
                                {
                                    palavras.Add(termo);
                                }
                            }
                            else
                            {
                                palavras.Add(termo);
                            }
                        }
                    }
                    if(numero.Length > 1) // checa se há mais de um item na lista numero registrado
                    {
                        if (Buscaindicador(analise)) // busca identificador de numero de endereço
                        {
                            foreach (var palavra in separa)
                            {
                                bool encontrou = false;
                                if (Buscaindicador(palavra))
                                {
                                    encontrou = true;
                                }
                                if (!encontrou)
                                {
                                    palavras.Add(palavra);
                                }
                                else
                                {
                                    numeros.Add(palavra);
                                }
                            }
                            rua = string.Join(" ", palavras);
                            numero = string.Join(" ", numeros);
                            return new string[] { rua, numero };
                        }
                        else
                        {
                            foreach (var termo in numeros)
                            {
                                if(termo.Length == 1 && !Contemnumero(termo)) // verifica se é um complemento
                                {
                                    continue;
                                }
                                else
                                {
                                    return new string[] { "Não foi possível REGISTRAR este endereço(erro 3)", null };
                                }
                            }
                            rua = string.Join(" ", palavras);
                            numero = string.Join(" ", numeros);
                            return new string[] { rua, numero };
                        }
                    }
                    else
                    {
                        rua = string.Join(" ", palavras);
                        numero = string.Join(" ", numeros);
                        return new string[] { rua, numero };
                    }
                }
                else
                {
                    return new string[] { "Não foi possível REGISTRAR este endereço(erro 2)", null };
                }
            }
            else
            {
                return new string[] { "Não foi possível REGISTRAR este endereço (Erro 1)", null };
            }*/
    }
}
