using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication4.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System.Text;

namespace WebApplication4.Models
{
    public class Post
    {
        public int PostID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ApplicationUserId { get; set; }        
        public DateTime Created { get; set; }
        public string Author { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }

    public static class StringExtension
    {
        public static bool ConteinsCyrillic(this String str, String searchString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encodedStr = utf8.GetBytes(str.ToLower());
            byte[] encodedSearchStr = utf8.GetBytes(searchString.ToLower());
            if (ContainsArr(encodedStr, encodedSearchStr))
                return true;
            return false;
        }

        private static bool ContainsArr(byte[] arr1, byte[] arr2)
        {
            int i = 0;
            while (i < arr1.Length)
            {
                if(arr1[i] == arr2[0])
                {
                    int k = 1, i1 = i + 1;
                    for (int j = 0; j < arr2.Length; j++)
                    {
                        if (i1 < arr1.Length && arr1[i1] == arr2[j])
                        {
                            k++;
                            i1++;
                        }
                    }
                    if (k == arr2.Length)
                        return true;
                }
                i++;
            }
            return false;
        }
    }

}
