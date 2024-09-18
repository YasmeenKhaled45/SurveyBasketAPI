using Microsoft.EntityFrameworkCore;

namespace SurveyBasket.Api.Abstractions
{
    public class PaginatedList<T> 
    {
        public PaginatedList(List<T> values,int pagenumber , int count , int pagesize) 
        {
            Items = values;
            PageNumber = pagenumber;
            TotalPages =(int)Math.Ceiling(count / (double)pagesize);
        }
        public List<T> Items { get; private set; }  
        public int PageNumber {  get; private set; }
        public int TotalPages { get; private set;}
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;


        public static async Task<PaginatedList<T>> Create(IQueryable<T> source,int pagenumber , int pagesize,CancellationToken cancellationToken=default)
        {
            var count = await source.CountAsync(cancellationToken);
            var items = await source.Skip((pagenumber-1)*pagesize).Take(pagesize).ToListAsync(cancellationToken);
            return new PaginatedList<T>(items, count, pagenumber,pagesize);
        }
    }
}
