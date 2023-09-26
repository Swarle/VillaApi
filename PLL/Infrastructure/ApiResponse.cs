using System.Net;

namespace PLL.Infrastructure
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public object Result { get; set; }
        public List<string> ErrorMessage { get; set; } = new List<string>();

    }
}
