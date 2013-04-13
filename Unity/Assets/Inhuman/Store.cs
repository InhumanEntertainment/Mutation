using UnityEngine;
using System.Collections;

namespace Inhuman
{
    public class Store
    {
        public Product[] Products;
        public void Initialize() { }
        public void GetProducts() { }
        public void BuyProduct(string product, int amount) { }
        public void Restore() { }
    }

    public class Product
    {
        public string Name;
        public string Id;
        public bool Purchased = false;
    }
}