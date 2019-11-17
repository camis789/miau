using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using SisProdutos.models;
using SisProdutos.Service;

namespace SisProdutos
{
    class Program
    {
        static void Main(string[] args)
        {
            User user = new User();

            UserService userService = new UserService();

            ProductService productService = new ProductService();

            CategoryService categoryService = new CategoryService();
            string res;
            int menu;
        menuu:
            Console.WriteLine("\n\n                     SISTEMA DE CADASTRO DE PRODUTOS         ");
            Console.WriteLine("\n\n\n\n\n     1 - Adicionar Usuário");
            Console.WriteLine("     2 - Listar Usuários");
            Console.WriteLine("     3 - Fazer Login");
            menu = Convert.ToInt32(Console.ReadLine());
            
            switch (menu)
            {
                default:
                    Console.WriteLine("Opção Invalida");
                    Console.WriteLine("Tentar Novamente s/n");
                    res = Console.ReadLine();

                    if (res == "s")
                    {
                        Console.Clear();
                        goto menuu;
                    }
                    break;
                case 1:
                    //Adicionar usuario
                    Console.Clear();
                    Console.WriteLine("Digite nome");
                        user.Name = Console.ReadLine();

                        Console.WriteLine("Digite email");
                        user.Email = Console.ReadLine();

                        Console.WriteLine("Digite Password");
                        user.Password = Console.ReadLine();

                        user.Access = Access.admin;

                    var resultUserEmail = userService.TestEmail(user);
                    var resultUserName = userService.TestUser(user);


                    if (resultUserEmail || resultUserName)
                    {
                        Console.WriteLine("Não foi possivel cadastrar, e-mail ou nome já cadastrado");

                    }
                    else
                    {
                        Console.WriteLine("Cadastrado com sucesso");
                        userService.AddUser(user);
                    }

                    Console.WriteLine("Tentar Novamente s/n");
                    res = Console.ReadLine();

                    if (res == "s")
                    {
                        Console.Clear();

                        goto menuu;

                    }
                    else
                    {
                        break;
                    }

                case 2:
                    //Listar usuarios
                    var resultListUser = userService.ListUsers();

                    foreach (var item in resultListUser)
                    {
                        Console.WriteLine("Nome: " + item.Name);
                    }
                    break;

                case 3:
                    //Login
                    //obs dentro de fazer login tem que ter o listar produtos(membro e adm) e cadastrar produtos(adm)
                    Console.Clear();
                    Console.WriteLine("\nDigite email");
                    user.Email = Console.ReadLine();

                    Console.WriteLine("Digite senha");
                    user.Password = Console.ReadLine();

                    var resultAuth = userService.Auth(user);

                    if (resultAuth)
                    {
                        Console.Clear();
                        Console.WriteLine("Logado");
                    }
                    else
                    {
                        Console.WriteLine("Incorreto");
                    }


                    while (resultAuth == true)
                    {
                        Product product = new Product();
                        Category category = new Category();
                        int opcao;
                    menu1:
                        Console.WriteLine("                 Bem Vindo, {0}", user.Email);
                        Console.WriteLine("\n\n\n       1 - Cadastro de Produtos\n");
                        Console.WriteLine("       2 - Listar Produtos");
                        opcao = Convert.ToInt32(Console.ReadLine());

                        switch (opcao)
                        {
                            case 1:
                            //ADICIONAR PRODUTO

                            nome:
                                string result;
                                Console.Clear();
                                Console.WriteLine("Digite Nome");
                                result = Console.ReadLine();

                                string pattern = @"(?i)[^0-9a-zA-Z]";
                  
                                string subs;
                                Console.WriteLine("Caracteres especiais não podem ser utilizados");
                                Console.WriteLine("Caracteres especiais serão retirados");
                                string replacement = "";
                                Regex rgx = new Regex(pattern);
                                product.Name = rgx.Replace(result, replacement);
                                Console.WriteLine("Deseja substituir nome s/n");

                                subs = Console.ReadLine();
                                if (subs == "s")
                                {
                                    goto nome;
                                }
                                
                         
                                price:
                                string texto;
                                
                                Console.WriteLine("Digite Preço");
                                texto = Console.ReadLine();
                               var resultString = Regex.Match(texto, @"^(\d*\,)\d{1,5}$").Value;


                                if (Regex.IsMatch(texto, @"^(\d*\,)\d{1,5}$"))
                                {

                                    product.Price = Convert.ToDecimal(texto);
                                    
                                      
                                }
                                else
                                {
                                    Console.WriteLine("Somente números decimais são suportados, por favor digite um valor válido!");
                                        Console.WriteLine("Exemplo: 12,30");
                                    goto price;
                                }
                            labelqtd:
                                string valor;
                                Console.WriteLine("Digite Quantidade");
                                valor = Console.ReadLine();
                                var resQtd = Regex.Match(valor, @"\d+").Value;
                                

                                if (Regex.IsMatch(valor, @"\d"))
                                { 
                                    product.Stock = Convert.ToInt32(valor);
                                }
                                else
                                {
                                    Console.WriteLine("Somente números são suportados, por favor digite um valor válido!");
                                    Console.WriteLine("Exemplo: 12");
                                    goto labelqtd;
                                }

                                Console.WriteLine("Digite a Categoria:");
                                category.Name = Console.ReadLine();
                                var resultTest = categoryService.CategoryTest(category);
                                if (resultTest)
                                {

                                    category.Products = new List<Product>();

                                    category.Products.Add(product);
                                    categoryService.AddCategory(category);
                                    productService.AddProduct(product);
                                    Console.WriteLine("Categoria criada com sucesso!");
                                    

                                }
                                else
                                {
                                    var categoriaQExist = categoryService.CategoryReturn(category);


                                    categoriaQExist.Products.Add(product);
                                    productService.AddProduct(product);
                                    Console.WriteLine("Essa categoria já existe!");
                                    Console.WriteLine("O produto foi adicionado a esta categoria!");
                                   
                                   
                                    
                                    

                                }


                               

                                Console.WriteLine("Retorno ok");
                                Console.WriteLine("Deseja voltar s/n");

                                res = Console.ReadLine();

                                if (res == "s")
                                {
                                    Console.Clear();
                                    goto menu1;
                                }
                                else
                                {
                                    break;
                                }

                            case 2:
                                var resultListCategory = categoryService.ListCategory();
                                var resultListProduct = productService.ListProducts();

                                foreach (var items in resultListCategory)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Categoria: " + items.Name);

                                      foreach (var item in resultListProduct)
                                    {
                                        Console.WriteLine("Id: " + item.Id);
                                        Console.WriteLine("Nome: " + item.Name);
                                        Console.WriteLine("Preço: " + item.Price);
                                        Console.WriteLine("Quantidade no Estoque: " + item.Stock);
                                        Console.WriteLine("------------------------------------");
                                       
                                    }
                                }
                                Console.WriteLine("Deseja voltar s/n");
                               
                                res = Console.ReadLine();
                                
                                if (res == "s")
                                {
                                    Console.Clear();
                                    goto menu1;
                                }
                                else
                                {
                                    break;
                                }

                            default:
                                Console.WriteLine("Opção Invalida");

                                goto menu1;
                        }



                        break;

                      
                    }
                    while (resultAuth == false)
                    {

                        Console.WriteLine("Deseja voltar s/n");

                        res = Console.ReadLine();

                        if (res == "s")
                        {
                            Console.Clear();
                            goto menuu;
                        }
                        else
                        {
                            break;
                        }
                        
                    }
                    break;
            }

        }
    }
}
