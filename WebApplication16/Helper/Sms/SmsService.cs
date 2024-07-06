using WebApplication16.Models;

namespace WebApplication16.Helper.Sms
{
    public class SmsService
    {

        public async Task<string> Send(string phonenumber)
        {

            Random random = new Random();
            int randomint = random.Next(999, 9999);

            string urlAPI = $"https://heydaroghlu.com/api/Accounts/Sms?number={phonenumber}&message= Tesdiq Kodunuz: {randomint}";


            using (HttpClient client = new HttpClient())
            {

                HttpResponseMessage message = await client.GetAsync(urlAPI);
                string result = await message.Content.ReadAsStringAsync();
               

            }
            return  randomint.ToString();

        }
    }
}
