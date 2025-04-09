using HospitalApp.Models;

namespace HospitalApp.Services
{
    public class UserSessionService
    {
        private static readonly UserSessionService _instance = new UserSessionService();

        public static UserSessionService Instance => _instance;

        private UserSessionService() { }

        public User CurrentUser { get; private set; }

        public void SetUser(User user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
