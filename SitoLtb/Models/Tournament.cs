﻿namespace SitoLtb.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime Data { get; set; }

        public string LinkBando { get; set; }

        public string LinkPreiscrizione { get; set; }
        public string Url { get; set; }

    }
}
