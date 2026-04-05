namespace API.Helpers
{
    public class LikesParams : PagingParams
    {
        public string MemberId { get; set; } = "";
        public string Predicated { get; set; } = "liked";

    }
}
