
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            List<product> products = new List<product>() 
            { 
            new product(){ ID=1,Name="pc"},
            new product(){ ID=2,Name="labtop"}
            
            };
            var dict = products.ToDictionary(x => x.ID.ToString());
          var res=  dict.FirstOrDefault();
        }
    }
}
