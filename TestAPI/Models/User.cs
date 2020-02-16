using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public class User
    {
        public int OID {get;set;}
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email address is required field")]
        public string Email { get; set; }
        private DateTime _dob;
        public string DOB {
            get {
                return _dob.ToString("dd-MM-yyyy");
            }

            set {
                DateTime.TryParse(value, out _dob);
            }
        }
        public string PhoneNumber { get; set; }
        public UserImage UserImage { get; set; }

        private List<string> _hobbiesTagList;
        public List<string> HobbiesTagList {
            get {
                return _hobbiesTagList;
            }
            set {
                if (value != null)
                    _hobbiesTagList = value.Distinct().ToList();
            }
        }

    }

    public class UserImage
    {
        public string Large { get; set; }
        public string Thumbnail { get; set; }
    }

    
}
