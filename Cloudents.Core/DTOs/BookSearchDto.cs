using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudents.Core.DTOs
{
    public class BookSearchDto
    {
        public string Isbn10 { get; set; }
        public string Isbn13 { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Edition { get; set; }
        public string Binding { get; set; }
        public string Image { get; set; }
        /*
         struct BookDto {
    let isbn10:String
    let isbn13:String
    let title:String
    let author:String
    let edition:String
    let binding: String
    let imageUrl:String?
    
    
    init(json:JSON) {
        
        /*{
         "isbn10": "1259291820",
         "isbn13": "9781259291821",
         "title": "The Macro Economy Today, 14 Edition (The Mcgraw-Hill Series in Economics)",
         "author": "Bradley R Schiller - Karen Gebhardt",
         "binding": "Paperback",
         "msrp": "",
         "pages": "536",
         "publisher": "McGraw-Hill Education",
         "published_date": "2015-02-20",
         "edition": "14",
         "rank": 30930,
         "rating": 0,
         "image": "https://images-na.ssl-images-amazon.com/images/I/41H%2Bmk3q8ZL._SL160_.jpg"
         },
        isbn10 = json["isbn10"].stringValue
            isbn13 = json["isbn13"].stringValue
        title = json["title"].stringValue
            author = json["author"].stringValue
        binding = json["binding"].stringValue
            edition = json["edition"].stringValue
        imageUrl = json["image"].string



    }


}*/
    }
}
