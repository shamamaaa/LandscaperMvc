﻿namespace Landscaper.Areas.Admin.ViewModels
{
    public class PaginationVm<T> where T : class,new()
    {
        public double TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public List<T> Items { get; set; }
    }
}
