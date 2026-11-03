using System;
using System.Collections.Generic;
using System.Text;

namespace WinFormsApp1
{
    internal class Product
    {
        private static int _seq = 0;
        public int _id { get; private set; } = ++_seq;
        public string _name { get; set; } = "";
        public string _category { get; set; } = "";
        public decimal _price { get; set; } = 0;
        public int _stock { get; set; } = 0;

        public Product() { }
        public Product(string Name, string category, decimal price, int stock)
        {
            //this._id = Id;
            this._name = Name;
            this._category = category;
            this._price = price;
            this._stock = stock;
        }

        public override string ToString()
        {
            return $"id: {_id}, 이름: {_name}, 카테고리: {_category}, 가격: {_price}, 재고: {_stock}";
        }
    }
}
