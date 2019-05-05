namespace JWTAuthApi.Models
{
    public class TokenModelForStorageModel
    {
        public int TokenId { get; set; }
        public string TokenToken { get; set; }
        public string TokenUser { get; set; }       
        public string TokenException { get; set; }
    }
}
