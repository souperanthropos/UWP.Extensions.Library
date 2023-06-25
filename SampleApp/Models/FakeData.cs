using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using static Bogus.DataSets.Name;

namespace SampleApp.Models
{
    public class FakeData
    {
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
    }
}
