
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            // Basic RFC 5322 compliant email format regex
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // Phone Format Validator (Exactly 10 digits)
        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^\d{10}$");
        }


        public void CreateUser(UserModel user)
        {
            // Input Validation
            if (string.IsNullOrWhiteSpace(user.FullName))
                throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required.");
            if (!IsValidEmail(user.Email))
                throw new ArgumentException("Email is not currect.");
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Password is required.");
            if (string.IsNullOrWhiteSpace(user.Phone))
                throw new ArgumentException("Phone number is required.");
            if (!IsValidPhone(user.Phone))
                throw new ArgumentException("Phone number must be exactly 10 digits.");
            // Password Hashing
            user.Password = PasswordHasher.HashPassword(user.Password);
            _userDal.CreateUser(user);
        }

        public List<UserModel> GetAllUsers()
        {
            return _userDal.GetAllUsers();
        }

        public UserModel GetUserById(int id)
        {
            var user = _userDal.GetUserById(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");
            return user;
        }

        public void UpdateUser(UserModel user)
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
            var existing = _userDal.GetUserById(user.Id);
            if (existing == null)
                throw new KeyNotFoundException($"User with ID {user.Id} not found.");
            _userDal.UpdateUser(user);
        }

        public void DeleteUser(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Valid user ID is required.");
            var user = _userDal.GetUserById(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");
            _userDal.DeleteUser(id);
        }
    }
}
