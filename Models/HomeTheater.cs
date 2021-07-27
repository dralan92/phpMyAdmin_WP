using System;
using System.Collections.Generic;
using System.Text;

namespace phpMyAdmin.Models
{
    class HomeTheater
    {
    }

    public class HowToChoose_PageData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Features { get; set; }


    }

    public class Receiver_PageData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Channels { get; set; }
        public string Power { get; set; }
        public string Connections { get; set; }
        public string Video { get; set; }
        public string Audio { get; set; }
        public string Room { get; set; }
        public string Amazon { get; set; }
        public string[] Tips { get; set; }

    }

}
