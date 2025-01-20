
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace GstMagazin.Views
{

    public class UserContactController : Controller
    {

        private readonly GstDbMagazin _context;


        public UserContactController(GstDbMagazin context)
        {
            _context = context;
        }
        public async Task<string> validaterecapatcha(string token)
        {

            var secretKey = "6LeGeqgqAAAAAKNy_IBNeoflMqmcSiE0cVbKCZsk";
            var client = new HttpClient();
            var result = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("secret", secretKey),
            new KeyValuePair<string, string>("response", token)
            }));

            var jsonResponse = await result.Content.ReadAsStringAsync();
            var reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(jsonResponse);

            if (!reCaptchaResponse.Success)
            {
                return "error";
            }
            if (reCaptchaResponse.Score <= 0.5)
            {
                return "scorelow ";
            }
            return "success";
        }

        [HttpPost]
        public async Task<IActionResult> UserContacterNous()
        {
            var token = Request.Form["recaptcha_token"].ToString();

            if(token == " ")
            {
                return Json(new { status = "error", message = "Échec de la vérification reCAPTCHA 'token est null' . Veuillez réessayer." });

            }

            var respons = await validaterecapatcha(token);

            if(respons == "error")
            {

                return Json(new { status = "error", message = "Échec de la vérification reCAPTCHA  . Veuillez réessayer." });
            }

            if (respons == "scorelow")
            {
                return Json(new { status = "error", message = "Activité suspecte détectée. Votre message n'a pas été envoyé." });

            }


            UserContact u1 = new UserContact
            {
                Name = Request.Form["Name"].ToString(),
                Email = Request.Form["Email"].ToString(),
                Message = Request.Form["Message"].ToString()
            };

            if(u1 != null)
            {
                try
                {
                    _context.userscontact.Add(u1);
                    await _context.SaveChangesAsync();
                    return Json(new { status = "success", message = "Votre message est bien envoyé." });
                }
                catch
                {
                    return Json(new { status = "error", message = "Erreur lors de l'enregistrement." });
                }
            }

            return Json(new { status = "error", message = "Model est non valider , Veuillez réessayer plus tard " });


        }

        // POST: UserContact/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.



    }
}
