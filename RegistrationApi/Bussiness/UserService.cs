

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RegistrationApi.DataAccess;
using RegistrationApi.Models;
using RegistrationApi.Utils;

namespace RegistrationApi.Business
{
    public class UserService
    {
        private readonly UserDAL _userDal;

        public UserService(UserDAL userDal)
        {
            _userDal = userDal;
        }

        // Email Format Validator
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // Phone Format Validator (Exactly 10 digits)
        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^\d{10}$");
        }

        public async Task CreateUserAsync(UserModel user)
        {
            if (string.IsNullOrWhiteSpace(user.FullName))
                throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required.");
            if (!IsValidEmail(user.Email))
                throw new ArgumentException("Email is not correct.");
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Password is required.");
            if (string.IsNullOrWhiteSpace(user.Phone))
                throw new ArgumentException("Phone number is required.");
            if (!IsValidPhone(user.Phone))
                throw new ArgumentException("Phone number must be exactly 10 digits.");

            user.Password = PasswordHasher.HashPassword(user.Password);

            await _userDal.CreateUserAsync(user);
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            return await _userDal.GetAllUsersAsync();
        }

        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            var user = await _userDal.GetUserByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");
            return user;
        }

        public async Task UpdateUserAsync(UserModel user)
        {
            if (user.Id <= 0)
                throw new ArgumentException("Valid user ID is required.");
            if (string.IsNullOrWhiteSpace(user.FullName))
                throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required.");
            if (!IsValidEmail(user.Email))
                throw new ArgumentException("Email format is invalid.");
            if (string.IsNullOrWhiteSpace(user.Phone))
                throw new ArgumentException("Phone number is required.");
            if (!IsValidPhone(user.Phone))
                throw new ArgumentException("Phone number must be exactly 10 digits.");

            var existing = await _userDal.GetUserByIdAsync(user.Id);
            if (existing == null)
                throw new KeyNotFoundException($"User with ID {user.Id} not found.");

            await _userDal.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Valid user ID is required.");

            var user = await _userDal.GetUserByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");

            await _userDal.DeleteUserAsync(id);
        }
    }
}
