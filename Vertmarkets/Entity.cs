using System;
using System.Collections.Generic;
using System.Text;

namespace Vertmarkets
{
   public class RequestToken
    {
        public string Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }

     public class MagazineList: RequestToken
    {
       public List<Magazine> Data { get; set; }
    }

    public class Subscriber
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<int> MagazineIds { get; set; }

      
    }

    public class Magazine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MagazineCategories { get; set; }
    }

    public class Categories: RequestToken
    {
        public List<string> Data { get; set; }
       
    }

    public class SubscriberList: RequestToken
    {
        public List<Subscriber> Data { get; set; }
      
    }



    }
