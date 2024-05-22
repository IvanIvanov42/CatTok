using CatTok.Models;
using CatTok.Services.IServices;
using System.Net.Http.Json;

namespace CatTok.Services
{
    public class InstagramService : IInstagramService
    {
        private readonly HttpClient _httpClient;
        
        public InstagramService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("CatTokAPI"); ;
        }

        public async Task<IEnumerable<Media>?> GetMediasAsync()
        {
            await Task.Delay(1000);
            return medias.Where(media => media.MediaType == "IMAGE");
            var allMedias = await _httpClient.GetFromJsonAsync<IEnumerable<Media>>("api/Instagram/GetInstagramData");
            return allMedias.Where(media => media.MediaType == "IMAGE");
        }

        public string GetUser()
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> SendAuthorizationToken(string token)
        {
            return await _httpClient.PostAsJsonAsync("Instagram/AuthorizeUser", token);
        }

        private readonly List<Media> medias = new List<Media> {
            new() {
                Id = "17844390627093942",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams4-1.cdninstagram.com/v/t51.29350-15/399973985_928991208886110_6542415147449715065_n.jpg?_nc_cat=109&ccb=1-7&_nc_sid=18de74&_nc_ohc=r2JDKQJF-4oAb4VDmT8&_nc_ht=scontent-ams4-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfDW5k0MkVur6m3T3LuAk92SbhLDGz7Z1IEqTLjjS496sA&oe=66285C2A",
                Caption = null,
                Timestamp = "2023-11-07T22:18:56+0000"
            },
            new() {
                Id = "17876052719987395",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams2-1.cdninstagram.com/v/t51.29350-15/399675826_1457156311799190_4598153660110154189_n.jpg?_nc_cat=108&ccb=1-7&_nc_sid=18de74&_nc_ohc=xXsU7KfeXs4Ab67bDJf&_nc_ht=scontent-ams2-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfDGYqW2xYXYVt3oGIwUqw4DRQBgTAhpJlN3qpkO1suC7w&oe=66286F9A",
                Caption = null,
                Timestamp = "2023-11-07T22:18:37+0000"
            },
            new() {
                Id = "17918834450740363",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams2-1.cdninstagram.com/v/t51.29350-15/402452759_1486524368857836_1331539081186916105_n.jpg?_nc_cat=100&ccb=1-7&_nc_sid=18de74&_nc_ohc=prvtfu84dyQAb4YPmWU&_nc_ht=scontent-ams2-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfB4akgUw_KPIiGMqoagiBA38cOgkqVPG9gYYMQuccaOpw&oe=66285C2F",
                Caption = null,
                Timestamp = "2023-11-14T18:15:49+0000"
            },
            new() {
                Id = "17987055953524525",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams4-1.cdninstagram.com/v/t51.29350-15/401937062_357860300140085_2331033316458569619_n.jpg?_nc_cat=111&ccb=1-7&_nc_sid=18de74&_nc_ohc=9WKP2tvsNiQAb52gJ86&_nc_ht=scontent-ams4-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfDeKWeTkyqtmcVtPYa_E5mpzU9fDFTRELXGgwMB58WYHA&oe=66287219",
                Caption = null,
                Timestamp = "2023-11-14T18:16:44+0000"
            },
            new() {
                Id = "17988309566262879",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams4-1.cdninstagram.com/v/t51.29350-15/401846985_1069122344121967_2671828393383921173_n.jpg?_nc_cat=101&ccb=1-7&_nc_sId=18de74&_nc_ohc=L5DoLK9-lGAAb7RvVFl&_nc_ht=scontent-ams4-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfAIoGD3noFOmLnmPQm-eXteXKxfhib21W-830hmbHVRkA&oe=662852BE",
                Caption = null,
                Timestamp = "2023-11-14T18:17:44+0000"
            },
            new() {
                Id = "17999870321246514",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams2-1.cdninstagram.com/v/t51.29350-15/399904504_328346136485002_3084622152900747689_n.jpg?_nc_cat=108&ccb=1-7&_nc_sId=18de74&_nc_ohc=gymr0ohIrTQAb64pSNj&_nc_ht=scontent-ams2-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfBCAHwqZfH9R5H8OfEOJ2fnwefV5LS-2hP_9XL6D3WGmw&oe=66284E0F",
                Caption = null,
                Timestamp = "2023-11-07T22:19:34+0000"
            },
            new() {
                Id = "18011122426854264",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams2-1.cdninstagram.com/v/t51.29350-15/399409144_347384651303513_3435697976494893368_n.jpg?_nc_cat=106&ccb=1-7&_nc_sId=18de74&_nc_ohc=kn2Y_tMfaWIAb6_oWaZ&_nc_ht=scontent-ams2-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfBE5kDnwBG9s0DDFFxs-FXKYRArBaqVE7xKmF-MV0V_cw&oe=66285A95",
                Caption = null,
                Timestamp = "2023-11-07T22:19:14+0000"
            },
            new() {
                Id = "18012612277963958",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams2-1.cdninstagram.com/v/t51.29350-15/401755573_643680037846391_2860728159209895503_n.jpg?_nc_cat=106&ccb=1-7&_nc_sId=18de74&_nc_ohc=YpjSyFyXmHEAb4FDQ33&_nc_ht=scontent-ams2-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfAvU0C6499g5lUZwHh8yz2BPSUCTkzsOeApNdlQHqnVgQ&oe=66284512",
                Caption = null,
                Timestamp = "2023-11-14T18:17:01+0000"
            },
            new() {
                Id = "18014174419827401",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams4-1.cdninstagram.com/v/t51.29350-15/402389834_6943432995719810_3596803039871452529_n.jpg?_nc_cat=103&ccb=1-7&_nc_sId=18de74&_nc_ohc=Y-M0Z9dNgrYAb5Hj1sS&_nc_ht=scontent-ams4-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfC1QBJ9Tjv5DlETFe8xcR0VOqJ6SJzRd5tE1RQURVHxRw&oe=662870C1",
                Caption = null,
                Timestamp = "2023-11-14T18:17:15+0000"
            },
            new() {
                Id = "18015117367802451",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams2-1.cdninstagram.com/v/t51.29350-15/400210498_1065046748008668_3229411524310045319_n.jpg?_nc_cat=104&ccb=1-7&_nc_sId=18de74&_nc_ohc=ABxrwqRiyYgAb5xtcx_&_nc_ht=scontent-ams2-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfBpKbeiHactaowcMLu-kziIXEtDFG-LvHPu-N9CVL5AbQ&oe=66284AC5",
                Caption = null,
                Timestamp = "2023-11-07T22:23:27+0000"
            },
            new() {
                Id = "18029811523724920",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams2-1.cdninstagram.com/v/t51.29350-15/399350534_150766251459647_6406879824100492250_n.jpg?_nc_cat=108&ccb=1-7&_nc_sId=18de74&_nc_ohc=_KaZBZleO7gAb7_7RCc&_nc_ht=scontent-ams2-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfCu-m1eVa-zdeM86YkVoDZBqc7r09RnL10W7TsDSsl8Dg&oe=66285232",
                Caption = null,
                Timestamp = "2023-11-07T22:17:56+0000"
            },
            new() {
                Id = "18037981492591203",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams4-1.cdninstagram.com/v/t51.29350-15/402347421_1535621140582953_6340155927428061968_n.jpg?_nc_cat=102&ccb=1-7&_nc_sId=18de74&_nc_ohc=wmXMOy4lvK8Ab6W7_MG&_nc_ht=scontent-ams4-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfBOFr9cRlx39KbUETaD5roV6Ils7VTXhbPYXV-Pd9xogQ&oe=66285C94",
                Caption = null,
                Timestamp = "2023-11-14T18:15:28+0000"
            },
            new() {
                Id = "18059654452475880",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams4-1.cdninstagram.com/v/t51.29350-15/401603686_1310227096520157_3641289326515481271_n.jpg?_nc_cat=111&ccb=1-7&_nc_sId=18de74&_nc_ohc=X3UX8l2xpB4Ab62xRuu&_nc_ht=scontent-ams4-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfBtMQ5hvfAaaTORAEM0djSfdgBSxs0e-4GbScu8Rb-lBA&oe=66285282",
                Caption = "wake up, is the first of the month",
                Timestamp = "2023-11-14T18:17:56+0000"
            },
            new() {
                Id = "18105199444354481",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams2-1.cdninstagram.com/v/t51.29350-15/402479992_1261016694575559_6074951415581975837_n.jpg?_nc_cat=106&ccb=1-7&_nc_sId=18de74&_nc_ohc=afnD9Scz4o4Ab7ieqXq&_nc_ht=scontent-ams2-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfDw3s8u6HrPL1_pjfqtNbRq4PotX9dObSxHwgNyuIaOPQ&oe=662841D1",
                Caption = null,
                Timestamp = "2023-11-14T18:17:38+0000"
            },
            new() {
                Id = "18209059213260436",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams4-1.cdninstagram.com/v/t51.29350-15/399394510_835239744966372_7277791946420851683_n.jpg?_nc_cat=109&ccb=1-7&_nc_sId=18de74&_nc_ohc=nHOLPdfRqGYAb7GrZIY&_nc_oc=Adhf507lgc_owHCb28PE1An_uajwIL8e0h2ywFupR7I4NUq4IQmfj4Jn4kRepS7f9Hs&_nc_ht=scontent-ams4-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfAF0EXj_89wJV6HOM9-Ym2Mw7JfOAYgX0w69MAexWT4ng&oe=66285EAC",
                Caption = null,
                Timestamp = "2023-11-07T22:17:41+0000"
            },
            new() {
                Id = "18215076577257174",
                MediaType = "IMAGE",
                MediaUrl = "https://scontent-ams4-1.cdninstagram.com/v/t51.29350-15/399577663_1151325876293374_5243546870678571435_n.jpg?_nc_cat=101&ccb=1-7&_nc_sId=18de74&_nc_ohc=JSSYL77QUd0Ab46dnJb&_nc_ht=scontent-ams4-1.cdninstagram.com&edm=ANo9K5cEAAAA&oh=00_AfDRPDRdwL8qMu2lmCjFSXrXc0UG7_jhveyZgYO35m8Cww&oe=66284FF4",
                Caption = null,
                Timestamp = "2023-11-07T22:18:14+0000"
            }
        };
    }
}