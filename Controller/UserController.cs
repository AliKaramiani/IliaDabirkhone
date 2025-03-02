using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kavenegar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
[Route("[Action]")]
[ApiController]
public class UserController : Controller
{
    private readonly string salt = "S@lt?";
    private readonly Context db;
    public UserController(Context _db)
    {
        db = _db;
    }

    [HttpPost]
    public IActionResult Register(DtodUser user)
    {
        if (user.IsNullOrEmpty())
        {
            return Ok("Complete Data Pls");
        }
        Users check = db.Users_tbl.FirstOrDefault(x => x.Username == user.Username || x.NatinalCode == user.NatinalCode || x.Phone == user.Phone);
        var conditions = new List<(Func<bool> Condition, string Message)>
{
    (() => check.Username == user.Username.ToLower(), "Invalid Username"),
    (() => check.NatinalCode == user.NatinalCode, "Invalid Natinal Code"),
    (() => check.Phone == user.Phone, "Invalid Phone")
};

        if (check != null)
        {
            foreach (var (condition, message) in conditions)
            {
                if (condition())
                {
                    return Ok(message);
                }
            }
        }

        var NewUser = new Users
        {
            Username = user.Username.ToLower(),
            Password = BCrypt.Net.BCrypt.HashPassword(user.Password + salt + user.Username.ToLower()),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            Addres = user.Addres,
            NatinalCode = user.NatinalCode,
            PerconalCode = user.PerconalCode,
            Profile = Uploadimage.Upload(user.Profile),
            CreateDateTime = DateTime.Now
        };
        db.Users_tbl.Add(NewUser);
        db.SaveChanges();
        //              --- add role
        db.UserRoles_tbl.Add(new UserRole
        {
            UserId = (int)NewUser.Id,
            RoleId = 2,
        });
        CreateUserLog((int)NewUser.Id, 3, true);
        db.SaveChanges();
        return Ok("Succesful !");
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult GoAdmin(int UserId)
    {
        Users check = db.Users_tbl.FirstOrDefault(x => x.Id == UserId);
        if (check == null)
        {
            return Ok("؟ کاربر اشتباهه");
        }
        else if (!db.UserRoles_tbl.Any(x => x.UserId == UserId && x.RoleId == 1))
        {
            db.UserRoles_tbl.Add(new UserRole
            {
                UserId = UserId,
                RoleId = 1
            });
            db.SaveChanges();
            return Ok("افرین ماموریت با موفقیت انجام شد");
        }
        else
        {
            return Ok($"به خدا {check.Username} ادمینه !");
        }
    }

    [HttpPost]
    public IActionResult Login(string Username, string Password)
    {
        Users check = db.Users_tbl.FirstOrDefault(x => x.Username == Username.ToLower());
        if (check == null)
        {
            return NotFound($"{Username} not found");
        }
        else if (!BCrypt.Net.BCrypt.Verify(Password + salt + Username.ToLower(), check.Password))
        {
            CreateUserLog((int)check.Id, 1, false);
            return Ok("Invalid Password!");
        }

        CreateUserLog((int)check.Id, 1, true);

        int UserId = Convert.ToInt32(check.Id);
        List<int> UserRoles = db.UserRoles_tbl.Where(ur => ur.UserId == UserId).Select(ur => ur.RoleId).ToList();

        string role = string.Join(",", UserRoles.Select(item => db.Role_tbl.FirstOrDefault(r => r.Id == item)?.Name));

        List<string> permissions = new List<string>();
        foreach (var item in UserRoles)
        {
            List<int> RolePermissions = db.RolePermissions_tbl.Where(rp => rp.RoleId == item).Select(rp => rp.PermissionId).ToList();

            foreach (var item2 in RolePermissions)
            {
                string permissionName = db.Permission_tbl.FirstOrDefault(p => p.Id == item2)?.Name;
                if (!permissions.Contains(permissionName))
                {
                    permissions.Add(permissionName);
                }
            }
        }

        return Ok(CreateToken(Username.ToLower(), role.Split(','), permissions.ToArray()));
    }

    [HttpPut]
    public IActionResult ResetPassword(string Username, string NatinalCode)
    {
        Users check = db.Users_tbl.FirstOrDefault(x => x.Username == Username.ToLower() && x.NatinalCode == NatinalCode);
        if (check == null)
        {
            return Ok("Invalid Data");
        }

        // sms check
        smsUser request = db.sms_tbl.FirstOrDefault(x => x.UserId == check.Id);



        if (request != null)
        {
            if (DateTime.Now.AddMinutes(-10) < request.CreateDateTime)
            {

                CreateUserLog((int)check.Id, 4, false);

                return Ok("you Must Wait about 10 min");
            }
            else
            {
                db.sms_tbl.Remove(request);
            }
        }
        Random random = new Random();
        smsUser newSms = new smsUser
        {
            TryCount = 0,
            SmsCode = random.Next(100000, 999999).ToString(),
            UserId = (int)check.Id,
            IsValid = true,
            CreateDateTime = DateTime.Now
        };
        db.sms_tbl.Add(newSms);
        db.SaveChanges();

        CreateUserLog((int)check.Id, 4, true);

        return Ok(SmsCode(newSms.SmsCode, check.Phone)
        );
    }

    [HttpPut]
    public IActionResult VerifyPassword(string Username, string NewPassword, string ConfirmPassword, string Code)
    {
        if (NewPassword != ConfirmPassword)
        {
            return Ok("Passwords Are not Match");
        }
        Users check = db.Users_tbl.FirstOrDefault(x => x.Username == Username.ToLower());
        if (check == null)
        {
            return Ok("Invalid User");
        }

        //sms Check
        smsUser smsCheck = db.sms_tbl.FirstOrDefault(x => x.UserId == check.Id);
        if (smsCheck == null)
        {
            CreateUserLog((int)check.Id, 5, false);
            return Ok("Haven't Code Requset. try Reset First");

        }
        else if (DateTime.Now.AddMinutes(-10) > smsCheck.CreateDateTime)
        { //Time Passed
            db.sms_tbl.Remove(smsCheck);
            db.SaveChanges();
            CreateUserLog((int)check.Id, 5, false);
            return Ok("Code Time Expire ... Try again");
        }
        else if (smsCheck.IsValid == true)
        {
            if (Code == smsCheck.SmsCode)
            {
                check.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword + salt + Username.ToLower());
                db.Users_tbl.Update(check);
                db.sms_tbl.Remove(smsCheck);
                db.SaveChanges();
                CreateUserLog((int)check.Id, 5, true);
                return Ok("Sucssesful");
            }
            else
            {
                if (smsCheck.TryCount > 3) // start from 0 -> 1,2,3,4 -> when 4 still can try 5 ! done
                    smsCheck.IsValid = false;
                else
                    ++smsCheck.TryCount;
                db.sms_tbl.Update(smsCheck);
                db.SaveChanges();
                CreateUserLog((int)check.Id, 5, false);
                return Ok("Code is Invalid");
            }
        }
        else
        {
            CreateUserLog((int)check.Id, 5, false);
            return Ok("you Must Try 10 min later.");
        }

    }
    [HttpPut]
    [Authorize]
   public IActionResult UpdateUser([FromQuery] DtoUpdateUser Data)
{
    // Retrieve the current user
    Users check = db.Users_tbl.FirstOrDefault(x => x.Username == User.FindFirstValue("username"));

    // Check if the user has the required permission to update user details
    if (!User.HasClaim(c => c.Type == "Permission" && c.Value == "UpdateUser"))
    {
        return Forbid("You do not have permission to update user details.");
    }

    // Update user details
    check.Addres = Data.Addres;
    check.FirstName = Data.FirstName;
    check.LastName = Data.LastName;
    check.Phone = Data.Phone;
    check.Profile = Uploadimage.Upload(Data.Profile);
    db.Users_tbl.Update(check);
    db.SaveChanges();

    // Log the update action
    CreateUserLog((int)check.Id, 6, true);

    return Ok("Done!");
}
[HttpPut]
public IActionResult Update()
{
    // Check if the user has permission to update a product
    if (User.HasClaim(c => c.Type == "Permission" && c.Value == "Update"))
    {
        return Ok("Update successful");
    }
    else
    {
        return Forbid("You do not have permission to update ");
    }
}

    [HttpGet]
    [Authorize(Roles = "admin")]
    public string Add()
    {

        return "Add  done";
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public string update()
    {

        return "Update done";
    }
    [HttpGet]
    [Authorize(Roles = "admin,client")]
    public string view()
    {
        return "Successful";
    }

    private string CreateToken(string Username, string[] role, string[] permission)
    {
        SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.Default.GetBytes("SymmetricSecurityKey secretKey Encoding.Default.GetBytes"));
        SigningCredentials Credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
         new Claim(ClaimTypes.Name,Username),
        //  new Claim(ClaimTypes.Role,role,permission )


        };
        claims.AddRange(role.Select(x => new Claim(ClaimTypes.Role, x)));
        claims.AddRange(permission.Select(permission => new Claim("Permission", permission)));

        var token = new JwtSecurityToken(
            issuer: "Issuer",
            audience: "Audience",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: Credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
    private string SmsCode(string Code, string Phone)
    {
        // real sms
        // KavenegarApi SmsApi = new KavenegarApi(db.smsTokens.Find(1).Token);
        // SmsApi.VerifyLookup(Phone, Code, "demo");
        // return "Sms Sended";

        // price less
        return $"{Code} Sent to {Phone} .";
    }

    private void CreateUserLog(int UserId, int LogAction, bool isSucces)
    {
        db.userLogs_tbl.Add(new UserLog
        {
            UserId = UserId,
            LogAction = LogAction,
            isSucces = isSucces,
            CreateDateTime = DateTime.Now
        });
        db.SaveChanges();
    }
}
