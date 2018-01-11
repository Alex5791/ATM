using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ATM.Models;
using System.Web.Security;

namespace ATM.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(Index model)
        {
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд, авторизация по логину и паролю, в зависимости от роли будем давать разные представления
                User user = null;
                string Role = "";
                using (UserContext db = new UserContext())
                {
                    user = db.Users.First(u => u.Login == model.Name && u.Password == model.Password);
                    Role = db.Users.First(u => u.Login == model.Name && u.Password == model.Password).Role;

                }
                if (user != null && Role == "менеджер")
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true);
                    return RedirectToAction("UserList", "Home");
                }
                else if (user != null && Role == "клиент")
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true);
                    return RedirectToAction("UserInfo", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином или паролем не существует");
                }
            }
            return View(model);
        }

        UserContext db = new UserContext();
        TransferContext dbt = new TransferContext();
        [Authorize]
        public ActionResult UserList()
        {
            string role = "";
            string CurUserName = "";
            string CurUserSurname = "";
                using (UserContext db = new UserContext())
                {
                    role = db.Users
                                   .First(u => u.Login == User.Identity.Name)
                                   .Role;
                    CurUserSurname = db.Users
                                    .First(u => u.Login == User.Identity.Name)
                                    .Surname;
                    CurUserName = db.Users
                                    .First(u => u.Login == User.Identity.Name)
                                    .Name;
                }
            if (role == "менеджер")
            {
                ViewBag.Surname = CurUserSurname;
                ViewBag.Name = CurUserName;
                return View(db.Users);
            }
            else return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public ActionResult UserInfo()
        {
            string role = "";
            string CurUserName = "";
            string CurUserSurname = "";
                using (UserContext db = new UserContext())
                {
                    role = db.Users
                                   .First(u => u.Login == User.Identity.Name)
                                   .Role;
                    CurUserSurname = db.Users
                                    .First(u => u.Login == User.Identity.Name)
                                    .Surname;
                    CurUserName = db.Users
                                    .First(u => u.Login == User.Identity.Name)
                                    .Name;
                }
                ViewBag.Surname = CurUserSurname;
                ViewBag.Name = CurUserName;
                return View();
        }



        public ActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddUser(User adduser)
        {
            db.Users.Add(adduser);
            db.SaveChanges();
            return RedirectToAction("UserList", "Home");
        }

        [HttpGet]
        public ActionResult EditUser(int id)
        {
            var editedUser = db.Users
                                .First(u => u.UserId == id);
            return View(editedUser);
        }

        [HttpPost]
        public ActionResult EditUser(User editedUser)
        {
            var user = db.Users.First(u => u.UserId == editedUser.UserId);
            user.Surname = editedUser.Surname;
            user.Name = editedUser.Name;
            user.Patronymic = editedUser.Patronymic;
            user.Login = editedUser.Login;
            user.Password = editedUser.Password;
            user.Telephone = editedUser.Telephone;
            user.Cardnumber = editedUser.Cardnumber;
            user.Balance = editedUser.Balance;
            user.Role = editedUser.Role;
            db.SaveChanges();
            return RedirectToAction("UserList", "Home");
        }


        public ActionResult CheckBalance()
        {
            int Balance = db.Users.First(u => u.Login == User.Identity.Name).Balance;
            ViewBag.Balance = Balance;
            return View();
        }

        public ActionResult TelTransfer()
        {
            int Balance = db.Users.First(u => u.Login == User.Identity.Name).Balance;
            ViewBag.Balance = Balance;
            return View();
        }

        [HttpPost]
        public ActionResult TelTransfer(int transferVal, string telNumber)
        {
            var user = db.Users.First(u => u.Login == User.Identity.Name);
            var newTransfer = new Transfer();
            newTransfer.Money = transferVal;
            newTransfer.Operation = "Оплата сотовой связи";
            newTransfer.Receiver = telNumber;
            newTransfer.Sender = string.Concat(user.Name, " ", user.Surname);
            newTransfer.Date = DateTime.Now;
            if (user.Balance >= transferVal)
            {
                user.Balance -= transferVal;
                dbt.Transfers.Add(newTransfer);
                db.SaveChanges();
                dbt.SaveChanges();
                return RedirectToAction("UserInfo", "Home");
            }
            else
            {
                ViewBag.Error = "У вас недостаточно средств для операции";
                int Balance = db.Users.First(u => u.Login == User.Identity.Name).Balance;
                ViewBag.Balance = Balance;
                return View();
            }
        }

        public ActionResult CardTransfer()
        {
            int Balance = db.Users.First(u => u.Login == User.Identity.Name).Balance;
            ViewBag.Balance = Balance;
            return View();
        }

        [HttpPost]
        public ActionResult CardTransfer(string cardNumber, int transferVal)
        {
            var FromUser = db.Users.First(u => u.Login == User.Identity.Name);
            var newTransfer = new Transfer();
            newTransfer.Money = transferVal;
            newTransfer.Operation = "Перевод на банковскую карту";
            newTransfer.Receiver = cardNumber;
            newTransfer.Sender = string.Concat(FromUser.Name, " ", FromUser.Surname);
            newTransfer.Date = DateTime.Now;
            if (FromUser.Balance <= transferVal)
            {
                ViewBag.Error = "У вас недостаточно средств для операции";
                int Balance = db.Users.First(u => u.Login == User.Identity.Name).Balance;
                ViewBag.Balance = Balance;
                return View();
            }
            else if (cardNumber == ""){
                ViewBag.Error = "Введите номер карты пользователя";
                int Balance = db.Users.First(u => u.Login == User.Identity.Name).Balance;
                ViewBag.Balance = Balance;
                return View();
            }
            else
            {
                var ToUser = db.Users.First(u => u.Cardnumber == cardNumber);
                FromUser.Balance -= transferVal;
                ToUser.Balance += transferVal;
                dbt.Transfers.Add(newTransfer);
                db.SaveChanges();
                dbt.SaveChanges();
                return RedirectToAction("UserInfo", "Home");
            }  
        }
        public ActionResult TransferList()
        {
            string role = "";
            string CurUserName = "";
            string CurUserSurname = "";
            using (UserContext db = new UserContext())
            {
                role = db.Users
                               .First(u => u.Login == User.Identity.Name)
                               .Role;
                CurUserSurname = db.Users
                                .First(u => u.Login == User.Identity.Name)
                                .Surname;
                CurUserName = db.Users
                                .First(u => u.Login == User.Identity.Name)
                                .Name;
            }
            if (role == "менеджер")
            {
                ViewBag.Surname = CurUserSurname;
                ViewBag.Name = CurUserName;
                return View(dbt.Transfers);
            }
            else return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}