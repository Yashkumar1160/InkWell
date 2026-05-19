using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InkWell.Auth.DTOs;
using InkWell.Auth.Models;
using InkWell.Auth.Repository.Interface;
using InkWell.Auth.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;


namespace AuthService.Services.Service
{
    public class AuthServiceImpl : IAuthService
    {
        // user repository instance
        private IUserRepository userRepository;

        // IConfiguration is used to read jwt settings from appsettings.json
        private IConfiguration configuration;


        // Constructor Dependency Injection
        public AuthServiceImpl(IUserRepository repository, IConfiguration config)
        {
            userRepository = repository;
            configuration = config;
        }


        // Create new account
        public async Task<AuthResponseDTO> Register(RegisterDTO dto)
        {
            // Check if email is already registered
            bool emailTaken = await userRepository.EmailExists(dto.Email);

            if (emailTaken == true)
            {
                throw new Exception("This email is already registered.");
            }

            // Check if username is already taken
            bool usernameTaken = await userRepository.UsernameExists(dto.Username);

            if (usernameTaken == true)
            {
                throw new Exception("This username is already registered.");
            }

            // Create new user object
            User newUser = new User
            {
                Username = dto.Username,
                Email = dto.Email,

                // Hash password
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                // Everyone starts as reader
                Role = "READER",
                // Registered with email and password
                Provider = "LOCAL",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Save to database
            User savedUser = await userRepository.Add(newUser);

            // return response with JWT token so user is logged in immediately
            AuthResponseDTO response = BuildResponse(savedUser);
            return response;
        }

        // Login user (verify credentials and return token)
        public async Task<AuthResponseDTO> Login(LoginDTO dto)
        {
            // Find user by email
            User user = await userRepository.GetByEmail(dto.Email);

            if (user == null)
            {
                throw new Exception("Wrong email or password");
            }

            // Check if account is active
            if (user.IsActive == false)
            {
                throw new Exception("This account has been suspended.");
            }

            // Verify password
            bool passwordCorrect = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            
            if (!passwordCorrect)
            {
                throw new Exception("Wrong email or password");
            }

            // Return response with JWT token
            AuthResponseDTO response = BuildResponse(user);
            return response;
        }

        // Get Profile (return user info)
        public async Task<object> GetProfile(int userId)
        {

            User user = await userRepository.GetById(userId);
            // Check if user exists
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // return profile object instead of user model so hashed password is not sent to client
            var userProfile = new
            {
                user.UserId,
                user.Username,
                user.Email,
                user.FullName,
                user.Role,
                user.Bio,
                user.AvatarUrl,
                user.IsActive,
                user.CreatedAt
            };

            return userProfile;
        }

        // Update profile (change name, bio, avatar)
        public async Task<object> UpdateProfile(int userId, UpdateProfileDTO dto)
        {
            // Find user from database
            User user = await userRepository.GetById(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // update fields
            user.FullName = dto.FullName;
            user.Bio = dto.Bio ?? "";
            user.AvatarUrl = dto.AvatarUrl ?? "";

            // Save changes
            await userRepository.Update(user);

            // return updated profile
            return new
            {
                user.UserId,
                user.Username,
                user.Email,
                user.FullName,
                user.Role,
                user.Bio,
                user.AvatarUrl,
                user.IsActive,
                user.CreatedAt
            };
        }

        // Change Password ( verify old password then set new password)
        public async Task ChangePassword(int userId, ChangePasswordDTO dto)
        {
            // Find user from database
            User user = await userRepository.GetById(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Verify old password
            bool oldPassword = BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash);
            if (!oldPassword)
            {
                throw new Exception("Old password is incorrect");
            }

            // Hash new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            // Save to database
            await userRepository.Update(user);
        }

        // Deactivate account (ADMIN action)
        public async Task Deactivate(int userId)
        {
            // Find user from database
            User user = await userRepository.GetById(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // deactivate account
            user.IsActive = false;

            // Save changes
            await userRepository.Update(user);
        }

        // Change role (ADMIN action)
        public async Task ChangeRole(int userId, string role)
        {
            // only these three values are valid roles
            if (role != "READER" && role != "AUTHOR" && role != "ADMIN")
            {
                throw new Exception("Role must be READER, AUTHOR or ADMIN.");
            }

            // Find user from database
            User user = await userRepository.GetById(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // set role
            user.Role = role;

            // save changes to database
            await userRepository.Update(user);
        }

        // Logout User
        public async Task Logout()
        {
            // Angular deleted the token from local storage on logout
            await Task.CompletedTask;
        }

        // Validate JWT token
        public bool ValidateToken(string token)
        {
            try
            {
                // set up validation rules same as Program.cs
                byte[] keyBytes = Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]);
                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(keyBytes);

                TokenValidationParameters parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true
                };

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;

                // this will thorw an exception when the token is invalid or expired
                handler.ValidateToken(token, parameters, out validatedToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Refresh Token
        public async Task<AuthResponseDTO> RefreshToken(string oldToken)
        {
            // Read claims from the old token without validating expiry
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(oldToken);

            // get user id from token claims
            string id = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (id == null)
            {
                throw new Exception("Invalid token.");
            }

            int userId = int.Parse(id);

            // read user from database
            User user = await userRepository.GetById(userId);

            if (user == null || user.IsActive == false)
            {
                throw new Exception("User nor found or suspended.");
            }

            // Generate new token with 24 hrs expiry
            AuthResponseDTO response = BuildResponse(user);
            return response;
        }


        // Get user by email
        public async Task<object> GetUserByEmail(string email)
        {
            // Find user by email
            User user = await userRepository.GetByEmail(email);

            if (user == null)
            {
                throw new Exception("No user found with that email.");
            }

            // return profile object without hashed password
            var profile = new
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return profile;
        }

        // Get all users
        public async Task<List<object>> GetAllUsers()
        {
            // list to store users
            List<User> users = await userRepository.GetAll();

            // convert each user to profile object
            List<object> result = new List<object>();

            foreach (User user in users)
            {
                var profile = new
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    Bio = user.Bio,
                    AvatarUrl = user.AvatarUrl,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                result.Add(profile);
            }

            return result;
        }


        // // Seach users with specific keyword
        public async Task<List<object>> SearchUsers(string keyword)
        {
            // Find users from database
            List<User> users = await userRepository.Search(keyword);

            // Convert users to profile object
            List<object> result = new List<object>();

            foreach (User user in users)
            {
                var profile = new
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    IsActive = user.IsActive
                };

                result.Add(profile);
            }

            return result;
        }

        // Get users by specific role
        public async Task<List<object>> GetUsersByRole(string role)
        {
            // verify role
            if (role != "READER" && role != "AUTHOR" && role != "ADMIN")
            {
                throw new Exception("Role must be READER, AUTHOR or ADMIN.");
            }

            // Find users by role from database
            List<User> users = await userRepository.GetAllByRole(role);

            // convert each user to profile object
            List<object> result = new List<object>();

            foreach (User user in users)
            {
                var profile = new
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                result.Add(profile);
            }

            return result;
        }


        // Reactivate account (admin only)
        public async Task Reactivate(int userId)
        {
            // Find user from database
            User user = await userRepository.GetById(userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // if user account is already active
            if (user.IsActive == true)
            {
                throw new Exception("This account is already active.");
            }

            // activate account
            user.IsActive = true;
            await userRepository.Update(user);
        }

        // Delete user from database (admin only)
        public async Task DeleteUser(int userId)
        {
            // Find user from database
            User user = await userRepository.GetById(userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // delete the user using user repository
            await userRepository.DeleteById(userId);
        }


        // Build the response object sent back to Angular
        private AuthResponseDTO BuildResponse(User user)
        {
            AuthResponseDTO response = new AuthResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Token = GenerateToken(user)
            };
            return response;
        }


        // Generate signed JWT token for user
        private string GenerateToken(User user)
        {
            // Get the secret key from appsettings.json
            // this key is used to sign the token so we can verify it later
            string secret = configuration["Jwt:Secret"];
            byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(keyBytes);

            // Create signing credentials using HMAC SHA256 algorithm
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            // Add Claims(data packed inside tokens)
            Claim[] claims = new Claim[]
            {
                // user id (used to find the user in any service)
                new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                
                // email (used for display purpose)
                new Claim(ClaimTypes.Email, user.Email),
                
                // username (used for author name mapping)
                new Claim(ClaimTypes.Name, user.Username),
                
                // role (used for authorization) [Authorize]
                new Claim(ClaimTypes.Role, user.Role),

                // full name (for personalized notifications)
                new Claim("FullName", user.FullName ?? user.Username)
            };

            // Token expires after 24 hrs
            DateTime expiry = DateTime.UtcNow.AddHours(24);

            // Create the actual JWT token object
            JwtSecurityToken token = new JwtSecurityToken(
                // Who created token
                issuer: configuration["Jwt:Issuer"],
                // Who should use the token
                audience: configuration["Jwt:Audience"],
                // Data inside token
                claims: claims,
                // Token expiry
                expires: expiry,
                // Signature to prevent tampering
                signingCredentials: credentials
            );

            // Convert to string that angular stores
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        // Google OAuth Login
        public async Task<AuthResponseDTO> GoogleLogin(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { configuration["Google:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                // Find user by email
                User user = await userRepository.GetByEmail(payload.Email);

                if (user == null)
                {
                    // Create new user for first time Google login
                    user = new User
                    {
                        Email = payload.Email,
                        Username = payload.Email.Split('@')[0] + "_" + Guid.NewGuid().ToString().Substring(0, 4),
                        FullName = payload.Name,
                        AvatarUrl = payload.Picture,
                        Role = "READER",
                        Provider = "GOOGLE",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()) // Random password
                    };

                    user = await userRepository.Add(user);
                }
                else if (user.IsActive == false)
                {
                    throw new Exception("This account has been suspended.");
                }

                return BuildResponse(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Google authentication failed: " + ex.Message);
            }
        }


    }
}